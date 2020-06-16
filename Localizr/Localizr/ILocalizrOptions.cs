using System;
using System.Collections.Generic;

namespace Localizr
{
    public interface ILocalizrOptions : ILocalizrOptionsBase
    {
        IList<Func<ITextProviderOptions, ITextProvider>> TextProvidersFactories { get; }
        Func<ILocalizrOptions, ILocalizrInitializationHandler> InitializationHandlerFactory { get; }
        Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, ILocalizrManager> LocalizrManagerFactory { get; }
    }
}