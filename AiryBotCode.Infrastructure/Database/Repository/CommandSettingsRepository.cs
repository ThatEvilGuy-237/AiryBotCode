using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class CommandSettingsRepository : ICommandSettingsRepository
    {
        private readonly AIDbContext _context;

        public CommandSettingsRepository(AIDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommandSetting>> GetAllSettingsAsync()
        {
            return await _context.CommandSettings.AsNoTracking().ToListAsync();
        }

        public async Task<List<CommandSetting>> GetByCommandAsync(string commandName)
        {
            return await _context.CommandSettings.AsNoTracking()
                .Where(c => c.CommandName == commandName)
                .ToListAsync();
        }

        public async Task AddOrUpdateDeclarationsAsync(List<CommandSetting> scanned)
        {
            var existing = await _context.CommandSettings
                .ToDictionaryAsync(s => (s.CommandName, s.Key));

            foreach (var setting in scanned)
            {
                if (existing.TryGetValue((setting.CommandName, setting.Key), out var current))
                {
                    // Refresh declared metadata but keep the stored (possibly edited) value.
                    current.Description = setting.Description;
                    current.Category = setting.Category;
                    current.UiHint = setting.UiHint;
                    current.IsReloadable = setting.IsReloadable;
                }
                else
                {
                    await _context.CommandSettings.AddAsync(setting);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateValueAsync(string commandName, string key, string value)
        {
            var row = await _context.CommandSettings
                .FirstOrDefaultAsync(c => c.CommandName == commandName && c.Key == key);
            if (row == null) return false;

            row.Value = value;
            row.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
