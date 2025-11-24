using AiryBotCode.Application.Comands;
using AiryBotCode.Application.Comands.ConversationalInteractions;
using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Application.Services;
using AiryBotCode.Application.Services.Loging;
using AiryBotCode.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Services.AIService;
using AiryBotCode.Application.Services.Database.ChatHistory;

namespace AiryBotCode.Application
{
    public static class RegisterApplication
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Configuration and AI Clients
            services.AddSingleton<IConfigurationReader, ConfigurationReader>();
            services.AddSingleton(provider =>
            {
                var configReader = provider.GetRequiredService<IConfigurationReader>();
                var apiKey = configReader.GetOpenAIApiKey();
                var modelName = configReader.GetOpenAIModel();
                return new OpenAIClient(apiKey, modelName);
            });

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
            services.AddScoped<SummarizeUserCommand>();
            // MESSAGE SEND
            services.AddScoped<TalkToAiry>();
            // SERIVCES
            services.AddScoped<UserService>();
            services.AddScoped<LogService>();
            services.AddScoped<DiscordService>();
            services.AddScoped<IConversationManagerService, ConversationManagerService>();
            services.AddScoped<IChannelConversationService, ChannelConversationService>();
            services.AddScoped<IChatUserService, ChatUserService>();
            services.AddScoped<IMessageService, MessageService>();

            return services;
        }

    }
}
