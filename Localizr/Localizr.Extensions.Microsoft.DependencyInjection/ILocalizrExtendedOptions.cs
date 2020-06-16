using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public interface ILocalizrExtendedOptions : ILocalizrOptionsBase
    {
        IDictionary<Type, CultureInfo> TextProviderTypes { get; }
        Type LocalizrManagerType { get; }
        Type InitializationHandlerType { get; }
    }
}
