using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public interface ILocalizrOptions
    {
        IList<Func<ITextProviderOptions, ITextProvider>> TextProvidersFactories { get; }
        Func<ILocalizrOptions, ILocalizrInitializationHandler> InitializationHandlerFactory { get; }
        Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, ILocalizrManager> LocalizrManagerFactory { get; }
        bool AutoInitialize { get; }
        bool TryParents { get; }
        bool RefreshAvailableCultures { get; }
        CultureInfo InitializationCulture { get; }
        CultureInfo DefaultInvariantCulture { get; }
    }
}