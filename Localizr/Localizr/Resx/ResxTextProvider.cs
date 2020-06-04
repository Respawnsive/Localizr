using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace Localizr.Resx
{
    /// <summary>
    /// Resx text provider
    /// </summary>
    /// <typeparam name="T">Should be a Resx Designer class</typeparam>
    public class ResxTextProvider<T> : IResxTextProvider<T> where T : class
    {
        private readonly ResourceManager _resourceManager;
        private TaskCompletionSource<IList<CultureInfo>> _availableCulturesTcs;
        private TaskCompletionSource<IDictionary<string, string>> _textResourcesTcs;

        public ResxTextProvider(ITextProviderOptions<ResxTextProvider<T>> textProviderOptions)
        {
            InvariantCulture = textProviderOptions.InvariantCulture;
            _resourceManager = new ResourceManager(typeof(T));
        }

        public CultureInfo InvariantCulture { get; }

        public Task<IList<CultureInfo>> GetAvailableCulturesAsync(CancellationToken token = default)
        {
            if (_availableCulturesTcs?.Task != null &&
                !_availableCulturesTcs.Task.IsCanceled && 
                !_availableCulturesTcs.Task.IsCompleted &&
                !_availableCulturesTcs.Task.IsFaulted)
                return _availableCulturesTcs.Task;

            _availableCulturesTcs = new TaskCompletionSource<IList<CultureInfo>>();

            Task.Run(() =>
            {
                using (token.Register(() => _availableCulturesTcs.TrySetCanceled()))
                {
                    var availableCultures = new List<CultureInfo>();
                    var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                    foreach (var culture in allCultures)
                    {
                        try
                        {
                            var rs = _resourceManager.GetResourceSet(culture, true, false);
                            if (rs != null)
                                availableCultures.Add(culture.Name == CultureInfo.InvariantCulture.Name && InvariantCulture != null ? InvariantCulture : culture);
                        }
                        catch (CultureNotFoundException e)
                        {

                        }
                    }

                    _resourceManager.ReleaseAllResources();

                    _availableCulturesTcs.TrySetResult(availableCultures);
                }
            }, token);
            

            return _availableCulturesTcs.Task;
        }

        public Task<IDictionary<string, string>> GetTextResourcesAsync(CultureInfo cultureInfo, CancellationToken token = default)
        {
            if (_textResourcesTcs?.Task != null &&
                !_textResourcesTcs.Task.IsCanceled &&
                !_textResourcesTcs.Task.IsCompleted &&
                !_textResourcesTcs.Task.IsFaulted)
                return _textResourcesTcs.Task;

            _textResourcesTcs = new TaskCompletionSource<IDictionary<string, string>>();

            Task.Run(() =>
            {
                using (token.Register(() => _textResourcesTcs.TrySetCanceled()))
                {
                    var localizations = _resourceManager.GetResourceSet(cultureInfo, true, false)
                        ?.Cast<DictionaryEntry>()
                        .ToDictionary(e => e.Key.ToString(), e => e.Value.ToString());

                    _textResourcesTcs.TrySetResult(localizations ?? new Dictionary<string, string>());
                }
            }, token);
            
            return _textResourcesTcs.Task;
        }
    }
}
