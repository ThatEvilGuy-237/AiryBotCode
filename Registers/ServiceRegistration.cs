using AiryBotCode.AiryBot;
using AiryBotCode.Interfaces;
using AiryBotCode.Registers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AiryBotCode.Register
{
    internal static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register Service
            services.AddScoped<IConfigurationReader, ConfigurationReader>();
            services.AddScoped<IBot, Bot>();

            return services;
        }

        public static IServiceProvider BuildServiceProvider()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            return new ServiceCollection()
                    .AddLogging()
                    .AddSingleton<IConfiguration>(configuration)
                    .RegisterServices()
                    .BuildServiceProvider();
        }
    }
}
