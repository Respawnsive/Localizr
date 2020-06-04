using System;
using System.Collections.Generic;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public static class Localizr
    {
        public static LocalizrManager For<TResxTextProvider>(Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider => For(
            textProviderOptions =>
                (TResxTextProvider) Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions),
            localizrOptions => new LocalizrInitializationHandler(localizrOptions),
            (textProviders, initializationHandler) => new LocalizrManager(textProviders, initializationHandler),
            optionsBuilder);

        public static LocalizrManager For<TResxTextProvider, TLocalizrInitializationHandler>(
            Func<ILocalizrOptions, TLocalizrInitializationHandler> initializationHandlerFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider
            where TLocalizrInitializationHandler : class, ILocalizrInitializationHandler => For(
            textProviderOptions =>
                (TResxTextProvider) Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions),
            initializationHandlerFactory,
            (textProviders, initializationHandler) => new LocalizrManager(textProviders, initializationHandler),
            optionsBuilder);

        public static TLocalizrManager For<TResxTextProvider, TLocalizrManager>(
            Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider
            where TLocalizrManager : class, ILocalizrManager => For(
            textProviderOptions =>
                (TResxTextProvider) Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions),
            localizrOptions => new LocalizrInitializationHandler(localizrOptions),
            localizrManagerFactory,
            optionsBuilder);

        public static TLocalizrManager For<TResxTextProvider, TLocalizrInitializationHandler, TLocalizrManager>(
            Func<ILocalizrOptions, TLocalizrInitializationHandler> initializationHandlerFactory,
            Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider
            where TLocalizrInitializationHandler : class, ILocalizrInitializationHandler
            where TLocalizrManager : class, ILocalizrManager => For(
            textProviderOptions =>
                (TResxTextProvider) Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions),
            initializationHandlerFactory,
            localizrManagerFactory,
            optionsBuilder);

        public static LocalizrManager For<TTextProvider>(Func<ITextProviderOptions, TTextProvider> textProviderFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider => For(
            textProviderFactory,
            localizrOptions => new LocalizrInitializationHandler(localizrOptions),
            (textProviders, initializationHandler) => new LocalizrManager(textProviders, initializationHandler),
            optionsBuilder);

        public static LocalizrManager For<TTextProvider, TLocalizrInitializationHandler>(
            Func<ITextProviderOptions, TTextProvider> textProviderFactory,
            Func<ILocalizrOptions, TLocalizrInitializationHandler> initializationHandlerFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrInitializationHandler : class, ILocalizrInitializationHandler => For(
            textProviderFactory,
            initializationHandlerFactory,
            (textProviders, initializationHandler) => new LocalizrManager(textProviders, initializationHandler),
            optionsBuilder);

        public static TLocalizrManager For<TTextProvider, TLocalizrManager>(
            Func<ITextProviderOptions, TTextProvider> textProviderFactory,
            Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager => For(
            textProviderFactory,
            localizrOptions => new LocalizrInitializationHandler(localizrOptions),
            localizrManagerFactory,
            optionsBuilder);

        public static TLocalizrManager For<TTextProvider, TLocalizrInitializationHandler, TLocalizrManager>(
            Func<ITextProviderOptions, TTextProvider> textProviderFactory,
            Func<ILocalizrOptions, TLocalizrInitializationHandler> initializationHandlerFactory,
            Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory,
            Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrInitializationHandler : class, ILocalizrInitializationHandler
            where TLocalizrManager : class, ILocalizrManager
        {
            var localizrOptions = CreateLocalizrOptions(textProviderFactory, initializationHandlerFactory,
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
            CreateLocalizrOptions<TTextProvider, TLocalizrInitializationHandler, TLocalizrManager>(
                Func<ITextProviderOptions, TTextProvider> textProviderFactory,
                Func<ILocalizrOptions, TLocalizrInitializationHandler> initializationHandlerFactory,
                Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager>
                    localizrManagerFactory, Action<ILocalizrOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrInitializationHandler : class, ILocalizrInitializationHandler
            where TLocalizrManager : class, ILocalizrManager
        {
            var builder = new LocalizrOptionsBuilder(
                new LocalizrOptions(textProviderFactory, initializationHandlerFactory, localizrManagerFactory),
                typeof(TTextProvider));

            optionsBuilder?.Invoke(builder);

            return builder.LocalizrOptions;
        }
    }
}
