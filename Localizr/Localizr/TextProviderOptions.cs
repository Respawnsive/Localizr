using System;
using System.Globalization;

namespace Localizr
{
    public class TextProviderOptions : ITextProviderOptions
    {
        protected TextProviderOptions(CultureInfo invariantCulture = null)
        {
            InvariantCulture = invariantCulture;
        } 

        public CultureInfo InvariantCulture { get; set; }

        public static ITextProviderOptions For(Type textProviderType, CultureInfo invariantCulture = null)
        {
            if (!typeof(ITextProvider).IsAssignableFrom(textProviderType))
                throw new ArgumentException($"Your text provider class must inherit from ITextProvider interface or derived");

            return (ITextProviderOptions) Activator.CreateInstance(
                typeof(TextProviderOptions<>).MakeGenericType(textProviderType), invariantCulture);
        }
    }

    public class TextProviderOptions<TTextProvider> : TextProviderOptions, ITextProviderOptions<TTextProvider> where TTextProvider : ITextProvider
    {
        public TextProviderOptions(CultureInfo invariantCulture = null) : base(invariantCulture)
        {
        }
    }
}
