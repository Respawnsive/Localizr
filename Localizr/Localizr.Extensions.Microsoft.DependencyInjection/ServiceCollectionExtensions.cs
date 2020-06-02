using System;
using Localizr.Resx;
using Microsoft.Extensions.DependencyInjection;

namespace Localizr
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalizr<TTextProvider>(this IServiceCollection services,
            Action<LocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider => AddLocalizr(services, typeof(TTextProvider),
            typeof(LocalizrManager), optionsBuilder);

        public static IServiceCollection AddLocalizr<TTextProvider, TLocalizrManager>(this IServiceCollection services,
            Action<LocalizrOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager => AddLocalizr(services, typeof(TTextProvider),
            typeof(TLocalizrManager), optionsBuilder);

        public static IServiceCollection AddLocalizr(this IServiceCollection services, Type textProviderType,
            Action<LocalizrOptionsBuilder>? optionsBuilder = null) => AddLocalizr(services, textProviderType,
            typeof(LocalizrManager), optionsBuilder);

        public static IServiceCollection AddLocalizr(this IServiceCollection services, Type textProviderType, Type localizrManagerType, Action<LocalizrOptionsBuilder>? optionsBuilder = null)
        {
            if (!typeof(ITextProvider).IsAssignableFrom(textProviderType))
                throw new ArgumentException($"Your text provider class must inherit from ITextProvider interface or derived");

            if (!typeof(ILocalizrManager).IsAssignableFrom(localizrManagerType))
                throw new ArgumentException($"Your localizr manager class must inherit from ILocalizrManager interface or derived");

            return services;
        }
    }
}
