using AiryBotCode.Application;
using AiryBotCode.Events.SendMessage.MessageComands.TalkWithAiry;
using AiryBotCode.Infrastructure.Activitys.SlashEvents;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Events;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;


namespace AiryBotCode.Infrastructure.Registers
{
    public static class RegisterInfrastructure
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // other
            services = RegisterApplication.RegisterServices(services);
            services.AddScoped<IConfigurationReader, ConfigurationReader>();
            // Handelers
            services.AddScoped<MessageSendHandler>();
            services.AddScoped<SlashCommandHandler>();
            services.AddScoped<ButtonPressHandler>();
            services.AddScoped<FormHandler>();
            services.AddScoped<CommandService>();
            // command and events 
            services = RegisterEvents(services);
            return services;
        }

        public static IServiceCollection RegisterEvents(this IServiceCollection services)
        {
            services.AddScoped<TalkWithAiryManager>();
            services.AddScoped<TimeoutEvent>();
            services.AddScoped<UntimeOutEvent>();
            services.AddScoped<UserlogsEvent>();
            return services;
        }
    }
}
