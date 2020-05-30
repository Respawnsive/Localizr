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

        public Func<IEnumerable<ITextProvider>, ILocalizrManager> LocalizrManagerFactory { get; internal set; }

        public bool AutoInitialize { get; internal set; } = true;

        public bool TryParents { get; internal set; } = true;

        public bool RefreshAvailableCultures { get; internal set; } = true;

        public CultureInfo? InitializationCulture { get; internal set; }

        public CultureInfo? DefaultInvariantCulture { get; internal set; }
    }
}
