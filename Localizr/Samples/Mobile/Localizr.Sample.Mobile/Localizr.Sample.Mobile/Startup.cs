using System.Globalization;
using Localizr.Resx;
using Localizr.Sample.Mobile.Resources;
using Localizr.Sample.Resources;
using Microsoft.Extensions.DependencyInjection;
using Shiny.Prism;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace Localizr.Sample.Mobile
{
    public class Startup : PrismStartup
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAppInfo, AppInfoImplementation>();

            services.AddLocalizr<ResxTextProvider<MobileResources>>(builder =>
                builder.AddTextProvider<ResxTextProvider<AppResources>>()
                    .WithDefaultInvariantCulture(CultureInfo.CreateSpecificCulture("en-US"))
                    .WithAutoInitialization());
        }
    }
}
