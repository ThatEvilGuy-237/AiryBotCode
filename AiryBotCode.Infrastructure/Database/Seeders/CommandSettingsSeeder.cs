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
        public static async Task Seed(IServiceProvider serviceProvider, ulong botId)
        {
            using var scope = serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var repository = sp.GetRequiredService<ICommandSettingsRepository>();
            var scanner = new CommandSettingsScanner(sp);

            var declarations = scanner.Scan(botId);
            await repository.AddOrUpdateDeclarationsAsync(declarations);

            await MigrateSpamCatcherRolesAsync(repository, botId);

            Console.WriteLine($"- Seeded/refreshed {declarations.Count} command settings for bot {botId}.");
        }

        /// <summary>
        /// One-time migration: the SpamCatcher's single <c>MonitoredRoleId</c> +
        /// <c>InvertMonitoredRole</c> + <c>IgnoredRoleId</c> were replaced by the
        /// <c>MonitoredRoleIds</c> / <c>IgnoredRoleIds</c> lists. Convert any pre-existing
        /// single-role config into the new lists — preserving behaviour exactly — then drop
        /// the obsolete keys so this runs at most once per bot.
        ///   • monitored role, not inverted → monitor list = [role]
        ///   • monitored role, inverted      → exclude list = [role]  (watch everyone except it)
        ///   • no monitored role (was OFF)   → both lists empty (stays OFF)
        ///   • old exempt role               → added to the exclude list
        /// </summary>
        private static async Task MigrateSpamCatcherRolesAsync(ICommandSettingsRepository repo, ulong botId)
        {
            const string cmd = "SpamCatcherCommand";
            var rows = (await repo.GetByCommandAsync(botId, cmd))
                .ToDictionary(r => r.Key, r => r.Value ?? string.Empty);

            // Idempotent: the old key only exists before this migration and is deleted below.
            if (!rows.ContainsKey("MonitoredRoleId")) return;

            ulong.TryParse(rows.GetValueOrDefault("MonitoredRoleId", "0"), out var monitored);
            ulong.TryParse(rows.GetValueOrDefault("IgnoredRoleId", "0"), out var ignored);
            var invert = string.Equals(rows.GetValueOrDefault("InvertMonitoredRole", "false"), "true",
                System.StringComparison.OrdinalIgnoreCase);

            var monitoredIds = new List<ulong>();
            var ignoredIds = new List<ulong>();

            if (monitored != 0)
            {
                if (invert) ignoredIds.Add(monitored);   // "watch everyone EXCEPT this role" → exclusion
                else monitoredIds.Add(monitored);        // watch holders of this role
            }
            // monitored == 0 was OFF → leave both empty so the catcher stays OFF.

            if (ignored != 0 && !ignoredIds.Contains(ignored)) ignoredIds.Add(ignored);

            await repo.UpdateValueAsync(botId, cmd, "MonitoredRoleIds", "[" + string.Join(",", monitoredIds) + "]");
            await repo.UpdateValueAsync(botId, cmd, "IgnoredRoleIds", "[" + string.Join(",", ignoredIds) + "]");
            await repo.DeleteByKeysAsync(botId, cmd, new[] { "MonitoredRoleId", "InvertMonitoredRole", "IgnoredRoleId" });

            Console.WriteLine($"- Migrated SpamCatcher roles for bot {botId}: " +
                $"monitor=[{string.Join(",", monitoredIds)}] ignore=[{string.Join(",", ignoredIds)}].");
        }
    }
}
