using System;
using System.Globalization;

namespace Localizr
{
    public class LocalizrExtendedOptionsBuilder : LocalizrOptionsBuilderBase<LocalizrExtendedOptions>, ILocalizrExtendedOptionsBuilder
    {
        internal LocalizrExtendedOptionsBuilder(LocalizrExtendedOptions localizrOptions) : base(localizrOptions)
        {
        }

        public new ILocalizrExtendedOptions LocalizrOptions => Options;

        public ILocalizrExtendedOptionsBuilder WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture)
        {
            Options.DefaultInvariantCulture = defaultInvariantCulture;

            return this;
        }

        public ILocalizrExtendedOptionsBuilder WithAutoInitialization(bool tryParents = true, bool refreshAvailableCultures = true,
            CultureInfo initializationCulture = null)
        {
            Options.AutoInitialize = true;
            Options.TryParents = tryParents;
            Options.RefreshAvailableCultures = refreshAvailableCultures;
            Options.InitializationCulture = initializationCulture;

            return this;
        }

        public ILocalizrExtendedOptionsBuilder WithInitializationHandler<TInitializationHandler>()
            where TInitializationHandler : class, ILocalizrInitializationHandler =>
            WithInitializationHandler(typeof(TInitializationHandler));

        public ILocalizrExtendedOptionsBuilder WithInitializationHandler(Type initializationHandlerType)
        {
            Options.InitializationHandlerType = initializationHandlerType;

            return this;
        }

        public virtual ILocalizrExtendedOptionsBuilder AddTextProvider<TTextProvider>(CultureInfo invariantCulture = null)
            where TTextProvider : class, ITextProvider =>
            AddTextProvider(typeof(TTextProvider), invariantCulture);

        public virtual ILocalizrExtendedOptionsBuilder AddTextProvider(Type textProviderType, CultureInfo invariantCulture = null)
        {
            if (!typeof(ITextProvider).IsAssignableFrom(textProviderType))
                throw new ArgumentException($"Your text provider class must inherit from {nameof(ITextProvider)} interface or derived");

            Options.TextProviderTypes.Add(textProviderType, invariantCulture);

            return this;
        }
    }
}
