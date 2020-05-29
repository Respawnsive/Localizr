using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public class LocalizrOptions : ILocalizrOptions
    {
        public LocalizrOptions(Func<ILocalizrOptions, ITextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, ILocalizrManager> localizrManagerFactory)
        {
            TextProviderFactories = new Dictionary<Func<ILocalizrOptions, ITextProvider>, CultureInfo ?>{ { textProviderFactory, null } };
            LocalizrManagerFactory = localizrManagerFactory;
        }

        public IDictionary<Func<ILocalizrOptions, ITextProvider>, CultureInfo?> TextProviderFactories { get; }

        public Func<IEnumerable<ITextProvider>, ILocalizrManager> LocalizrManagerFactory { get; internal set; }

        public bool AutoInitialize { get; internal set; } = true;

        public bool TryParents { get; internal set; } = true;

        public bool RefreshAvailableCultures { get; internal set; } = true;

        public CultureInfo? InitializationCulture { get; internal set; }

        public CultureInfo? DefaultInvariantCulture { get; internal set; }
    }
}
