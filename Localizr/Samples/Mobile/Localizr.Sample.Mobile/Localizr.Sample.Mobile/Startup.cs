using System;
using System.Globalization;
using Localizr.Resx;
using Localizr.Sample.Mobile.Resources;
using Localizr.Sample.Resources;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.Prism;

namespace Localizr.Sample.Mobile
{
    public class Startup : PrismStartup
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.UseLocalizr<ResxTextProvider<MobileResources>>(builder =>
                builder.AddTextProvider<ResxTextProvider<AppResources>>()
                    .WithDefaultInvariantCulture(CultureInfo.CreateSpecificCulture("en-US"))
                    .WithAutoInitialization());
        }
    }
}
