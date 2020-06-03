using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Localizr;
using Microsoft.Extensions.DependencyInjection;
using Shiny.Infrastructure;

namespace Shiny
{
    public class ShinyLocalizrIntegrationAttribute : ServiceModuleAttribute
    {
        public ShinyLocalizrIntegrationAttribute(Type textProviderType, bool autoInitialize = true, bool tryParents = true, bool refreshAvailableCultures = true, string? initializationCultureName = null, string? defaultInvariantCultureName = null) :
            this(textProviderType, typeof(LocalizrInitializationHandler), typeof(LocalizrManager), autoInitialize, tryParents, refreshAvailableCultures, initializationCultureName, defaultInvariantCultureName)
        {
        }
        public ShinyLocalizrIntegrationAttribute(Type textProviderType, Type initializationHandlerType, bool autoInitialize = true, bool tryParents = true, bool refreshAvailableCultures = true, string? initializationCultureName = null, string? defaultInvariantCultureName = null) :
            this(textProviderType, initializationHandlerType, typeof(LocalizrManager), autoInitialize, tryParents, refreshAvailableCultures, initializationCultureName, defaultInvariantCultureName)
        {
        }

        public ShinyLocalizrIntegrationAttribute(Type textProviderType, Type initializationHandlerType, Type localizationManagerType, bool autoInitialize = true, bool tryParents = true, bool refreshAvailableCultures = true, string? initializationCultureName = null, string? defaultInvariantCultureName = null)
        {
            TextProviderType = textProviderType;
            InitializationHandlerType = initializationHandlerType;
            LocalizationManagerType = localizationManagerType;
            OptionsBuilder = optionsAction =>
            {
                optionsAction
                    .WithDefaultInvariantCulture(!defaultInvariantCultureName.IsEmpty()
                        ? CultureInfo.CreateSpecificCulture(defaultInvariantCultureName)
                        : CultureInfo.InvariantCulture);

                if (autoInitialize)
                    optionsAction.WithAutoInitialization(tryParents, refreshAvailableCultures,
                        !initializationCultureName.IsEmpty()
                            ? CultureInfo.CreateSpecificCulture(initializationCultureName)
                            : null);
            };
        }

        public Type TextProviderType { get; }
        public Type InitializationHandlerType { get; }
        public Type LocalizationManagerType { get; }
        public Action<ILocalizrExtendedOptionsBuilder>? OptionsBuilder { get; }

        public override void Register(IServiceCollection services)
            => services.UseLocalizr(TextProviderType, InitializationHandlerType, LocalizationManagerType, OptionsBuilder);
    }
}
