using System;
using System.Globalization;
using Localizr.Resx;

namespace Localizr
{
    public interface ILocalizrOptionsBuilder<out TLocalizrOptions> where TLocalizrOptions : class, ILocalizrOptions
    {
        ILocalizrOptions LocalizrOptions { get; }
    }

    public interface ILocalizrOptionsBuilder : ILocalizrOptionsBuilder<LocalizrOptions>
    {
        ILocalizrOptionsBuilder WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture);

        ILocalizrOptionsBuilder WithAutoInitialization(bool tryParents = true, bool refreshAvailableCultures = true, CultureInfo initializationCulture = null);

        ILocalizrOptionsBuilder AddTextProvider<TResxTextProvider>(
            CultureInfo invariantCulture = null)
            where TResxTextProvider : class, IResxTextProvider;

        ILocalizrOptionsBuilder AddTextProvider<TTextProvider>(
            Func<ITextProviderOptions, TTextProvider> textProviderFactory)
            where TTextProvider : class, ITextProvider;
    }
}
