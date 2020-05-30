using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public class LocalizrOptionsBuilder
    {
        private readonly IList<Type> _textProviderTypes;

        internal LocalizrOptionsBuilder(LocalizrOptions localizrOptions, Type mainTextProviderType)
        {
            LocalizrOptions = localizrOptions;
            _textProviderTypes = new List<Type>{mainTextProviderType};
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
        /// Add some extra resx text providers
        /// </summary>
        /// <typeparam name="TResxTextProvider">Type of resx text provider</typeparam>
        /// <param name="invariantCulture"></param>
        /// <returns></returns>
        public virtual LocalizrOptionsBuilder AddTextProvider<TResxTextProvider>(CultureInfo? invariantCulture = null)
            where TResxTextProvider : class, IResxTextProvider
        {
            LocalizrOptions.TextProviderFactories.Add(defaultInvariantCulture => (TResxTextProvider)Activator.CreateInstance(typeof(TResxTextProvider), invariantCulture ?? defaultInvariantCulture));

            return this;
        }

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <typeparam name="TTextProvider">Type of text provider</typeparam>
        /// <param name="textProviderFactory"></param>
        /// <returns></returns>
        public virtual LocalizrOptionsBuilder AddTextProvider<TTextProvider>(Func<CultureInfo?, TTextProvider> textProviderFactory)
            where TTextProvider : class, ITextProvider
        {
            var textProviderType = typeof(TTextProvider);
            if(_textProviderTypes.Contains(textProviderType))
                throw new ArgumentException($"{nameof(TTextProvider)} added already");

            LocalizrOptions.TextProviderFactories.Add(textProviderFactory);

            _textProviderTypes.Add(textProviderType);

            return this;
        }
    }
}
