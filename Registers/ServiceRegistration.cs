using AiryBotCode.AiryBot;
using AiryBotCode.Events;
using AiryBotCode.Interfaces;
using AiryBotCode.Registers;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AiryBotCode.Register
{
    internal static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register required services
            services.AddScoped<IConfigurationReader, ConfigurationReader>();
            services.AddSingleton<DiscordSocketClient>(); // Ensure the client is registered as Singleton
            services.AddSingleton<CommandService>();        // Ensure the command service is registered
            services.AddSingleton<MessageSendHandler>();        // Ensure the command service is registered

            // Register other services if necessary
            services.AddScoped<IBot, Bot>();

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
