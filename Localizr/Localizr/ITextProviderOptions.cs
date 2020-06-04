using System.Globalization;

namespace Localizr
{
    public interface ITextProviderOptions
    {
        /// <summary>
        /// Culture used as invariant (default = InvariantCulture)
        /// </summary>
        CultureInfo InvariantCulture { get; set; }
    }

    public interface ITextProviderOptions<out TTextProvider> : ITextProviderOptions where TTextProvider : ITextProvider
    {
    }
}
