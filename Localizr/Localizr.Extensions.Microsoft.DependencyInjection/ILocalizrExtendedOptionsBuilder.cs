using System;
using System.Globalization;

namespace Localizr
{
    public interface ILocalizrExtendedOptionsBuilder
    {
        /// <summary>
        /// Localizr options
        /// </summary>
        ILocalizrExtendedOptions LocalizrOptions { get; }

        /// <summary>
        /// Specify the default culture used as invariant for all text providers
        /// </summary>
        /// <param name="defaultInvariantCulture">Culture used as invariant (default: null = InvariantCulture)</param>
        /// <returns>An <see cref="ILocalizrExtendedOptionsBuilder"/> implementation instance</returns>
        ILocalizrExtendedOptionsBuilder WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture);

        /// <summary>
        /// Adjust auto initialization settings
        /// </summary>
        /// <param name="tryParents">Try with parent culture up to invariant when the asked one can't be found (default: true)</param>
        /// <param name="refreshAvailableCultures">Refresh AvailableCultures property during initialization (default: true)</param>
        /// <param name="initializationCulture">Culture used for auto initialization</param>
        /// <returns>An <see cref="ILocalizrExtendedOptionsBuilder"/> implementation instance</returns>
        ILocalizrExtendedOptionsBuilder WithAutoInitialization(bool tryParents = true, bool refreshAvailableCultures = true, CultureInfo initializationCulture = null);

        /// <summary>
        /// Specify a custom initialization handler
        /// </summary>
        /// <typeparam name="TInitializationHandler">Your custom <see cref="ILocalizrInitializationHandler"/> implementation class</typeparam>
        /// <returns>An <see cref="ILocalizrExtendedOptionsBuilder"/> implementation instance</returns>
        ILocalizrExtendedOptionsBuilder WithInitializationHandler<TInitializationHandler>()
            where TInitializationHandler : class, ILocalizrInitializationHandler;

        /// <summary>
        /// Specify a custom initialization handler
        /// </summary>
        /// <param name="initializationHandlerType">Your custom <see cref="ILocalizrInitializationHandler"/> implementation class type</param>
        /// <returns></returns>
        ILocalizrExtendedOptionsBuilder WithInitializationHandler(Type initializationHandlerType);

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <typeparam name="TTextProvider">Type of extra text provider</typeparam>
        /// <param name="invariantCulture">Culture used as invariant for this text provider (default: null = InvariantCulture)</param>
        /// <returns>An <see cref="ILocalizrExtendedOptionsBuilder"/> implementation instance</returns>
        ILocalizrExtendedOptionsBuilder AddTextProvider<TTextProvider>(CultureInfo invariantCulture = null)
            where TTextProvider : class, ITextProvider;

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <param name="textProviderType">Type of extra text provider</param>
        /// <param name="invariantCulture">Culture used as invariant for this text provider (default: null = InvariantCulture)</param>
        /// <returns>An <see cref="ILocalizrExtendedOptionsBuilder"/> implementation instance</returns>
        ILocalizrExtendedOptionsBuilder AddTextProvider(Type textProviderType, CultureInfo invariantCulture = null);
    }
}
