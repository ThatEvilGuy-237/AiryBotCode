using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Database.Seeders
{
    /// <summary>
    /// Populates the CommandSettings table from the command attributes. Must run
    /// after the full DI container is built (commands depend on the Discord
    /// client), so it is invoked from the bot's startup once it is ready.
    /// </summary>
    public static class CommandSettingsSeeder
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var repository = sp.GetRequiredService<ICommandSettingsRepository>();
            var scanner = new CommandSettingsScanner(sp);

            var declarations = scanner.Scan();
            await repository.AddOrUpdateDeclarationsAsync(declarations);

            Console.WriteLine($"- Seeded/refreshed {declarations.Count} command settings.");
        }
    }
}
