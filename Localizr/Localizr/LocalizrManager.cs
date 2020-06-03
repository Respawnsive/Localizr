using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Localizr
{
    public class LocalizrManager : ILocalizrManager
    {
        private readonly IEnumerable<ITextProvider> _textProviders;
        private readonly ILocalizrInitializationHandler _initializationHandler;
        private readonly IList<IDictionary<string, string>> _providersLocalizations;
        private readonly Subject<LocalizrState> _stateChanged;
        private readonly Subject<IList<CultureInfo>> _availableCulturesChanged;
        private readonly AsyncLock _availableCulturesLocker;
        private readonly AsyncLock _initializationLocker;

        public LocalizrManager(IEnumerable<ITextProvider> textProviders, ILocalizrInitializationHandler initializationHandler)
        {
            _textProviders = textProviders;
            _initializationHandler = initializationHandler;
            _providersLocalizations = new List<IDictionary<string, string>>();
            _stateChanged = new Subject<LocalizrState>();
            _availableCulturesChanged = new Subject<IList<CultureInfo>>();
            _availableCulturesLocker = new AsyncLock();
            _initializationLocker = new AsyncLock();
            AvailableCultures = new List<CultureInfo>();
        }

        IObservable<LocalizrState> ILocalizrManager.WhenLocalizrStatusChanged() => _stateChanged;

        public LocalizrState Status { get; private set; }

        public CultureInfo? CurrentCulture { get; private set; }

        public IObservable<IList<CultureInfo>> WhenAvailableCulturesChanged() => _availableCulturesChanged;

        public IList<CultureInfo> AvailableCultures { get; }

        public virtual async Task<bool> RefreshAvailableCulturesAsync(CancellationToken token = default)
        {
            try
            {
                using (await _availableCulturesLocker.LockAsync(token))
                {
                    AvailableCultures.Clear();
                    var availableCultures = new List<CultureInfo>();

                    foreach (var textProvider in _textProviders)
                    {
                        var providerAvailableCultures = await textProvider.GetAvailableCulturesAsync(token);
                        foreach (var providerAvailableCulture in providerAvailableCultures)
                        {
                            if (availableCultures.All(x => x.Name != providerAvailableCulture.Name))
                                availableCultures.Add(providerAvailableCulture);
                        }
                    }

                    var invariantCulture = availableCultures.FirstOrDefault(x => x.Name == CultureInfo.InvariantCulture.Name);
                    if (invariantCulture != null)
                    {
                        AvailableCultures.Add(invariantCulture);
                        availableCultures.Remove(invariantCulture);
                    }

                    foreach (var availableCulture in availableCultures.OrderBy(x => x.DisplayName))
                    {
                        AvailableCultures.Add(availableCulture);
                    }

                    return true; 
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _availableCulturesChanged.OnNext(AvailableCultures);
            }
        }

        public virtual async Task<bool> InitializeAsync(CultureInfo? culture = null, bool tryParents = true, bool refreshAvailableCultures = false, CancellationToken token = default)
        {
            var isFirstInitialization = Status == LocalizrState.Uninitialized;
            try
            {
                using (await _initializationLocker.LockAsync(token))
                {
                    return await _initializationHandler.OnInitializing(async () =>
                    {
                        var foundProvidersLocalizations =
                            new Dictionary<CultureInfo, IList<IDictionary<string, string>>>();
                        _providersLocalizations.Clear();
                        _stateChanged.OnNext(Status = LocalizrState.Initializing);

                        if (refreshAvailableCultures)
                            await RefreshAvailableCulturesAsync(token);

                        if (culture == null)
                            culture = CultureInfo.CurrentUICulture;

                        foreach (var textProvider in _textProviders)
                        {
                            var textResourcesCulture = culture.Name == textProvider.InvariantCulture?.Name
                                ? CultureInfo.InvariantCulture
                                : culture;
                            var currentCulture = culture;
                            var currentTryParents = tryParents;
                            while (currentTryParents)
                            {
                                var localizations =
                                    await textProvider.GetTextResourcesAsync(textResourcesCulture, token);
                                if (localizations != null && localizations.Any())
                                {
                                    var currentCultureLocalizations = foundProvidersLocalizations
                                        .FirstOrDefault(x => x.Key.Name == currentCulture.Name).Value;
                                    if (currentCultureLocalizations == null || !currentCultureLocalizations.Any())
                                    {
                                        currentCultureLocalizations = new List<IDictionary<string, string>>();
                                        foundProvidersLocalizations.Add(currentCulture, currentCultureLocalizations);
                                    }

                                    currentCultureLocalizations.Add(localizations);
                                    currentTryParents = false;
                                }
                                else if (textResourcesCulture.Name == CultureInfo.InvariantCulture.Name)
                                {
                                    currentTryParents = false;
                                }
                                else
                                {
                                    var parentCulture = currentCulture.Parent;
                                    textResourcesCulture = parentCulture.Name == textProvider.InvariantCulture?.Name
                                        ? CultureInfo.InvariantCulture
                                        : parentCulture;
                                    currentCulture = parentCulture;
                                }
                            }
                        }

                        if (foundProvidersLocalizations.Any())
                        {
                            if (foundProvidersLocalizations.Any(x => x.Key.Name != CultureInfo.InvariantCulture.Name) &&
                                foundProvidersLocalizations.Any(x => x.Key.Name == CultureInfo.InvariantCulture.Name))
                                foundProvidersLocalizations.Remove(CultureInfo.InvariantCulture);

                            var foundProvidersLocalizationsOrdered = foundProvidersLocalizations
                                .OrderByDescending(x => x.Key.Name.Length).ToList();
                            foreach (var foundCultureLocalizations in foundProvidersLocalizationsOrdered)
                            {
                                foreach (var foundLocalizations in foundCultureLocalizations.Value)
                                {
                                    _providersLocalizations.Add(foundLocalizations);
                                }
                            }

                            CurrentCulture = foundProvidersLocalizationsOrdered.First().Key;
                        }

                        _stateChanged.OnNext(Status =
                            !_providersLocalizations.Any() ? LocalizrState.None : LocalizrState.Some);

                        return _providersLocalizations.Any();
                    }, isFirstInitialization);
                }
            }
            catch (Exception)
            {
                _stateChanged.OnNext(Status = LocalizrState.Error);
                return false;
            }
            finally
            {
                await _initializationHandler.OnInitialized(Status, isFirstInitialization);
            }
        }

        public virtual string GetText(string key)
        {
            try
            {
                foreach (var providerLocalizations in _providersLocalizations)
                {
                    if (providerLocalizations.TryGetValue(key, out var value))
                        return value;
                }

                return $"[{key}]";
            }
            catch (Exception)
            {
                return $"[{key}]";
            }
        }
    }
}
