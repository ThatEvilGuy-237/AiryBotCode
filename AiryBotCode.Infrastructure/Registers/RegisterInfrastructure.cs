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
            // Hive effect passthrough: deliver outbound agent messages to Discord.
            services.AddScoped<AiryBotCode.Application.Hive.IEffectDelivery, Hive.DiscordEffectDelivery>();



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
            return services;
        }
    }
}
