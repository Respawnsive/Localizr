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
            textProviders => new LocalizrManager(textProviders),
            optionsBuilder);

        public static LocalizrManager For<TTextProvider>(Func<ITextProviderOptions, TTextProvider> textProviderFactory,
            Action<ILocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider => For(
            textProviderFactory,
            textProviders => new LocalizrManager(textProviders),
            optionsBuilder);

        public static TLocalizrManager For<TResxTextProvider, TLocalizrManager>(Func<IEnumerable<ITextProvider>, TLocalizrManager> localizrManagerFactory, Action<ILocalizrOptionsBuilder>? optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider
            where TLocalizrManager : class, ILocalizrManager => For(
            textProviderOptions => (TResxTextProvider)Activator.CreateInstance(typeof(TResxTextProvider), textProviderOptions),
            localizrManagerFactory,
            optionsBuilder);

        public static TLocalizrManager For<TTextProvider, TLocalizrManager>(Func<ITextProviderOptions, TTextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, TLocalizrManager> localizrManagerFactory, Action<ILocalizrOptionsBuilder>? optionsBuilder = null) 
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var localizrOptions = CreateLocalizrOptions(textProviderFactory, localizrManagerFactory, optionsBuilder);
            var textProviders = localizrOptions.TextProvidersFactories.Select(factory => factory(TextProviderOptions.For(factory.Method.ReturnType, localizrOptions.DefaultInvariantCulture))).ToList();
            var localizrManager = localizrOptions.LocalizrManagerFactory(textProviders);

            if (localizrOptions.AutoInitialize)
                Task.Run(async () => await localizrManager.InitializeAsync(localizrOptions.InitializationCulture,
                    localizrOptions.TryParents, localizrOptions.RefreshAvailableCultures).ConfigureAwait(false));

            return (TLocalizrManager)localizrManager;
        }

        private static ILocalizrOptions CreateLocalizrOptions<TTextProvider, TLocalizrManager>(Func<ITextProviderOptions, TTextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, TLocalizrManager> localizrManagerFactory, Action<ILocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var builder = new LocalizrOptionsBuilder(new LocalizrOptions(textProviderFactory, localizrManagerFactory), typeof(TTextProvider));

            optionsBuilder?.Invoke(builder);

            return builder.LocalizrOptions;
        }
    }
}
