using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Database.Repository;
using AiryBotCode.Infrastructure.Database.Repository.BotSettings;

namespace AiryBotCode.Api
{
    public static class PanelServices
    {
        public static IServiceCollection AddPanelServices(this IServiceCollection services)
        {
            services.AddSingleton<Services.DevicePairingService>();
            services.AddSingleton<Services.DiscordGuildLookup>();
            services.AddScoped<ICommandSettingsRepository, CommandSettingsRepository>();
            services.AddScoped<IBotCommandRepository, BotCommandRepository>();
            services.AddScoped<IChannelWebhookRepository, ChannelWebhookRepository>();
            services.AddScoped<IAiryChannelRepository, AiryChannelRepository>();
            services.AddScoped<IBotSettingRepository, BotSettingRepository>();
            services.AddScoped<ISuggestionRepository, SuggestionRepository>();
            services.AddScoped<ISuggestionShareCodeRepository, SuggestionShareCodeRepository>();
            services.AddScoped<IShareCodeService, ShareCodeService>();
            services.AddScoped<ICountingStateRepository, CountingStateRepository>();
            services.AddScoped<ILevelUserRepository, LevelUserRepository>();
            services.AddSingleton(new HiveAnswerClient(new HttpClient { Timeout = TimeSpan.FromSeconds(15) }));
            return services;
        }
    }
}
