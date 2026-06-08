using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    /// <summary>
    /// Which commands each bot runs (enable/disable per bot).
    /// </summary>
    public interface IBotCommandRepository
    {
        Task<List<BotCommand>> GetForBotAsync(ulong botId);

        /// <summary>Create rows for any commands not yet present for this bot, with the given default.</summary>
        Task EnsureSeededAsync(ulong botId, IEnumerable<string> commandNames, bool defaultEnabled);

        Task<bool> SetEnabledAsync(ulong botId, string commandName, bool enabled);
    }
}
