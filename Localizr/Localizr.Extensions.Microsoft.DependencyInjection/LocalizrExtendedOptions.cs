using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public class LocalizrExtendedOptions : LocalizrOptions, ILocalizrExtendedOptions
    {
        public LocalizrExtendedOptions(Type textProviderType, Type localizrManagerType) : base(null, null)
        {
            TextProviderTypes = new Dictionary<Type, CultureInfo?> { { textProviderType, null } };
            LocalizrManagerType = localizrManagerType;
        }

        public IDictionary<Type, CultureInfo?> TextProviderTypes { get; }
        public Type LocalizrManagerType { get; }
    }
}
