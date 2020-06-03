using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Localizr.Resx;

namespace Localizr
{
    public interface ILocalizrOptionsBuilder<out TLocalizrOptions> where TLocalizrOptions : class, ILocalizrOptions
    {
        public TLocalizrOptions LocalizrOptions { get; }

        ILocalizrOptionsBuilder<TLocalizrOptions> WithAutoInitialization(bool autoInitialize = true,
            bool tryParents = true, bool refreshAvailableCultures = true, CultureInfo? initializationCulture = null);

        ILocalizrOptionsBuilder<TLocalizrOptions> WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture);
    }

    public interface ILocalizrOptionsBuilder : ILocalizrOptionsBuilder<LocalizrOptions>
    {
        ILocalizrOptionsBuilder<LocalizrOptions> AddTextProvider<TResxTextProvider>(
            CultureInfo? invariantCulture = null)
            where TResxTextProvider : class, IResxTextProvider;

        ILocalizrOptionsBuilder<LocalizrOptions> AddTextProvider<TTextProvider>(
            Func<ITextProviderOptions, TTextProvider> textProviderFactory)
            where TTextProvider : class, ITextProvider;
    }
}
