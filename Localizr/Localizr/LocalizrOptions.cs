using System;
using System.Collections.Generic;

namespace Localizr
{
    public class LocalizrOptions : LocalizrOptionsBase, ILocalizrOptions
    {
        public LocalizrOptions(Func<ITextProviderOptions, ITextProvider> textProviderFactory,
            Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, ILocalizrManager> localizrManagerFactory)
        {
            TextProvidersFactories = new List<Func<ITextProviderOptions, ITextProvider>> {textProviderFactory};
            InitializationHandlerFactory = localizrOptions => new LocalizrInitializationHandler(localizrOptions);
            LocalizrManagerFactory = localizrManagerFactory;
        }

        public IList<Func<ITextProviderOptions, ITextProvider>> TextProvidersFactories { get; }

        public Func<ILocalizrOptions, ILocalizrInitializationHandler> InitializationHandlerFactory { get; set; }

        public Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, ILocalizrManager> LocalizrManagerFactory { get; set; }
    }
}
