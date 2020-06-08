using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public class LocalizrOptions : ILocalizrOptions
    {
        public LocalizrOptions(Func<ITextProviderOptions, ITextProvider> textProviderFactory,
            Func<ILocalizrOptions, ILocalizrInitializationHandler> initializationHandlerFactory,
            Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, ILocalizrManager> localizrManagerFactory)
        {
            TextProvidersFactories = new List<Func<ITextProviderOptions, ITextProvider>> {textProviderFactory};
            InitializationHandlerFactory = initializationHandlerFactory;
            LocalizrManagerFactory = localizrManagerFactory;
        }

        public IList<Func<ITextProviderOptions, ITextProvider>> TextProvidersFactories { get; }

        public Func<ILocalizrOptions, ILocalizrInitializationHandler> InitializationHandlerFactory { get; set; }

        public Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, ILocalizrManager> LocalizrManagerFactory { get; set; }

        public bool AutoInitialize { get; set; } = false;

        public bool TryParents { get; set; } = true;

        public bool RefreshAvailableCultures { get; set; } = false;

        public CultureInfo InitializationCulture { get; set; } = null;

        public CultureInfo DefaultInvariantCulture { get; set; } = null;
    }
}
