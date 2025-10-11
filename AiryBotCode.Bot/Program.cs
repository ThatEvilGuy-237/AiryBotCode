using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Bot.Interfaces;
using AiryBotCode.Bot.Registers;
using System;
using System.IO;
using System.Threading.Tasks;
using AiryBotCode.Bot.Bots;

namespace AiryBotCode
{
    internal class Program
    {
        private static void Main(string[] args) =>
            MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            // Register services and build the service provider
            var serviceProvider = ServiceRegistration.BuildServiceProvider();

            //// Write a startup log entry to file
            //File.AppendAllText("/app/logs/bot-log.txt", $"[{DateTime.Now}] App started...\n");

            try
            {
                // Resolve and start the bot
                var bot = serviceProvider.GetRequiredService<AiryDevBot>();
                await bot.StartAsync(serviceProvider);

                Console.WriteLine("[DISCORD] Connecting to Discord...");

                // Keep the app alive forever (Docker will stop it externally)
                await Task.Delay(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                //// Also log to file for diagnosis
                //File.AppendAllText("/app/logs/bot-log.txt", $"[{DateTime.Now}] ERROR: {ex.Message}\n");

                Environment.Exit(-1); // Exit with error code
            }
        }
    }
}
