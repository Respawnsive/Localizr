using System;
using Localizr;
using Microsoft.Extensions.DependencyInjection;

namespace Shiny
{
    public static class ServiceCollectionExtensions
    {
        public static bool UseLocalizr<TTextProvider>(this IServiceCollection services,
            Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider => UseLocalizr(services, typeof(TTextProvider),
            typeof(LocalizrInitializationHandler),
            typeof(LocalizrManager), optionsBuilder);

        public static bool UseLocalizr<TTextProvider, TLocalizrInitializationHandler>(this IServiceCollection services,
            Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrInitializationHandler : class, ILocalizrInitializationHandler => UseLocalizr(services, typeof(TTextProvider),
            typeof(TLocalizrInitializationHandler), typeof(LocalizrManager), optionsBuilder);

        public static bool UseLocalizr<TTextProvider, TLocalizrInitializationHandler, TLocalizrManager>(
            this IServiceCollection services,
            Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrInitializationHandler : class, ILocalizrInitializationHandler
            where TLocalizrManager : class, ILocalizrManager => UseLocalizr(services, typeof(TTextProvider),
            typeof(TLocalizrInitializationHandler),
            typeof(TLocalizrManager), optionsBuilder);

        public static bool UseLocalizr(this IServiceCollection services, Type textProviderType,
            Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null) => UseLocalizr(services, textProviderType,
            typeof(LocalizrInitializationHandler),
            typeof(LocalizrManager), optionsBuilder);

        public static bool UseLocalizr(this IServiceCollection services, Type textProviderType,
            Type initializationHandlerType,
            Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null) => UseLocalizr(services, textProviderType,
            initializationHandlerType,
            typeof(LocalizrManager), optionsBuilder);

        public static bool UseLocalizr(this IServiceCollection services, Type textProviderType, Type initializationHandlerType, Type localizrManagerType, Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
        {
            services.RegisterModule(new LocalizrModule(textProviderType, initializationHandlerType, localizrManagerType, optionsBuilder));

            return true;
        }
    }
}
