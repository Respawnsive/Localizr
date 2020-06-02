﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Localizr.Resx;
using Localizr.Sample.Console.Resources;
using Localizr.Sample.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Localizr.Sample.Console
{
    class Program
    {
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
            var configChoiceIndex = Convert.ToInt32(readConfigChoice) - 1;

            System.Console.WriteLine("Initializing...");

            ILocalizrManager localizr;
            if (configChoiceIndex == 1)
            {
                localizr = Localizr.For<ResxTextProvider<ConsoleResources>>(builder =>
                        builder.AddTextProvider<ResxTextProvider<AppResources>>()
                            .WithDefaultInvariantCulture(CultureInfo.CreateSpecificCulture("en-US"))); 
            }
            else
            {
                var services = new ServiceCollection();

                services.AddLocalizr<ResxTextProvider<ConsoleResources>>(builder =>
                    builder.AddTextProvider<ResxTextProvider<AppResources>>()
                        .WithDefaultInvariantCulture(CultureInfo.CreateSpecificCulture("en-US")));

                var container = services.BuildServiceProvider(true);
                var scope = container.CreateScope();

                localizr = scope.ServiceProvider.GetRequiredService<ILocalizrManager>();
            }

            var success = await localizr.InitializeAsync(refreshAvailableCultures: true);
            if (!success)
            {
                System.Console.WriteLine("Failed to initialize :(");
                return;
            }

            System.Console.WriteLine($"Initialized with culture : {localizr.CurrentCulture?.DisplayName ?? "N/A"}");

            System.Console.WriteLine("Choose one of available cultures:");
            for (int i = 0; i < localizr.AvailableCultures.Count; i++)
            {
                System.Console.WriteLine($"{i+1} - {localizr.AvailableCultures[i].DisplayName}");
            }
            System.Console.WriteLine("Your choice : ");
            var readChoice = System.Console.ReadLine();

            var choiceIndex = Convert.ToInt32(readChoice) - 1;
            var choosenCulture = localizr.AvailableCultures[choiceIndex];
            if (choosenCulture.Name != localizr.CurrentCulture?.Name)
            {
                System.Console.WriteLine($"Switching to culture : {choosenCulture.DisplayName}");
                success = await localizr.InitializeAsync(choosenCulture);
                if (!success)
                {
                    System.Console.WriteLine("Failed to initialize :(");
                    return;
                }
            }
            System.Console.WriteLine($"'key1' in {localizr.CurrentCulture?.DisplayName} : {localizr.GetText("Key1")}");
            System.Console.WriteLine($"'key2' in {localizr.CurrentCulture?.DisplayName} : {localizr.GetText("Key2")}");
        }
    }
}
