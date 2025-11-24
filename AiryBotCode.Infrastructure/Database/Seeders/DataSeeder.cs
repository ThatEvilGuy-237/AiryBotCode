using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AiryBotCode.Infrastructure.Database.Seeders
{
    public static class DataSeeder
    {
        public static async Task SeedData(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AIDbContext>();
                await BotSettingsSeeder.Seed(scope.ServiceProvider);
                // Add other seeders here if needed in the future
            }
        }
    }
}
