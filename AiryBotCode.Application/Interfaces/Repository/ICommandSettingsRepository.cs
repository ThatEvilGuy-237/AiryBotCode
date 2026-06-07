using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    /// <summary>
    /// Persistence for the per-command settings that are projected from the
    /// [ConfigurableCommand] / [LiveSetting] / [ReloadableSetting] attributes.
    /// </summary>
    public interface ICommandSettingsRepository
    {
        Task<List<CommandSetting>> GetAllSettingsAsync();

        Task<List<CommandSetting>> GetByCommandAsync(string commandName);

        /// <summary>
        /// Insert any newly-declared settings (with their default value) and refresh
        /// the metadata of existing rows WITHOUT overwriting their stored value, so
        /// values edited through the UI survive restarts/re-seeds.
        /// </summary>
        Task AddOrUpdateDeclarationsAsync(List<CommandSetting> scanned);

        Task<bool> UpdateValueAsync(string commandName, string key, string value);
    }
}
