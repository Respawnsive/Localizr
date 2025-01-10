using Localizr.Resx;
using Localizr.Sample.Resources;
using Microsoft.Extensions.Logging;
using Shiny;
using System.Globalization;
using Localizr.Sample.Mobile.Resources.Resx;
using CommunityToolkit.Maui;
using Localizr.Sample.Mobile.ViewModels;

namespace Localizr.Sample.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseShiny()
                .UsePrism(
                    new DryIocContainerExtension(),
                    prism => prism
                        .AddGlobalNavigationObserver(ObserveGlobalNavigation)
                        .CreateWindow(
                            navigationService => navigationService
                                .CreateBuilder()
                                .AddNavigationPage()
                                .AddSegment<MainPageViewModel>()
                                .NavigateAsync(HandleNavigationError))
                )
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(AppInfo.Current);

            builder.Services.AddLocalizr<ResxTextProvider<MobileResources>>(opt =>
                opt.AddTextProvider<ResxTextProvider<AppResources>>()
                    .WithDefaultInvariantCulture(CultureInfo.CreateSpecificCulture("en-US"))
                    .WithAutoInitialization());

            builder.Services
                .RegisterForNavigation<NavigationPage>()
                .RegisterForNavigation<Views.MainPage, MainPageViewModel>();

            return builder.Build();
        }

        private static void ObserveGlobalNavigation(IContainerProvider container, IObservable<NavigationRequestContext> context)
        {
            var logger = container.Resolve<ILogger<App>>();
            context.Subscribe(x =>
            {
                var target = x.Type == NavigationRequestType.Navigate
                    ? x.Uri.ToString()
                    : x.Type.ToString();

                var status = x.Cancelled
                    ? "Cancelled"
                    : x.Result.Success
                        ? "Succeeded"
                        : "Failed";

                logger.LogTrace("Navigation to {0} {1}", target, status);

                if (status == "Failed" && !string.IsNullOrEmpty(x.Result?.Exception?.Message))
                    logger.LogError(x.Result.Exception.Message);
            });
        }

        private static void HandleNavigationError(Exception ex)
        {
            Console.WriteLine(ex);
            System.Diagnostics.Debugger.Break();
        }
    }
}
