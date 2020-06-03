using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public class LocalizrExtendedOptionsBuilder : LocalizrOptionsBuilderBase<LocalizrExtendedOptions>, ILocalizrExtendedOptionsBuilder
    {
        internal LocalizrExtendedOptionsBuilder(LocalizrExtendedOptions localizrOptions) : base(localizrOptions)
        {
        }

        public new ILocalizrExtendedOptions LocalizrOptions => Options;

        public ILocalizrExtendedOptionsBuilder WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture)
        {
            Options.DefaultInvariantCulture = defaultInvariantCulture;

            return this;
        }

        public ILocalizrExtendedOptionsBuilder WithAutoInitialization<TLocalizrInitializationHandler>(
            bool tryParents = true,
            bool refreshAvailableCultures = true, CultureInfo? initializationCulture = null)
            where TLocalizrInitializationHandler : ILocalizrInitializationHandler => WithAutoInitialization(tryParents,
            refreshAvailableCultures, initializationCulture, typeof(TLocalizrInitializationHandler));

        public ILocalizrExtendedOptionsBuilder WithAutoInitialization(bool tryParents = true, bool refreshAvailableCultures = true,
            CultureInfo? initializationCulture = null, Type? initializationHandlerType = null)
        {
            if (initializationHandlerType != null && !typeof(ILocalizrInitializationHandler).IsAssignableFrom(initializationHandlerType))
                throw new ArgumentException($"Your initialization handler class must inherit from {nameof(ILocalizrInitializationHandler)} interface or derived");

            Options.InitializationHandlerType = initializationHandlerType;
            Options.AutoInitialize = true;
            Options.TryParents = tryParents;
            Options.RefreshAvailableCultures = refreshAvailableCultures;
            Options.InitializationCulture = initializationCulture;

            return this;
        }

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <typeparam name="TTextProvider">Type of extra text provider</typeparam>
        /// <param name="invariantCulture">Culture used as invariant for this text provider (default: null = InvariantCulture)</param>
        /// <returns></returns>
        public virtual ILocalizrExtendedOptionsBuilder AddTextProvider<TTextProvider>(CultureInfo? invariantCulture = null)
            where TTextProvider : class, ITextProvider =>
            AddTextProvider(typeof(TTextProvider), invariantCulture);

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <param name="textProviderType">Type of extra text provider</param>
        /// <param name="invariantCulture">Culture used as invariant for this text provider (default: null = InvariantCulture)</param>
        /// <returns></returns>
        public virtual ILocalizrExtendedOptionsBuilder AddTextProvider(Type textProviderType, CultureInfo? invariantCulture = null)
        {
            if (!typeof(ITextProvider).IsAssignableFrom(textProviderType))
                throw new ArgumentException($"Your text provider class must inherit from {nameof(ITextProvider)} interface or derived");

            Options.TextProviderTypes.Add(textProviderType, invariantCulture);

            return this;
        }
    }
}
