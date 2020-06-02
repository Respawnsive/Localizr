using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Localizr
{
    public interface ILocalizrExtendedOptions : ILocalizrOptions
    {
        IDictionary<Type, CultureInfo?> TextProviderTypes { get; }
        Type LocalizrManagerType { get; }
    }
}
