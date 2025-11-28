using AiryBotCode.Domain.database;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Interfaces.Repository
{
    public interface IBotSettingRepository
    {
        Task<BotSetting> GetBotSettingAsync(ulong botId);
        Task<BotSetting> GetBotSettingAsync(string botName);
        Task CreateBotSettingAsync(BotSetting botSetting);
    }
}
