using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    public class CommandSettingsRepository : ICommandSettingsRepository
    {
        // Reserved command name for internal control rows (restart signal, etc.).
        // Hidden from the command listing so it never shows up as a "command".
        private const string ControlCommand = "$control";

        private readonly AIDbContext _context;

        public CommandSettingsRepository(AIDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommandSetting>> GetAllSettingsAsync(ulong botId)
        {
            return await _context.CommandSettings.AsNoTracking()
                .Where(c => c.BotId == botId && c.CommandName != ControlCommand)
                .ToListAsync();
        }

        public async Task<List<CommandSetting>> GetByCommandAsync(ulong botId, string commandName)
        {
            return await _context.CommandSettings.AsNoTracking()
                .Where(c => c.BotId == botId && c.CommandName == commandName)
                .ToListAsync();
        }

        public async Task AddOrUpdateDeclarationsAsync(List<CommandSetting> scanned)
        {
            var botIds = scanned.Select(s => s.BotId).Distinct().ToList();
            var existing = await _context.CommandSettings
                .Where(s => botIds.Contains(s.BotId))
                .ToDictionaryAsync(s => (s.BotId, s.CommandName, s.Key));

            foreach (var setting in scanned)
            {
                if (existing.TryGetValue((setting.BotId, setting.CommandName, setting.Key), out var current))
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

        public async Task<bool> UpdateValueAsync(ulong botId, string commandName, string key, string value)
        {
            var row = await _context.CommandSettings
                .FirstOrDefaultAsync(c => c.BotId == botId && c.CommandName == commandName && c.Key == key);
            if (row == null) return false;

            row.Value = value;
            row.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DateTime> GetMaxLastUpdatedAsync(ulong botId)
        {
            var rows = _context.CommandSettings.Where(c => c.BotId == botId && c.CommandName != ControlCommand);
            return await rows.AnyAsync()
                ? await rows.MaxAsync(c => c.LastUpdated)
                : DateTime.MinValue;
        }

        public async Task<string?> GetControlValueAsync(string key)
        {
            var row = await _context.CommandSettings.AsNoTracking()
                .FirstOrDefaultAsync(c => c.BotId == 0 && c.CommandName == ControlCommand && c.Key == key);
            return row?.Value;
        }

        public async Task SetControlValueAsync(string key, string value)
        {
            var row = await _context.CommandSettings
                .FirstOrDefaultAsync(c => c.BotId == 0 && c.CommandName == ControlCommand && c.Key == key);
            if (row == null)
            {
                await _context.CommandSettings.AddAsync(new CommandSetting
                {
                    BotId = 0,
                    CommandName = ControlCommand,
                    Key = key,
                    Value = value,
                    Description = string.Empty,
                    Category = "Control",
                    UiHint = "text",
                    IsReloadable = false,
                    LastUpdated = DateTime.UtcNow
                });
            }
            else
            {
                row.Value = value;
                row.LastUpdated = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
    }
}
