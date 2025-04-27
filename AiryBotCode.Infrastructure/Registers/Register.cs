using AiryBotCode.;
using AiryBotCode.Events.ButtonPress;
using AiryBotCode.Events.Forms;
using AiryBotCode.Events.JoinServer;
using AiryBotCode.Events.SendMessage;
using AiryBotCode.Events.SlashCommands;
using AiryBotCode.Infrastructure.Configuration;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Registers
{
    public static class Register
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services = Application.Register.RegisterServices(services);
            services.AddScoped<IConfigurationReader, ConfigurationReader>();
            //services.AddSingleton<CommandService>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<MessageSendHandler>();
            // register comands
            services = MessageSendRegistrator.RegisterServices(services);
            services.AddSingleton<SlashCommandHandler>();
            services = JoinServerRegistrator.RegisterServices(services);
            services = ButtonPressRegistrator.RegisterServices(services);
            services = FormRegistrator.RegisterServices(services);
            // Register required services
            services.AddScoped<IConfigurationReader, ConfigurationReader>();

            return services;
        }
    }
}
