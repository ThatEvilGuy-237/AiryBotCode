using AiryBotCode.Domain.database;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Interfaces.Repository
{
    // Extends the generic repository so the control-panel API also gets
    // GetAllAsync / UpdateAsync / SaveChangesAsync (already implemented by the
    // EvilRepository<BotSetting> base) for listing and editing bot settings.
    public interface IBotSettingRepository : IEvilRepository<BotSetting>
    {
        Task<BotSetting> GetBotSettingAsync(ulong botId);
        Task<BotSetting> GetBotSettingAsync(string botName);
        Task CreateBotSettingAsync(BotSetting botSetting);
    }
}
