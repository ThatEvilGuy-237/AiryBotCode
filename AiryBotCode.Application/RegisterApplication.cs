using AiryBotCode.Application.Comands;
using AiryBotCode.Application.Comands.ConversationalInteractions;
using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Application.Services.Loging;
using AiryBotCode.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application
{
    public static class RegisterApplication
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register commands
            // STATIC LIKE (can save run time data)
            //services.AddSingleton<TimeoutCommand>();
            // Register Services
            // INDEPENDENT
            // COMMAND
            services.AddScoped<TimeoutCommand>();
            services.AddScoped<UntimeoutCommand>();
            services.AddScoped<UserlogsCommand>();
            services.AddScoped<ReminderCommand>();
            services.AddScoped<VerifyUserAgeCommand>();
            services.AddScoped<ContactUserCommand>();
            // MESSAGE SEND
            services.AddScoped<TalkToAiry>();

            // SERIVCES
            services.AddScoped<UserService>();
            services.AddScoped<LogService>();

            return services;
        }

    }
}
