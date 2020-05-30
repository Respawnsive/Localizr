using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public interface ILocalizrOptions
    {
        IList<Func<CultureInfo?, ITextProvider>> TextProviderFactories { get; }
        Func<IEnumerable<ITextProvider>, ILocalizrManager> LocalizrManagerFactory { get; }
        bool AutoInitialize { get; }
        bool TryParents { get; } 
        bool RefreshAvailableCultures { get; }
        CultureInfo? InitializationCulture { get; }
        CultureInfo? DefaultInvariantCulture { get; }
    }
}