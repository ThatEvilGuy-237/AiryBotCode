
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Events.SlashCommands
{
    public static class SlashCommandRegistrator
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register as Singleton
            services.AddSingleton<SlashCommandHandler>();

            return services;
        }
    }
}
