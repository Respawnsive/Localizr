using System.Globalization;

namespace Localizr
{
    public interface ILocalizrOptionsBase
    {
        bool AutoInitialize { get; }
        bool TryParents { get; }
        bool RefreshAvailableCultures { get; }
        CultureInfo InitializationCulture { get; }
        CultureInfo DefaultInvariantCulture { get; }
    }
}