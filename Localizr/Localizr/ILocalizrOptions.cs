using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public interface ILocalizrOptions
    {
        IList<Func<CultureInfo?, ITextProvider>> TextProviderFactories { get; }
        Func<IEnumerable<ITextProvider>, ILocalizrManager> LocalizrManagerFactory { get; }
        bool AutoInitialize { get; internal set; }
        bool TryParents { get; internal set; }
        bool RefreshAvailableCultures { get; internal set; }
        CultureInfo? InitializationCulture { get; internal set; }
        CultureInfo? DefaultInvariantCulture { get; internal set; }
    }
}