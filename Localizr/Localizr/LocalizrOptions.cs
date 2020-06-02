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

        bool ILocalizrOptions.AutoInitialize { get; set; }

        bool ILocalizrOptions.TryParents { get; set; }

        bool ILocalizrOptions.RefreshAvailableCultures { get; set; }

        CultureInfo? ILocalizrOptions.InitializationCulture { get; set; }

        CultureInfo? ILocalizrOptions.DefaultInvariantCulture { get; set; }
    }
}
