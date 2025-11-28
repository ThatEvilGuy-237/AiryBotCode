using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository.BotSettings
{
    public class BotSettingRepository : EvilRepository<BotSetting>, IBotSettingRepository
    {
        public  BotSettingRepository(AIDbContext context) : base(context) { }

        public async Task<BotSetting> GetBotSettingAsync(ulong botId)
        {
            return await _dbSet.FirstOrDefaultAsync(b => b.BotId == botId);
        }

        public async Task<BotSetting> GetBotSettingAsync(string botName)
        {
            return await _dbSet.FirstOrDefaultAsync(b => b.BotName == botName);
        }

        public async Task CreateBotSettingAsync(BotSetting botSetting)
        {
            _dbSet.Add(botSetting);
            await _context.SaveChangesAsync();
        }
    }
}
