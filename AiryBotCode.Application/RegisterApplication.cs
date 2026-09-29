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
            services.AddSingleton<IConfigurationReader, ConfigurationReader>();

            services.AddScoped<TimeoutCommand>();
            services.AddScoped<UntimeoutCommand>();
            services.AddScoped<UserlogsCommand>();
            services.AddScoped<ReminderCommand>();
            services.AddScoped<VerifyUserAgeCommand>();
            services.AddScoped<ContactUserCommand>();
            services.AddScoped<GiveawayCommand>();
            services.AddScoped<Features.SpamCatcher.SpamCatcherCommand>();
            services.AddSingleton<Features.SpamCatcher.SpamTracker>();
            services.AddScoped<Features.Leveling.LevelingCommand>();
            services.AddSingleton<Features.Leveling.XpCooldown>();
            services.AddScoped<Features.Counting.CountingCommand>();
            services.AddScoped<Features.HiveChat.HiveChatCommand>();
            services.AddScoped<UserService>();
            services.AddScoped<LogService>();
            services.AddScoped<DiscordService>();
            services.AddSingleton(new Hive.HiveAgentClient(new System.Net.Http.HttpClient { Timeout = TimeSpan.FromMinutes(5) }));
            services.AddScoped<WebhookChatService>();
            services.AddScoped<IGiveAwayUserService, GiveAwayUserService>();
            services.AddScoped<GiveAwayUserService>();

            return services;
        }

    }
}
