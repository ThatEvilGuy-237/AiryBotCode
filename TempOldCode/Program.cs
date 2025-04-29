
//using AiryBotCode.Registers;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode
{
    internal class Program
    {
        private static void Main(string[] args) =>
            MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            // Register services and build the service provider
        //    var serviceProvider = ServiceRegistration.BuildServiceProvider();

        //    try
        //    {
        //        // Resolve the IBot instance
        //        var bot = serviceProvider.GetRequiredService<IBot>();

        //        // Start the bot
        //        await bot.StartAsync(serviceProvider);

        //        Console.WriteLine("Connected to Discord");

        //        // Keep running until 'Q' is pressed
        //        while (true)
        //        {
        //            var keyInfo = Console.ReadKey(intercept: true); // Intercept the key press

        //            if (keyInfo.Key == ConsoleKey.Q)
        //            {
        //                Console.WriteLine("\nShutting down!");

        //                // Stop the bot and exit
        //                await bot.StopAsync();
        //                return;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //        Environment.Exit(-1); // Exit with error code
        //    }
        }

    }
}
