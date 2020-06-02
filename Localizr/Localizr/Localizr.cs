using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public class Localizr
    {
        public static LocalizrManager For<TResxTextProvider>(Action<LocalizrOptionsBuilder>? optionsBuilder = null)
            where TResxTextProvider : class, IResxTextProvider => For(
            defaultInvariantCulture => (TResxTextProvider) Activator.CreateInstance(typeof(TResxTextProvider), defaultInvariantCulture),
            textProviders => new LocalizrManager(textProviders),
            optionsBuilder);

        public static LocalizrManager For<TTextProvider>(Func<CultureInfo?, TTextProvider> textProviderFactory,
            Action<LocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider => For(
            textProviderFactory,
            textProviders => new LocalizrManager(textProviders),
            optionsBuilder);

        public static TLocalizrManager For<TTextProvider, TLocalizrManager>(Func<IEnumerable<ITextProvider>, TLocalizrManager> localizrManagerFactory, Action<LocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, IResxTextProvider
            where TLocalizrManager : class, ILocalizrManager => For(
            defaultInvariantCulture => (TTextProvider)Activator.CreateInstance(typeof(TTextProvider), defaultInvariantCulture),
            localizrManagerFactory,
            optionsBuilder);

        public static TLocalizrManager For<TTextProvider, TLocalizrManager>(Func<CultureInfo?, TTextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, TLocalizrManager> localizrManagerFactory, Action<LocalizrOptionsBuilder>? optionsBuilder = null) 
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var options = CreateLocalizrOptions(textProviderFactory, localizrManagerFactory, optionsBuilder);
            var textProviders = options.TextProviderFactories.Select(factory => factory(options.DefaultInvariantCulture)).ToList();
            var localizrManager = options.LocalizrManagerFactory(textProviders);

            return (TLocalizrManager)localizrManager;
        }

        private static ILocalizrOptions CreateLocalizrOptions<TTextProvider, TLocalizrManager>(Func<CultureInfo?, TTextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, TLocalizrManager> localizrManagerFactory, Action<LocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var builder = new LocalizrOptionsBuilder(new LocalizrOptions(textProviderFactory, localizrManagerFactory), typeof(TTextProvider));

            optionsBuilder?.Invoke(builder);

            return builder.LocalizrOptions;
        }
    }
}
