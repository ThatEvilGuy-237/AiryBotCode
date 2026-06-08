using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Bot.Registers;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AiryBotCode.Bot.Bots;
using Microsoft.Extensions.Configuration;

namespace AiryBotCode
{
    internal class Program
    {
        private static void Main(string[] args) =>
            MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            // Shared/base configuration: the control-DB connection, the OpenAI key,
            // and any other settings common to every bot. Per-bot identity (token,
            // id, name, channels, roles) comes from the DB roster, NOT this file.
            IConfiguration baseConfig = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Read the roster (enabled rows) from the control database.
            List<BotSetting> roster = LoadRoster(baseConfig);
            Console.WriteLine($"[INFO] Roster: {roster.Count} enabled bot(s).");

            // Start one fully independent bot per enabled row — each with its own
            // Discord client and its own configuration view. A single bad row can't
            // take the whole fleet down.
            var started = new List<AiryDevBot>();
            foreach (var row in roster)
            {
                try
                {
                    var perBotConfig = BuildPerBotConfig(baseConfig, row);
                    var serviceProvider = ServiceRegistration.BuildServiceProvider(perBotConfig);

                    Console.WriteLine($"[INFO] Starting {row.BotName} (ID {row.BotId})...");
                    var bot = serviceProvider.GetRequiredService<AiryDevBot>();
                    await bot.StartAsync(serviceProvider);
                    started.Add(bot);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to start {row.BotName} ({row.BotId}): {ex.Message}");
                }
            }

            if (started.Count == 0)
                Console.WriteLine("[WARN] No bots started. Idling — add/enable a bot in the panel, then reload.");
            else
                Console.WriteLine($"[INFO] {started.Count} bot(s) connecting to Discord...");

            // Keep the host alive. Docker stops it externally; the reload watcher
            // exits the process on a control-panel reload so the whole fleet is
            // re-read from the DB on the next start.
            await Task.Delay(Timeout.Infinite);
        }

        // Read the enabled bots from the control DB using a throwaway bootstrap
        // provider built from the shared config.
        private static List<BotSetting> LoadRoster(IConfiguration baseConfig)
        {
            var bootstrap = ServiceRegistration.BuildServiceProvider(baseConfig);
            using var scope = bootstrap.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AIDbContext>();
            return db.BotSettings.Where(b => b.Enabled).ToList();
        }

        // Overlay one roster row onto the shared config so the existing
        // ConfigurationReader transparently serves this bot's identity/settings.
        private static IConfiguration BuildPerBotConfig(IConfiguration baseConfig, BotSetting row)
        {
            var overrides = new Dictionary<string, string?>
            {
                ["Bots:Token"] = row.Token,
                ["Bots:BotId"] = row.BotId.ToString(),
                ["Bots:Name"] = row.BotName,
                ["Bots:Enabled"] = row.Enabled.ToString(),
                ["Bots:LogChannelId"] = row.LogChannelId.ToString(),
                ["Bots:EvilLogChannelId"] = row.EvilLogChannelId.ToString(),
                ["Bots:EvilId"] = row.EvilId.ToString(),
                ["Bots:ThemePrimary"] = row.ThemePrimary,
            };

            // Route this bot's data to its own database when one is assigned. The
            // control plane (roster + reload signal) stays on the shared DB, which
            // the default bot keeps watching — so Restart still reloads the fleet.
            if (!string.IsNullOrWhiteSpace(row.DatabaseName))
                overrides["Database:Name"] = row.DatabaseName.Trim();

            // AdminRoleIds is a comma-separated string in the DB but an array in
            // config — expand it into indexed keys (Bots:AdminRoleIds:0, :1, ...).
            var roleIds = (row.AdminRoleIds ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (int i = 0; i < roleIds.Length; i++)
                overrides[$"Bots:AdminRoleIds:{i}"] = roleIds[i];

            return new ConfigurationBuilder()
                .AddConfiguration(baseConfig)
                .AddInMemoryCollection(overrides)
                .Build();
        }
    }
}
