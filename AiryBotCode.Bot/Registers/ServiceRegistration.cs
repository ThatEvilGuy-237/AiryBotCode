using AiryBotCode.Bot.Bots;
using AiryBotCode.Bot.Interfaces;
using AiryBotCode.Infrastructure.Registers;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AiryBotCode.Bot.Registers
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services = RegisterInfrastructure.RegisterServices(services);
            // Register required services
            services.AddScoped<IBot, AiryDevBot>();
            services = AiryDevBot.CreateClientService(services);
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
