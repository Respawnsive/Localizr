using System.Globalization;

namespace Localizr
{
    public abstract class LocalizrOptionsBase : ILocalizrOptionsBase
    {

        public bool AutoInitialize { get; set; } = false;

        public bool TryParents { get; set; } = true;

        public bool RefreshAvailableCultures { get; set; } = false;

        public CultureInfo InitializationCulture { get; set; } = null;

        public CultureInfo DefaultInvariantCulture { get; set; } = null;
    }
}