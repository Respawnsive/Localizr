using System;
using System.Collections.Generic;
using System.Globalization;
using Localizr.Resx;

namespace Localizr
{
    public class LocalizrOptionsBuilder : LocalizrOptionsBuilderBase<LocalizrOptions>, ILocalizrOptionsBuilder
    {
        private readonly IList<Type> _textProviderTypes;

        internal LocalizrOptionsBuilder(LocalizrOptions localizrOptions, Type mainTextProviderType) : base(localizrOptions)
        {
            _textProviderTypes = new List<Type>{mainTextProviderType};
        }

        public ILocalizrOptionsBuilder WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture)
        {
            Options.DefaultInvariantCulture = defaultInvariantCulture;

            return this;
        }

        public virtual ILocalizrOptionsBuilder WithAutoInitialization(bool tryParents = true,
            bool refreshAvailableCultures = true, CultureInfo initializationCulture = null)
        {
            Options.AutoInitialize = true;
            Options.TryParents = tryParents;
            Options.RefreshAvailableCultures = refreshAvailableCultures;
            Options.InitializationCulture = initializationCulture;

            return this;
        }

        public ILocalizrOptionsBuilder WithInitializationHandler<TInitializationHandler>(
            Func<ILocalizrOptions, TInitializationHandler> initializationHandlerFactory)
            where TInitializationHandler : class, ILocalizrInitializationHandler
        {
            Options.InitializationHandlerFactory = initializationHandlerFactory;

            return this;
        }

        public virtual ILocalizrOptionsBuilder AddTextProvider<TResxTextProvider>(CultureInfo invariantCulture = null)
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
