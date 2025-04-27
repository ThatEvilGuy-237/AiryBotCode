using AiryBotCode.Events.SlashCommands.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Events.SlashCommands
{
    public static class SlashCommandRegistrator
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register as Singleton

            // timeout and untimeout commands

            services.AddSingleton<UntimeoutCommand>();
            // user logs command
            services.AddSingleton<UserLogsCommand>();
            return services;
        }
    }
}
