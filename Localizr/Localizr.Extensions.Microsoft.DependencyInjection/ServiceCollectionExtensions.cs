using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Localizr
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalizr<TTextProvider>(this IServiceCollection services,
            Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider => AddLocalizr(services, typeof(TTextProvider),
            typeof(LocalizrManager), optionsBuilder);

        public static IServiceCollection AddLocalizr<TTextProvider, TLocalizrManager>(this IServiceCollection services,
            Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrManager : class, ILocalizrManager => AddLocalizr(services, typeof(TTextProvider),
            typeof(TLocalizrManager), optionsBuilder);

        public static IServiceCollection AddLocalizr(this IServiceCollection services, Type textProviderType,
            Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null) => AddLocalizr(services, textProviderType,
            typeof(LocalizrManager), optionsBuilder);

        public static IServiceCollection AddLocalizr(this IServiceCollection services, Type textProviderType, Type localizrManagerType, Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
        {
            if (!typeof(ITextProvider).IsAssignableFrom(textProviderType))
                throw new ArgumentException($"Your text provider class must inherit from ITextProvider interface or derived");

            if (!typeof(ILocalizrManager).IsAssignableFrom(localizrManagerType))
                throw new ArgumentException($"Your localizr manager class must inherit from ILocalizrManager interface or derived");

            var localizrOptions = CreateLocalizrExtendedOptions(textProviderType, localizrManagerType, optionsBuilder);

            foreach (var optionsTextProviderType in localizrOptions.TextProviderTypes)
            {
                services.AddSingleton(typeof(ITextProviderOptions<>).MakeGenericType(optionsTextProviderType.Key),
                    TextProviderOptions.For(optionsTextProviderType.Key,
                        optionsTextProviderType.Value ?? localizrOptions.DefaultInvariantCulture));

                services.AddSingleton(typeof(ITextProvider), optionsTextProviderType.Key);
            }

            services.AddSingleton(typeof(ILocalizrManager), localizrOptions.LocalizrManagerType);

            if (localizrOptions.AutoInitialize)
            {
                var intermediateServiceProvider = services.BuildServiceProvider();
                var localizrManager = intermediateServiceProvider.GetRequiredService<ILocalizrManager>();
                Task.Run(async () => await localizrManager.InitializeAsync(localizrOptions.InitializationCulture,
                    localizrOptions.TryParents, localizrOptions.RefreshAvailableCultures).ConfigureAwait(false));
            }

            return services;
        }

        private static ILocalizrExtendedOptions CreateLocalizrExtendedOptions(Type textProviderType, Type localizrManagerType, Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
        {
            var builder = new LocalizrExtendedOptionsBuilder(new LocalizrExtendedOptions(textProviderType, localizrManagerType));

            optionsBuilder?.Invoke(builder);

            return builder.LocalizrOptions;
        }
    }
}
