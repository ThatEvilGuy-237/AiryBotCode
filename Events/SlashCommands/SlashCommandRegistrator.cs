using AiryBotCode.Events.SlashCommands.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Events.SlashCommands
{
    public static class SlashCommandRegistrator
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register as Singleton
            services.AddSingleton<SlashCommandHandler>();
            services.AddSingleton<BaseSlashCommand>();
            services.AddSingleton<RockPaperScissors>();

            return services;
        }
    }
}
