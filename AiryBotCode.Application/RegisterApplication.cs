using AiryBotCode.Application.Features.Giveaway;
using AiryBotCode.Application.Features.Logging;
using AiryBotCode.Application.Features.Moderation;
using AiryBotCode.Application.Features.Reminders;
using AiryBotCode.Application.Services;
using AiryBotCode.Application.Services.Loging;
using AiryBotCode.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Services.Database.GiveAway;
using AiryBotCode.Application.Features.ContactUser;

namespace AiryBotCode.Application
{
    public static class RegisterApplication
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Configuration
            services.AddSingleton<IConfigurationReader, ConfigurationReader>();

            // Register commands
            services.AddScoped<TimeoutCommand>();
            services.AddScoped<UntimeoutCommand>();
            services.AddScoped<UserlogsCommand>();
            services.AddScoped<ReminderCommand>();
            services.AddScoped<VerifyUserAgeCommand>();
            services.AddScoped<ContactUserCommand>();
            services.AddScoped<GiveawayCommand>();
            // SERVICES
            services.AddScoped<UserService>();
            services.AddScoped<LogService>();
            services.AddScoped<DiscordService>();
            services.AddScoped<IGiveAwayUserService, GiveAwayUserService>();
            services.AddScoped<GiveAwayUserService>();

            return services;
        }

    }
}
