using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Localizr.Resx;
using Localizr.Sample.Console.Resources;
using Localizr.Sample.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Localizr.Sample.Console
{
    class Program
    {
        private static ILocalizrManager _localizrManager;
        private static IDisposable _subscription;

        static async Task Main(string[] args)
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("########################################################################");
            System.Console.WriteLine($"Welcome to Localizr sample Console !!!");
            System.Console.WriteLine("########################################################################");
            System.Console.WriteLine("");
            System.Console.WriteLine("Choose one of available configurations:");
            System.Console.WriteLine("1 - Static instance");
            System.Console.WriteLine("2 - Microsoft extensions");
            System.Console.WriteLine("Your choice : ");
            var readConfigChoice = System.Console.ReadLine();
            var configChoice = Convert.ToInt32(readConfigChoice);

            System.Console.WriteLine("");
            System.Console.WriteLine("Initializing...");

            if (configChoice == 1)
            {
                _localizrManager = Localizr.For<ResxTextProvider<ConsoleResources>>(builder =>
                        builder.AddTextProvider<ResxTextProvider<AppResources>>()
                            .WithDefaultInvariantCulture(CultureInfo.CreateSpecificCulture("en-US")));

                var success = await _localizrManager.InitializeAsync(refreshAvailableCultures: true);
                if (!success)
                {
                    System.Console.WriteLine("");
                    System.Console.WriteLine("Failed to initialize :(");
                    return;
                }

                System.Console.WriteLine("");
                System.Console.WriteLine("Initialization succeed :)");

                await OnInitializedAsync();
            }
            else
            {
                var services = new ServiceCollection();

                services.AddLocalizr<ResxTextProvider<ConsoleResources>>(builder =>
                    builder.AddTextProvider<ResxTextProvider<AppResources>>()
                        .WithDefaultInvariantCulture(CultureInfo.CreateSpecificCulture("en-US"))
                        .WithAutoInitialization());

                var container = services.BuildServiceProvider(true);
                var scope = container.CreateScope();

                _localizrManager = scope.ServiceProvider.GetRequiredService<ILocalizrManager>();
                _subscription = _localizrManager.WhenLocalizrStatusChanged().Subscribe(async status => await OnStatusChanged(status));
                var manualResetEvent = new ManualResetEvent(false);
                manualResetEvent.WaitOne();
            }
        }

        private static async Task OnStatusChanged(LocalizrState status)
        {
            if(status > LocalizrState.Initializing)
                _subscription?.Dispose();

            if (status == LocalizrState.Error)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("An error occured !");
            }
            else if (status == LocalizrState.None)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("No resource available !");
            }
            else if (status == LocalizrState.Some)
                await OnInitializedAsync();
        }

        static async Task OnInitializedAsync()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine($"Current culture : {_localizrManager.CurrentCulture?.DisplayName ?? "N/A"}");

            System.Console.WriteLine("");
            System.Console.WriteLine("Choose one of available cultures:");
            for (int i = 0; i < _localizrManager.AvailableCultures.Count; i++)
            {
                System.Console.WriteLine($"{i + 1} - {_localizrManager.AvailableCultures[i].DisplayName}");
            }
            System.Console.WriteLine("Your choice : ");
            var readChoice = System.Console.ReadLine();

            var choiceIndex = Convert.ToInt32(readChoice) - 1;
            var choosenCulture = _localizrManager.AvailableCultures[choiceIndex];
            if (choosenCulture.Name != _localizrManager.CurrentCulture?.Name)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine($"Switching to culture : {choosenCulture.DisplayName}");
                var success = await _localizrManager.InitializeAsync(choosenCulture);
                if (!success)
                {
                    System.Console.WriteLine("");
                    System.Console.WriteLine("Failed to initialize :(");
                    return;
                }
            }
            System.Console.WriteLine("");
            System.Console.WriteLine($"'key1' in {_localizrManager.CurrentCulture?.DisplayName} : {_localizrManager.GetText("Key1")}");
            System.Console.WriteLine($"'key2' in {_localizrManager.CurrentCulture?.DisplayName} : {_localizrManager.GetText("Key2")}");

            await OnInitializedAsync();
        }
    }
}
