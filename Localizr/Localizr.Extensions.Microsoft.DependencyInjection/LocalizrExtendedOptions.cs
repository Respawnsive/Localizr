using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizr
{
    public class LocalizrExtendedOptions : LocalizrOptions, ILocalizrExtendedOptions
    {
        public LocalizrExtendedOptions(Type textProviderType, Type initializationHandlerType, Type localizrManagerType) : base(null, null)
        {
            TextProviderTypes = new Dictionary<Type, CultureInfo> { { textProviderType, null } };
            InitializationHandlerType = initializationHandlerType;
            LocalizrManagerType = localizrManagerType;
        }

        public IDictionary<Type, CultureInfo> TextProviderTypes { get; }
        public Type LocalizrManagerType { get; }
        public Type InitializationHandlerType { get; set; }
    }
}
