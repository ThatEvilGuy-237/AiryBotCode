using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task AddOrUpdateSettingsAsync(List<CommandSetting> settings)
        {
            var existingSettings = await _context.CommandSettings
                .ToDictionaryAsync(s => (s.CommandName, s.Key));

            var settingsToAdd = new List<CommandSetting>();
            foreach (var setting in settings)
            {
                if (existingSettings.TryGetValue((setting.CommandName, setting.Key), out var existingSetting))
                {
                    // Update existing setting
                    existingSetting.Value = setting.Value;
                    existingSetting.Description = setting.Description;
                    existingSetting.Category = setting.Category;
                    existingSetting.UiHint = setting.UiHint;
                    existingSetting.IsReloadable = setting.IsReloadable;
                    existingSetting.LastUpdated = DateTime.UtcNow; // Update timestamp
                    _context.CommandSettings.Update(existingSetting);
                }
                else
                {
                    // Add new setting
                    settingsToAdd.Add(setting);
                }
            }

            if (settingsToAdd.Any())
            {
                await _context.CommandSettings.AddRangeAsync(settingsToAdd);
            }
            
            await _context.SaveChangesAsync();
        }
    }
}
