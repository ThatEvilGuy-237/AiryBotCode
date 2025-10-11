using AiryBotCode.Bot.Bots;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Registers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AiryBotCode.Bot.Registers
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IConfigurationReader, ConfigurationReader>();
            services = RegisterInfrastructure.RegisterServices(services);
            // Register required services
            services.AddScoped<AiryDevBot>();
            services = AiryDevBot.CreateClientService(services);

           
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
