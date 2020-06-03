using System;
using System.Collections.Generic;
using System.Text;
using Localizr;
using Microsoft.Extensions.DependencyInjection;
using Shiny.Jobs;

namespace Shiny
{
    public class LocalizrModule : ShinyModule
    {
        private readonly ILocalizrExtendedOptions _localizrOptions;

        public LocalizrModule(Type textProviderType, Type initializationHandlerType, Type localizrManagerType, Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
        {
            if (!typeof(ITextProvider).IsAssignableFrom(textProviderType))
                throw new ArgumentException($"Your text provider class must inherit from {nameof(ITextProvider)} interface or derived");

            if (!typeof(ILocalizrInitializationHandler).IsAssignableFrom(initializationHandlerType))
                throw new ArgumentException($"Your initialization handler class must inherit from {nameof(ILocalizrInitializationHandler)} interface or derived");

            if (!typeof(ILocalizrManager).IsAssignableFrom(localizrManagerType))
                throw new ArgumentException($"Your localizr manager class must inherit from {nameof(ILocalizrManager)} interface or derived");

            _localizrOptions = CreateLocalizrExtendedOptions(textProviderType, localizrManagerType, initializationHandlerType, optionsBuilder);
        }

        public override void Register(IServiceCollection services)
        {
            services.AddSingleton<ILocalizrOptions>(_localizrOptions);

            foreach (var optionsTextProviderType in _localizrOptions.TextProviderTypes)
            {
                services.AddSingleton(typeof(ITextProviderOptions<>).MakeGenericType(optionsTextProviderType.Key),
                    TextProviderOptions.For(optionsTextProviderType.Key,
                        optionsTextProviderType.Value ?? _localizrOptions.DefaultInvariantCulture));

                services.AddSingleton(typeof(ITextProvider), optionsTextProviderType.Key);
            }

            services.AddSingleton(typeof(ILocalizrInitializationHandler), _localizrOptions.InitializationHandlerType);

            services.AddSingleton(typeof(ILocalizrManager), _localizrOptions.LocalizrManagerType);

            if (_localizrOptions.AutoInitialize)
            {
                var initializationJob = new JobInfo(typeof(LocalizrJob))
                {
                    Repeat = true,
                    IsSystemJob = true
                };

                services.RegisterJob(initializationJob);
            }
        }

        private static ILocalizrExtendedOptions CreateLocalizrExtendedOptions(Type textProviderType, Type initializationHandlerType, Type localizrManagerType, Action<ILocalizrExtendedOptionsBuilder>? optionsBuilder = null)
        {
            var builder = new LocalizrExtendedOptionsBuilder(new LocalizrExtendedOptions(textProviderType, initializationHandlerType, localizrManagerType));

            optionsBuilder?.Invoke(builder);

            return builder.LocalizrOptions;
        }
    }
}
