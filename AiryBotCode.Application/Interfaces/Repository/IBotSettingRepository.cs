using AiryBotCode.Domain.database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface IBotSettingRepository
    {
        Task<BotSetting> GetBotSettingAsync(ulong botId);
        Task<BotSetting> GetBotSettingAsync(string botName);
        Task<IReadOnlyList<BotSetting>> GetAllBotSettingsAsync();
        Task CreateBotSettingAsync(BotSetting botSetting);
        Task UpdateBotSettingAsync(BotSetting botSetting);
    }
}
