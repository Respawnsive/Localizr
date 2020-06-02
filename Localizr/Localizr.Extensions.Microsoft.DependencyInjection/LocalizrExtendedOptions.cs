using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Localizr
{
    public class LocalizrExtendedOptions : LocalizrOptions, ILocalizrExtendedOptions
    {
        public LocalizrExtendedOptions(Type textProviderType, Type localizationManagerType) : base(null, null)
        {
            TextProviderTypes = new Dictionary<Type, CultureInfo?> { { textProviderType, null } };
            LocalizationManagerType = localizationManagerType;
        }

        public IDictionary<Type, CultureInfo?> TextProviderTypes { get; }

        public Type LocalizationManagerType { get; }
    }
}
