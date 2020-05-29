using System;
using System.Globalization;
using System.Linq;

namespace Localizr
{
    public class LocalizrOptionsBuilder
    {
        public LocalizrOptionsBuilder(LocalizrOptions localizrOptions)
        {
            LocalizrOptions = localizrOptions;
        }

        internal LocalizrOptions LocalizrOptions { get; }

        /// <summary>
        /// Adjust auto initialization settings
        /// </summary>
        /// <param name="autoInitialize">True to initialize localizr on app startup from a background job or False for on demand initialization (default: true)</param>
        /// <param name="tryParents">Try with parent culture up to invariant when the asked one can't be found (default: true)</param>
        /// <param name="refreshAvailableCultures">Refresh AvailableCultures property during initialization (default: true)</param>
        /// <param name="initializationCulture">Culture used for auto initialization</param>
        /// <returns></returns>
        public virtual LocalizrOptionsBuilder WithAutoInitialization(bool autoInitialize = true, bool tryParents = true, bool refreshAvailableCultures = true, CultureInfo? initializationCulture = null)
        {
            LocalizrOptions.AutoInitialize = autoInitialize;
            if (autoInitialize)
            {
                LocalizrOptions.TryParents = tryParents;
                LocalizrOptions.RefreshAvailableCultures = refreshAvailableCultures;
                LocalizrOptions.InitializationCulture = initializationCulture;
            }

            return this;
        }

        /// <summary>
        /// Specify the default culture used as invariant for all text providers
        /// </summary>
        /// <param name="defaultInvariantCulture">Culture used as invariant (default: null = InvariantCulture)</param>
        /// <returns></returns>
        public virtual LocalizrOptionsBuilder WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture)
        {
            LocalizrOptions.DefaultInvariantCulture = defaultInvariantCulture;

            return this;
        }

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <typeparam name="TTextProvider">Type of text provider</typeparam>
        /// <param name="textProviderFactory"></param>
        /// <param name="invariantCulture">Culture used as invariant for this text provider (default: null = InvariantCulture)</param>
        /// <returns></returns>
        public virtual LocalizrOptionsBuilder AddTextProvider<TTextProvider>(
            Func<ILocalizrOptions, TTextProvider> textProviderFactory, CultureInfo? invariantCulture = null)
            where TTextProvider : class, ITextProvider
        {
            if(LocalizrOptions.TextProviderFactories.Keys.Any(k => k.Method.ReturnType == typeof(TTextProvider)))
                throw new ArgumentException($"{nameof(TTextProvider)} already added");

            LocalizrOptions.TextProviderFactories.Add(textProviderFactory, invariantCulture);

            return this;
        }
    }
}
