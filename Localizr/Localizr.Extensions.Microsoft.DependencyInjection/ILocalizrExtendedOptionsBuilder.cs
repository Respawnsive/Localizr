using System;
using System.Globalization;

namespace Localizr
{
    public interface ILocalizrExtendedOptionsBuilder : ILocalizrOptionsBuilder<LocalizrExtendedOptions>
    {
        public new ILocalizrExtendedOptions LocalizrOptions { get; }

        ILocalizrExtendedOptionsBuilder WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture);

        ILocalizrExtendedOptionsBuilder WithAutoInitialization(bool tryParents = true, bool refreshAvailableCultures = true, CultureInfo? initializationCulture = null);

        ILocalizrExtendedOptionsBuilder AddTextProvider<TTextProvider>(CultureInfo? invariantCulture = null)
            where TTextProvider : class, ITextProvider;

        ILocalizrExtendedOptionsBuilder AddTextProvider(Type textProviderType, CultureInfo? invariantCulture = null);
    }
}
