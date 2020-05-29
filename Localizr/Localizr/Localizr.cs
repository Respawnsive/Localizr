using System;
using System.Collections.Generic;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public class Localizr
    {
        public static LocalizrManager For<TTextProvider>(Action<LocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, IResxTextProvider => For(
            options => (TTextProvider) Activator.CreateInstance(typeof(TTextProvider), options),
            textProviders => new LocalizrManager(textProviders),
            optionsBuilder);

        public static LocalizrManager For<TTextProvider>(Func<ILocalizrOptions, TTextProvider> textProviderFactory,
            Action<LocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider => For(
            textProviderFactory,
            textProviders => new LocalizrManager(textProviders),
            optionsBuilder);

        public static TLocalizrManager For<TTextProvider, TLocalizrManager>(Func<ILocalizrOptions, TTextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, TLocalizrManager> localizrManagerFactory, Action<LocalizrOptionsBuilder>? optionsBuilder = null) 
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var options = CreateLocalizrOptions(textProviderFactory, localizrManagerFactory, optionsBuilder);
            var textProviders = options.TextProviderFactories.Select(x => x.Key(options)).ToList();
            var localizrManager = options.LocalizrManagerFactory(textProviders);

            return (TLocalizrManager)localizrManager;
        }

        private static LocalizrOptions CreateLocalizrOptions<TTextProvider, TLocalizrManager>(Func<ILocalizrOptions, TTextProvider> textProviderFactory, Func<IEnumerable<ITextProvider>, TLocalizrManager> localizrManagerFactory, Action<LocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager
        {
            var builder = new LocalizrOptionsBuilder(new LocalizrOptions(textProviderFactory, localizrManagerFactory));

            optionsBuilder?.Invoke(builder);

            return builder.LocalizrOptions;
        }
    }
}
