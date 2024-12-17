using AiryBotCode.AiryBot;
using AiryBotCode.Register;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode
{
    internal class Program
    {
        private static void Main(string[] args) => 
            MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {

            // Register services
            ServiceRegistration.RegisterServices(new ServiceCollection());

            // Build the service provider
            var serviceProvider = ServiceRegistration.BuildServiceProvider();

            try
            {
                IBot bot = serviceProvider.GetRequiredService<IBot>();

                await bot.StartAsync(serviceProvider);

                Console.WriteLine("Connected to Discord");

                do
                {
                    var keyInfo = Console.ReadKey();

                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        Console.WriteLine("\nShutting down!");

                        await bot.StopAsync();
                        return;
                    }
                } while (true);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Environment.Exit(-1);
            }
        }
    }
}
