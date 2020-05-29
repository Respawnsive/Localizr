using System;
using System.Globalization;
using System.Threading.Tasks;
using Localizr.Resx;
using Localizr.Sample.Resources;

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
            System.Console.WriteLine("Initializing...");

            var localizr = Localizr.For<ResxTextProvider<AppResources>>(builder =>
                builder.WithDefaultInvariantCulture(CultureInfo.CreateSpecificCulture("en-US")));

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
        }
    }
}
