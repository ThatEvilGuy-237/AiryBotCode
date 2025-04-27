using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Application.Services;
using AiryBotCode.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application
{
    public static class Register
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register commands
            // STATIC LIKE (can save run time data)
            services.AddSingleton<TimeoutCommand>();
            // Register Services
            // INDEPENDENT
            services.AddScoped<ClientService>();
            services.AddScoped<UserService>();

            return services;
        }

    }
}
