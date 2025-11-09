using AiryBotCode.Application;
using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Infrastructure.Database.Persistence;
using AiryBotCode.Infrastructure.Database.Repository;
using AiryBotCode.Infrastructure.DiscordEvents;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces.Repository;


namespace AiryBotCode.Infrastructure.Registers
{
    public static class RegisterInfrastructure
    {
        
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
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
            // repositorys
            services.AddScoped(typeof(IEvilRepository<>), typeof(EvilRepository<>));
            services.AddScoped<IChannelConversationRepository, ChannelConversationRepository>();
            services.AddScoped<IChatUserRepository, ChatUserRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();


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
            services.AddScoped<TalkToAiryAction>();
            services.AddScoped<SummarizeUserAction>(); // Register the new action
            return services;
        }
    }
}
