using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public class LocalizrOptions : ILocalizrOptions
    {
        public LocalizrOptions(Func<ITextProviderOptions, ITextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, ILocalizrManager> localizrManagerFactory)
        {
            TextProvidersFactories = new List<Func<ITextProviderOptions, ITextProvider>> { textProviderFactory };
            LocalizrManagerFactory = localizrManagerFactory;
        }

        public IList<Func<ITextProviderOptions, ITextProvider>> TextProvidersFactories { get; }

        public Func<IEnumerable<ITextProvider>, ILocalizrManager> LocalizrManagerFactory { get; }

        bool ILocalizrOptions.AutoInitialize { get; set; } = true;

        bool ILocalizrOptions.TryParents { get; set; } = true;

        bool ILocalizrOptions.RefreshAvailableCultures { get; set; } = false;

        CultureInfo? ILocalizrOptions.InitializationCulture { get; set; } = null;

        CultureInfo? ILocalizrOptions.DefaultInvariantCulture { get; set; } = null;
    }
}
