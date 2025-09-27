using AiryBotCode.Application;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.DiscordEvents;
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
            services.AddScoped<BanHandler>();
            // command and events 
            services = RegisterEvents(services);
            return services;
        }

        public static IServiceCollection RegisterEvents(this IServiceCollection services)
        {
            services.AddScoped<MessageSendHandler>();
            services.AddScoped<TimeoutAction>();
            services.AddScoped<UntimeOutAction>();
            services.AddScoped<UserlogsAction>();
            services.AddScoped<ReminderAction>();
            services.AddScoped<VerifyUserAgeAction>();
            services.AddScoped<ContactUserAction>();
            return services;
        }
    }
}
