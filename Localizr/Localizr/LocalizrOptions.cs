using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public class LocalizrOptions : ILocalizrOptions
    {
        public LocalizrOptions(Func<CultureInfo?, ITextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, ILocalizrManager> localizrManagerFactory)
        {
            TextProviderFactories = new List<Func<CultureInfo?, ITextProvider>>{ textProviderFactory };
            LocalizrManagerFactory = localizrManagerFactory;
        }

        public IList<Func<CultureInfo?, ITextProvider>> TextProviderFactories { get; }

        public Func<IEnumerable<ITextProvider>, ILocalizrManager> LocalizrManagerFactory { get; }

        bool ILocalizrOptions.AutoInitialize { get; set; }

        bool ILocalizrOptions.TryParents { get; set; }

        bool ILocalizrOptions.RefreshAvailableCultures { get; set; }

        CultureInfo? ILocalizrOptions.InitializationCulture { get; set; }

        CultureInfo? ILocalizrOptions.DefaultInvariantCulture { get; set; }
    }
}
