using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Localizr
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register a <see cref="LocalizrManager"/> singleton instance for a main text provider
        /// </summary>
        /// <typeparam name="TTextProvider">Your main text provider</typeparam>
        /// <param name="services">The service collection</param>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddLocalizr<TTextProvider>(this IServiceCollection services,
            Action<ILocalizrExtendedOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider => AddLocalizr(services, typeof(TTextProvider),
            typeof(LocalizrInitializationHandler),
            typeof(LocalizrManager), optionsBuilder);

        /// <summary>
        /// Register a <see cref="LocalizrManager"/> singleton instance for a main text provider with a custom <see cref="ILocalizrInitializationHandler"/> implementation
        /// </summary>
        /// <typeparam name="TTextProvider">Your main text provider</typeparam>
        /// <typeparam name="TLocalizrInitializationHandler">Your <see cref="ILocalizrInitializationHandler"/> implementation class</typeparam>
        /// <param name="services">The service collection</param>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddLocalizr<TTextProvider, TLocalizrInitializationHandler>(
            this IServiceCollection services,
            Action<ILocalizrExtendedOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrInitializationHandler : class, ILocalizrInitializationHandler => AddLocalizr(services,
            typeof(TTextProvider),
            typeof(TLocalizrInitializationHandler), typeof(LocalizrManager), optionsBuilder);

        /// <summary>
        /// Register a custom <see cref="ILocalizrManager"/> implementation singleton instance for a main text provider with a custom <see cref="ILocalizrInitializationHandler"/> implementation
        /// </summary>
        /// <typeparam name="TTextProvider">Your main text provider</typeparam>
        /// <typeparam name="TLocalizrInitializationHandler">Your <see cref="ILocalizrInitializationHandler"/> implementation class</typeparam>
        /// <typeparam name="TLocalizrManager">Your <see cref="ILocalizrManager"/> implementation class</typeparam>
        /// <param name="services">The service collection</param>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddLocalizr<TTextProvider, TLocalizrInitializationHandler, TLocalizrManager>(
            this IServiceCollection services,
            Action<ILocalizrExtendedOptionsBuilder> optionsBuilder = null)
            where TTextProvider : class, ITextProvider
            where TLocalizrInitializationHandler : class, ILocalizrInitializationHandler
            where TLocalizrManager : class, ILocalizrManager => AddLocalizr(services, typeof(TTextProvider),
            typeof(TLocalizrInitializationHandler),
            typeof(TLocalizrManager), optionsBuilder);

        /// <summary>
        /// Register a <see cref="LocalizrManager"/> singleton instance for a main text provider
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="textProviderType">Your main text provider</param>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddLocalizr(this IServiceCollection services, Type textProviderType,
            Action<ILocalizrExtendedOptionsBuilder> optionsBuilder = null) => AddLocalizr(services, textProviderType,
            typeof(LocalizrInitializationHandler),
            typeof(LocalizrManager), optionsBuilder);

        /// <summary>
        /// Register a <see cref="LocalizrManager"/> singleton instance for a main text provider with a custom <see cref="ILocalizrInitializationHandler"/> implementation
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="textProviderType">Your main text provider</param>
        /// <param name="initializationHandlerType">Your <see cref="ILocalizrInitializationHandler"/> implementation class</param>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddLocalizr(this IServiceCollection services, Type textProviderType,
            Type initializationHandlerType,
            Action<ILocalizrExtendedOptionsBuilder> optionsBuilder = null) => AddLocalizr(services, textProviderType,
            initializationHandlerType,
            typeof(LocalizrManager), optionsBuilder);

        /// <summary>
        /// Register a custom <see cref="ILocalizrManager"/> implementation singleton class instance for a main text provider with a custom <see cref="ILocalizrInitializationHandler"/> implementation
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="textProviderType">Your main text provider</param>
        /// <param name="initializationHandlerType">Your <see cref="ILocalizrInitializationHandler"/> implementation class</param>
        /// <param name="localizrManagerType">Your <see cref="ILocalizrManager"/> implementation class</param>
        /// <param name="optionsBuilder">Some options</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddLocalizr(this IServiceCollection services, Type textProviderType,
            Type initializationHandlerType, Type localizrManagerType,
            Action<ILocalizrExtendedOptionsBuilder> optionsBuilder = null)
        {
            if (!typeof(ITextProvider).IsAssignableFrom(textProviderType))
                throw new ArgumentException(
                    $"Your text provider class must inherit from {nameof(ITextProvider)} interface or derived");

            if (!typeof(ILocalizrInitializationHandler).IsAssignableFrom(initializationHandlerType))
                throw new ArgumentException(
                    $"Your initialization handler class must inherit from {nameof(ILocalizrInitializationHandler)} interface or derived");

            if (!typeof(ILocalizrManager).IsAssignableFrom(localizrManagerType))
                throw new ArgumentException(
                    $"Your localizr manager class must inherit from {nameof(ILocalizrManager)} interface or derived");

            var localizrOptions = CreateLocalizrExtendedOptions(textProviderType, initializationHandlerType,
                localizrManagerType, optionsBuilder);
            services.AddSingleton<ILocalizrOptions>(localizrOptions);

            foreach (var optionsTextProviderType in localizrOptions.TextProviderTypes)
            {
                services.AddSingleton(typeof(ITextProviderOptions<>).MakeGenericType(optionsTextProviderType.Key),
                    TextProviderOptions.For(optionsTextProviderType.Key,
                        optionsTextProviderType.Value ?? localizrOptions.DefaultInvariantCulture));

                services.AddSingleton(typeof(ITextProvider), optionsTextProviderType.Key);
            }

            services.AddSingleton(typeof(ILocalizrInitializationHandler), localizrOptions.InitializationHandlerType);

            services.AddSingleton(typeof(ILocalizrManager), localizrOptions.LocalizrManagerType);

            if (localizrOptions.AutoInitialize)
            {
                var intermediateServiceProvider = services.BuildServiceProvider();
                var localizrManager = intermediateServiceProvider.GetRequiredService<ILocalizrManager>();
                localizrManager.InitializeAsync(localizrOptions.InitializationCulture, localizrOptions.TryParents,
                    localizrOptions.RefreshAvailableCultures);
                services.Replace(new ServiceDescriptor(typeof(ILocalizrManager), localizrManager));
            }

            return services;
        }

        private static ILocalizrExtendedOptions CreateLocalizrExtendedOptions(Type textProviderType,
            Type initializationHandlerType, Type localizrManagerType,
            Action<ILocalizrExtendedOptionsBuilder> optionsBuilder = null)
        {
            var builder = new LocalizrExtendedOptionsBuilder(new LocalizrExtendedOptions(textProviderType,
                initializationHandlerType, localizrManagerType));

            optionsBuilder?.Invoke(builder);

            return builder.LocalizrOptions;
        }
    }
}
