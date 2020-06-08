using System;
using System.Globalization;
using Localizr.Resx;

namespace Localizr
{
    public interface ILocalizrOptionsBuilder<out TLocalizrOptions> where TLocalizrOptions : class, ILocalizrOptions
    {
        ILocalizrOptions LocalizrOptions { get; }
    }

    public interface ILocalizrOptionsBuilder : ILocalizrOptionsBuilder<LocalizrOptions>
    {
        /// <summary>
        /// Specify the default culture used as invariant for all text providers
        /// </summary>
        /// <param name="defaultInvariantCulture">Culture used as invariant (default: null = InvariantCulture)</param>
        /// <returns>An <see cref="ILocalizrOptionsBuilder"/> implementation instance</returns>
        ILocalizrOptionsBuilder WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture);

        /// <summary>
        /// Adjust auto initialization settings
        /// </summary>
        /// <param name="tryParents">Try with parent culture up to invariant when the asked one can't be found (default: true)</param>
        /// <param name="refreshAvailableCultures">Refresh AvailableCultures property during initialization (default: true)</param>
        /// <param name="initializationCulture">Culture used for auto initialization</param>
        /// <returns>An <see cref="ILocalizrOptionsBuilder"/> implementation instance</returns>
        ILocalizrOptionsBuilder WithAutoInitialization(bool tryParents = true, bool refreshAvailableCultures = true, CultureInfo initializationCulture = null);

        /// <summary>
        /// Specify a custom initialization handler
        /// </summary>
        /// <typeparam name="TInitializationHandler">Your custom <see cref="ILocalizrInitializationHandler"/> implementation class</typeparam>
        /// <param name="initializationHandlerFactory">Your custom <see cref="ILocalizrInitializationHandler"/> implementation class factory</param>
        /// <returns>An <see cref="ILocalizrOptionsBuilder"/> implementation instance</returns>
        ILocalizrOptionsBuilder WithInitializationHandler<TInitializationHandler>(
            Func<ILocalizrOptions, TInitializationHandler> initializationHandlerFactory)
            where TInitializationHandler : class, ILocalizrInitializationHandler;

        /// <summary>
        /// Add some extra resx text providers
        /// </summary>
        /// <typeparam name="TResxTextProvider">Type of resx text provider</typeparam>
        /// <param name="invariantCulture"></param>
        /// <returns>An <see cref="ILocalizrOptionsBuilder"/> implementation instance</returns>
        ILocalizrOptionsBuilder AddTextProvider<TResxTextProvider>(
            CultureInfo invariantCulture = null)
            where TResxTextProvider : class, IResxTextProvider;

        /// <summary>
        /// Add some extra text providers
        /// </summary>
        /// <typeparam name="TTextProvider">Type of text provider</typeparam>
        /// <param name="textProviderFactory"></param>
        /// <returns>An <see cref="ILocalizrOptionsBuilder"/> implementation instance</returns>
        ILocalizrOptionsBuilder AddTextProvider<TTextProvider>(
            Func<ITextProviderOptions, TTextProvider> textProviderFactory)
            where TTextProvider : class, ITextProvider;
    }
}
