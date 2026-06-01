using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Settings
{
    /// <summary>
    /// Default <see cref="ISettingsProvider"/>: preloads <see cref="AirySettings"/>
    /// once and hands the same tree to every handler. Reads the persisted
    /// <see cref="BotSetting"/> (seeded from config) and projects it via
    /// <see cref="AirySettings.FromBotSetting"/>; falls back to config-built
    /// settings if the database row isn't there yet.
    /// </summary>
    public class SettingsProvider : ISettingsProvider
    {
        private readonly IServiceProvider _services;
        private readonly IConfigurationReader _config;
        private readonly object _lock = new();
        private AirySettings? _current;

        public SettingsProvider(IServiceProvider services, IConfigurationReader config)
        {
            _services = services;
            _config = config;
        }

        public AirySettings Current
        {
            get
            {
                var snapshot = _current;
                if (snapshot != null) return snapshot;
                // Nobody preloaded at startup — load now (blocking) so handlers stay safe.
                return LoadAsync().GetAwaiter().GetResult();
            }
        }

        public async Task<AirySettings> LoadAsync()
        {
            var (setting, origin) = await ReadSettingAsync();
            var built = AirySettings.FromBotSetting(setting);
            lock (_lock) { _current = built; }

            Console.WriteLine(
                $"[SETTINGS] Loaded for bot '{setting.BotName}' (source: {origin}). " +
                $"Owner={built.Owner}, Listen={built.Channels.Listen.Count} channel(s), " +
                $"Admin={built.Roles.Admin.Count} role(s).");

            return built;
        }

        public Task<AirySettings> RefreshAsync() => LoadAsync();

        private async Task<(BotSetting setting, string origin)> ReadSettingAsync()
        {
            try
            {
                using var scope = _services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IBotSettingRepository>();
                var setting = await repo.GetBotSettingAsync(_config.GetBotId());
                if (setting != null) return (setting, "database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SETTINGS] DB read failed, falling back to config: {ex.Message}");
            }

            // No persisted row yet — build straight from configuration.
            return (_config.GetBotSettings(), "config");
        }
    }
}
