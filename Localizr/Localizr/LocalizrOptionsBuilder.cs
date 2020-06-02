using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public class LocalizrOptionsBuilder : LocalizrOptionsBuilderBase<LocalizrOptions>
    {
        private readonly IList<Type> _textProviderTypes;

        internal LocalizrOptionsBuilder(LocalizrOptions localizrOptions, Type mainTextProviderType) : base(localizrOptions)
        {
            _textProviderTypes = new List<Type>{mainTextProviderType};
        }

        /// <summary>
        /// Add some extra resx text providers
        /// </summary>
        /// <typeparam name="TResxTextProvider">Type of resx text provider</typeparam>
        /// <param name="invariantCulture"></param>
        /// <returns></returns>
        public virtual LocalizrOptionsBuilder AddTextProvider<TResxTextProvider>(CultureInfo? invariantCulture = null)
            where TResxTextProvider : class, IResxTextProvider
        {
            LocalizrOptions.TextProviderFactories.Add(defaultInvariantCulture => (TResxTextProvider)Activator.CreateInstance(typeof(TResxTextProvider), invariantCulture ?? defaultInvariantCulture));

            return this;
        }

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <typeparam name="TTextProvider">Type of text provider</typeparam>
        /// <param name="textProviderFactory"></param>
        /// <returns></returns>
        public virtual LocalizrOptionsBuilder AddTextProvider<TTextProvider>(Func<CultureInfo?, TTextProvider> textProviderFactory)
            where TTextProvider : class, ITextProvider
        {
            var textProviderType = typeof(TTextProvider);
            if(_textProviderTypes.Contains(textProviderType))
                throw new ArgumentException($"{nameof(TTextProvider)} added already");

            LocalizrOptions.TextProviderFactories.Add(textProviderFactory);

            _textProviderTypes.Add(textProviderType);

            return this;
        }
    }
}
