using System;
using System.Collections.Generic;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public static class Localizr
    {
        /// <summary>
        /// Create a <see cref="LocalizrManager"/> instance for an Resx main text provider
        /// </summary>
        /// <typeparam name="TResxTextProvider">Your Resx main text provider</typeparam>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>An instance of <see cref="LocalizrManager"/></returns>
        public static LocalizrManager For<TResxTextProvider>(Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider => For(
            textProviderOptions =>
                (TResxTextProvider) Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions),
            (textProviders, initializationHandler) => new LocalizrManager(textProviders, initializationHandler),
            optionsBuilder);

        /// <summary>
        /// Create a custom <see cref="ILocalizrManager"/> implementation instance for an Resx main text provider
        /// </summary>
        /// <typeparam name="TResxTextProvider">Your Resx main text provider</typeparam>
        /// <typeparam name="TLocalizrManager">Your <see cref="ILocalizrManager"/> implementation class</typeparam>
        /// <param name="localizrManagerFactory">Your <see cref="ILocalizrManager"/> implementation class factory</param>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>An instance of <see cref="TLocalizrManager"/></returns>
        public static TLocalizrManager For<TResxTextProvider, TLocalizrManager>(
            Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider
            where TLocalizrManager : class, ILocalizrManager => For(
            textProviderOptions =>
                (TResxTextProvider) Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions),
            localizrManagerFactory,
            optionsBuilder);

        /// <summary>
        /// Create a <see cref="LocalizrManager"/> instance for a custom main text provider with a custom <see cref="ILocalizrInitializationHandler"/> implementation
        /// </summary>
        /// <typeparam name="TTextProvider">Your custom main text provider class</typeparam>
        /// <param name="textProviderFactory">Your custom main text provider class factory</param>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>An instance of <see cref="LocalizrManager"/></returns>
        public static LocalizrManager For<TTextProvider>(
            Func<ITextProviderOptions, TTextProvider> textProviderFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider => For(
            textProviderFactory,
            (textProviders, initializationHandler) => new LocalizrManager(textProviders, initializationHandler),
            optionsBuilder);


        /// <summary>
        /// Create a custom <see cref="ILocalizrManager"/> implementation class instance for a custom main text provider with a custom <see cref="ILocalizrInitializationHandler"/> implementation
        /// </summary>
        /// <typeparam name="TTextProvider">Your custom main text provider class</typeparam>
        /// <typeparam name="TLocalizrManager">Your <see cref="ILocalizrManager"/> implementation class</typeparam>
        /// <param name="textProviderFactory">Your custom main text provider class factory</param>
        /// <param name="localizrManagerFactory">Your <see cref="ILocalizrManager"/> implementation class factory</param>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>An instance of <see cref="TLocalizrManager"/></returns>
        public static TLocalizrManager For<TTextProvider, TLocalizrManager>(
            Func<ITextProviderOptions, TTextProvider> textProviderFactory,
            Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var localizrOptions = CreateLocalizrOptions(textProviderFactory,
                localizrManagerFactory, optionsBuilder);
            var textProviders = localizrOptions.TextProvidersFactories.Select(factory =>
                    factory(TextProviderOptions.For(factory.Method.ReturnType,
                        localizrOptions.DefaultInvariantCulture)))
                .ToList();
            var initializationHandler = localizrOptions.InitializationHandlerFactory.Invoke(localizrOptions);
            var localizrManager = localizrOptions.LocalizrManagerFactory(textProviders, initializationHandler);

            if (localizrOptions.AutoInitialize)
                localizrManager.InitializeAsync(localizrOptions.InitializationCulture, localizrOptions.TryParents,
                    localizrOptions.RefreshAvailableCultures);

            return (TLocalizrManager) localizrManager;
        }

        private static ILocalizrOptions
            CreateLocalizrOptions<TTextProvider, TLocalizrManager>(
                Func<ITextProviderOptions, TTextProvider> textProviderFactory,
                Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory,
                Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var builder = new LocalizrOptionsBuilder(
                new LocalizrOptions(textProviderFactory,
                    localizrManagerFactory),
                typeof(TTextProvider));

            optionsBuilder?.Invoke(builder);

            return builder.LocalizrOptions;
        }
    }
}
