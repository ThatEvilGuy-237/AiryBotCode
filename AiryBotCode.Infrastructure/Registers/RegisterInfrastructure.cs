using AiryBotCode.Application;
using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Infrastructure.Database.Persistence;
using AiryBotCode.Infrastructure.DiscordEvents;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Infrastructure.Database.Repository;
using AiryBotCode.Infrastructure.Database.Repository.BotSettings;
using AiryBotCode.Infrastructure.Database.Repository.GiveAway;


namespace AiryBotCode.Infrastructure.Registers
{
    public static class RegisterInfrastructure
    {
        
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // repositorys
            services.AddScoped(typeof(IEvilRepository<>), typeof(EvilRepository<>));
            services.AddScoped<IBotSettingRepository, BotSettingRepository>();
            services.AddScoped<IGiveAwayUserRepository, GiveAwayUserRepository>();
            services.AddScoped<ICommandSettingsRepository, CommandSettingsRepository>();
            services.AddScoped<IBotCommandRepository, BotCommandRepository>();
            services.AddScoped<IChannelWebhookRepository, ChannelWebhookRepository>();
            services.AddScoped<IUserConsentRepository, UserConsentRepository>();
            services.AddScoped<ILevelUserRepository, LevelUserRepository>();
            services.AddScoped<ICountingStateRepository, CountingStateRepository>();
            services.AddScoped<ISuggestionRepository, SuggestionRepository>();
            // DbContext
            services = AIDbContext.registerDbContext(services);
            // Other
            services = RegisterApplication.RegisterServices(services);
            // Handelers
            services.AddScoped<MessageSendHandler>();
            services.AddScoped<SlashCommandHandler>();
            services.AddScoped<ButtonPressHandler>();
            services.AddScoped<FormHandler>();
            services.AddScoped<CommandService>();
            services.AddScoped<BanHandler>();
            services.AddScoped<DiscordService>();
            // Per-channel dedup shared by the effect (say) + webhook-reply paths so an
            // answer the agent both says and finishes with is posted only once.
            services.AddSingleton<AiryBotCode.Application.Hive.RecentMessageDedup>();
            // Hive effect passthrough: deliver outbound agent messages to Discord.
            services.AddScoped<AiryBotCode.Application.Hive.IEffectDelivery, Hive.DiscordEffectDelivery>();
            // await-mode ask_user: deliver a question with option buttons.
            services.AddScoped<AiryBotCode.Application.Hive.IAskDelivery, Hive.DiscordAskDelivery>();
            // Singleton bridge: the long-lived WS listener binds itself here so the
            // request-scoped button handler can send the user's answer back up the WS.
            services.AddSingleton<AiryBotCode.Application.Hive.HiveEffectGateway>();
            services.AddSingleton<AiryBotCode.Application.Hive.IHiveResponseSender>(
                sp => sp.GetRequiredService<AiryBotCode.Application.Hive.HiveEffectGateway>());
            // Counting mini-boss answers pushed from the Hive land here (→ DB).
            services.AddScoped<AiryBotCode.Application.Hive.ICountingBossSink, Hive.CountingBossSink>();
            // Agent-filed suggestions (submit_suggestion effect) → /suggestions board.
            // Singleton: holds the cross-listener effect-id dedup and scopes its own repo.
            services.AddSingleton<AiryBotCode.Application.Hive.ISuggestionIngest, Hive.SuggestionIngest>();



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
            services.AddScoped<GiveawayAction>();
            services.AddScoped<SpamCatcherAction>();
            services.AddScoped<LevelingAction>();
            services.AddScoped<CountingAction>();
            return services;
        }
    }
}
