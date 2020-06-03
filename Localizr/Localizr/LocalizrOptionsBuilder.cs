using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public class LocalizrOptionsBuilder : LocalizrOptionsBuilderBase<LocalizrOptions>, ILocalizrOptionsBuilder
    {
        private readonly IList<Type> _textProviderTypes;

        public LocalizrOptionsBuilder(LocalizrOptions localizrOptions, Type mainTextProviderType) : base(localizrOptions)
        {
            _textProviderTypes = new List<Type>{mainTextProviderType};
        }

        /// <summary>
        /// Specify the default culture used as invariant for all text providers
        /// </summary>
        /// <param name="defaultInvariantCulture">Culture used as invariant (default: null = InvariantCulture)</param>
        /// <returns></returns>
        public ILocalizrOptionsBuilder WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture)
        {
            Options.DefaultInvariantCulture = defaultInvariantCulture;

            return this;
        }

        /// <summary>
        /// Adjust auto initialization settings
        /// </summary>
        /// <param name="tryParents">Try with parent culture up to invariant when the asked one can't be found (default: true)</param>
        /// <param name="refreshAvailableCultures">Refresh AvailableCultures property during initialization (default: true)</param>
        /// <param name="initializationCulture">Culture used for auto initialization</param>
        /// <returns></returns>
        public virtual ILocalizrOptionsBuilder WithAutoInitialization(bool tryParents = true,
            bool refreshAvailableCultures = true, CultureInfo? initializationCulture = null)
        {
            Options.AutoInitialize = true;
            Options.TryParents = tryParents;
            Options.RefreshAvailableCultures = refreshAvailableCultures;
            Options.InitializationCulture = initializationCulture;

            return this;
        }

        /// <summary>
        /// Add some extra resx text providers
        /// </summary>
        /// <typeparam name="TResxTextProvider">Type of resx text provider</typeparam>
        /// <param name="invariantCulture"></param>
        /// <returns></returns>
        public virtual ILocalizrOptionsBuilder AddTextProvider<TResxTextProvider>(CultureInfo? invariantCulture = null)
            where TResxTextProvider : class, IResxTextProvider
        {
            var textProviderType = typeof(TResxTextProvider);
            if (_textProviderTypes.Contains(textProviderType))
                throw new ArgumentException($"{nameof(TResxTextProvider)} added already");

            var textProviderFactory = new Func<ITextProviderOptions, TResxTextProvider>(textProviderOptions =>
            {
                if (invariantCulture != null)
                    textProviderOptions.InvariantCulture = invariantCulture;

                return (TResxTextProvider) Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions);
            });
            Options.TextProvidersFactories.Add(textProviderFactory);

            _textProviderTypes.Add(textProviderType);

            return this;
        }

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <typeparam name="TTextProvider">Type of text provider</typeparam>
        /// <param name="textProviderFactory"></param>
        /// <returns></returns>
        public virtual ILocalizrOptionsBuilder AddTextProvider<TTextProvider>(Func<ITextProviderOptions, TTextProvider> textProviderFactory)
            where TTextProvider : class, ITextProvider
        {
            var textProviderType = typeof(TTextProvider);
            if(_textProviderTypes.Contains(textProviderType))
                throw new ArgumentException($"{nameof(TTextProvider)} added already");

            Options.TextProvidersFactories.Add(textProviderFactory);

            _textProviderTypes.Add(textProviderType);

            return this;
        }
    }
}
