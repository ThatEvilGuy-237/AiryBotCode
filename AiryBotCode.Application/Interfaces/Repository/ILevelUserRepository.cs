using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface ILevelUserRepository
    {
        Task<LevelUser?> GetAsync(ulong botId, ulong userId);
        Task SaveAsync(LevelUser user);
        Task<IReadOnlyList<LevelUser>> TopAsync(ulong botId, int skip, int take);
        Task<int> CountAsync(ulong botId);
        // 1-based position by XP (highest = 1); 0 when the user has no row.
        Task<int> GetRankAsync(ulong botId, ulong userId);
    }
}
