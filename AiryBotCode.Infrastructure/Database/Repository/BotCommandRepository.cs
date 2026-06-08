using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class BotCommandRepository : IBotCommandRepository
    {
        private readonly AIDbContext _context;

        public BotCommandRepository(AIDbContext context)
        {
            _context = context;
        }

        public async Task<List<BotCommand>> GetForBotAsync(ulong botId)
        {
            return await _context.BotCommands.AsNoTracking()
                .Where(c => c.BotId == botId)
                .ToListAsync();
        }

        public async Task EnsureSeededAsync(ulong botId, IEnumerable<string> commandNames, bool defaultEnabled)
        {
            var present = await _context.BotCommands
                .Where(c => c.BotId == botId)
                .Select(c => c.CommandName)
                .ToListAsync();
            var have = present.ToHashSet();

            foreach (var name in commandNames.Distinct())
            {
                if (have.Contains(name)) continue;
                await _context.BotCommands.AddAsync(new BotCommand
                {
                    BotId = botId,
                    CommandName = name,
                    Enabled = defaultEnabled
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> SetEnabledAsync(ulong botId, string commandName, bool enabled)
        {
            var row = await _context.BotCommands
                .FirstOrDefaultAsync(c => c.BotId == botId && c.CommandName == commandName);
            if (row == null)
            {
                await _context.BotCommands.AddAsync(new BotCommand
                {
                    BotId = botId,
                    CommandName = commandName,
                    Enabled = enabled
                });
            }
            else
            {
                row.Enabled = enabled;
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
