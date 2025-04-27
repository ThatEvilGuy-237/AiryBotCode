using AiryBotCode.Bot.Bots;
using AiryBotCode.Bot.Interfaces;
using AiryBotCode.Infrastructure.Registers;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AiryBotCode.Bot.Registers
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services = Register.RegisterServices(services);
            // Register required services
            services.AddSingleton<DiscordSocketClient>();
            services.AddScoped<IBot, AiryDevBot>();
            return services;
        }

        public static IServiceProvider BuildServiceProvider()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            return new ServiceCollection()
                .AddLogging()
                .AddSingleton<IConfiguration>(configuration)
                .RegisterServices()
                .BuildServiceProvider();
        }
    }
}
