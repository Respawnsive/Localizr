using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Localizr.Resx;

namespace Localizr
{
    public static class Localizr
    {
        public static LocalizrManager For<TResxTextProvider>(Action<ILocalizrOptionsBuilder>? optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider => For(
            textProviderOptions => (TResxTextProvider) Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions),
            (textProviders, initializationHandler) => new LocalizrManager(textProviders, initializationHandler),
            optionsBuilder);

        public static LocalizrManager For<TTextProvider>(Func<ITextProviderOptions, TTextProvider> textProviderFactory,
            Action<ILocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider => For(
            textProviderFactory,
            (textProviders, initializationHandler) => new LocalizrManager(textProviders, initializationHandler),
            optionsBuilder);

        public static TLocalizrManager For<TResxTextProvider, TLocalizrManager>(Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory, Action<ILocalizrOptionsBuilder>? optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider
            where TLocalizrManager : class, ILocalizrManager => For(
            textProviderOptions => (TResxTextProvider)Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions),
            localizrManagerFactory,
            optionsBuilder);

        public static TLocalizrManager For<TTextProvider, TLocalizrManager>(Func<ITextProviderOptions, TTextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory, Action<ILocalizrOptionsBuilder>? optionsBuilder = null) 
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var localizrOptions = CreateLocalizrOptions(textProviderFactory, localizrManagerFactory, optionsBuilder);
            var textProviders = localizrOptions.TextProvidersFactories.Select(factory => factory(TextProviderOptions.For(factory.Method.ReturnType, localizrOptions.DefaultInvariantCulture))).ToList();
            var initializationHandler = localizrOptions.InitializationHandlerFactory?.Invoke(localizrOptions) ?? new LocalizrInitializationHandler(localizrOptions);
            var localizrManager = localizrOptions.LocalizrManagerFactory(textProviders, initializationHandler);

            if (localizrOptions.AutoInitialize)
                localizrManager.InitializeAsync(localizrOptions.InitializationCulture, localizrOptions.TryParents, localizrOptions.RefreshAvailableCultures);

            return (TLocalizrManager)localizrManager;
        }

        private static ILocalizrOptions CreateLocalizrOptions<TTextProvider, TLocalizrManager>(Func<ITextProviderOptions, TTextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, ILocalizrInitializationHandler, TLocalizrManager> localizrManagerFactory, Action<ILocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var builder = new LocalizrOptionsBuilder(new LocalizrOptions(textProviderFactory, localizrManagerFactory), typeof(TTextProvider));

            optionsBuilder?.Invoke(builder);

            return builder.LocalizrOptions;
        }
    }
}
