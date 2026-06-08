using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    /// <summary>
    /// Persistence for the per-command settings that are projected from the
    /// [ConfigurableCommand] / [LiveSetting] / [ReloadableSetting] attributes.
    /// Settings are per-bot (scoped by BotId); control rows are global (BotId 0).
    /// </summary>
    public interface ICommandSettingsRepository
    {
        Task<List<CommandSetting>> GetAllSettingsAsync(ulong botId);

        Task<List<CommandSetting>> GetByCommandAsync(ulong botId, string commandName);

        /// <summary>
        /// Insert any newly-declared settings (with their default value) and refresh
        /// the metadata of existing rows WITHOUT overwriting their stored value, so
        /// values edited through the UI survive restarts/re-seeds. Each scanned row
        /// carries its BotId.
        /// </summary>
        Task AddOrUpdateDeclarationsAsync(List<CommandSetting> scanned);

        Task<bool> UpdateValueAsync(ulong botId, string commandName, string key, string value);

        /// <summary>Newest LastUpdated across this bot's settings, for hot-reload polling.</summary>
        Task<DateTime> GetMaxLastUpdatedAsync(ulong botId);

        /// <summary>Read a global control value (e.g. the restart signal). Null if unset.</summary>
        Task<string?> GetControlValueAsync(string key);

        /// <summary>Upsert a global control value (e.g. bump the restart signal).</summary>
        Task SetControlValueAsync(string key, string value);
    }
}
