using AiryBotCode.AiryBot;
using AiryBotCode.Events.JoinServer;
using AiryBotCode.Events.SendMessage;
using AiryBotCode.Events.SlashCommands;
using AiryBotCode.Interfaces;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Registers
{
    internal static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register required services
            services.AddScoped<IConfigurationReader, ConfigurationReader>();
            services.AddSingleton<DiscordSocketClient>();
            //services.AddSingleton<CommandService>();
            services.AddSingleton<CommandService>();       
            services.AddSingleton<MessageSendHandler>();
            // register comands
            services = MessageSendRegistrator.RegisterServices(services);
            services = SlashCommandRegistrator.RegisterServices(services);
            services = JoinServerRegistrator.RegisterServices(services);
            // Register other services if necessary
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
