using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public class LocalizrExtendedOptionsBuilder : LocalizrOptionsBuilderBase<LocalizrExtendedOptions>
    {
        internal LocalizrExtendedOptionsBuilder(LocalizrExtendedOptions localizrOptions) : base(localizrOptions)
        {
        }

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <typeparam name="TTextProvider">Type of extra text provider</typeparam>
        /// <param name="invariantCulture">Culture used as invariant for this text provider (default: null = InvariantCulture)</param>
        /// <returns></returns>
        public virtual LocalizrExtendedOptionsBuilder AddTextProvider<TTextProvider>(CultureInfo? invariantCulture = null)
            where TTextProvider : class, ITextProvider =>
            AddTextProvider(typeof(TTextProvider), invariantCulture);

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <param name="textProviderType">Type of extra text provider</param>
        /// <param name="invariantCulture">Culture used as invariant for this text provider (default: null = InvariantCulture)</param>
        /// <returns></returns>
        public virtual LocalizrExtendedOptionsBuilder AddTextProvider(Type textProviderType, CultureInfo? invariantCulture = null)
        {
            if (!typeof(ITextProvider).IsAssignableFrom(textProviderType))
                throw new ArgumentException($"Your text provider class must inherit from ITextProvider interface or derived");

            LocalizrOptions.TextProviderTypes.Add(textProviderType, invariantCulture);

            return this;
        }
    }
}
