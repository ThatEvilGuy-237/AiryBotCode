using System.Linq;
using System.Threading.Tasks;
using AiryBotCode.Api.Models;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    // Reads/writes the real BotSettings table (the row a bot seeds on first run).
    // Editing token/name/channels here takes effect after a bot reload.
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly IBotSettingRepository _repository;

        public SettingsController(IBotSettingRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _repository.GetAllAsync();
            return Ok(settings.Select(ToDto));
        }

        [HttpPut("{botId}")]
        public async Task<IActionResult> Update(string botId, [FromBody] BotSettingDto dto)
        {
            if (!ulong.TryParse(botId, out var id))
            {
                return BadRequest("Invalid bot id.");
            }

            var entity = await _repository.GetBotSettingAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.BotName = dto.BotName ?? entity.BotName;
            entity.Enabled = dto.Enabled;
            entity.OpenAIModel = dto.OpenAIModel ?? entity.OpenAIModel;
            entity.OpenAIPrompt = dto.OpenAIPrompt;
            entity.AdminRoleIds = dto.AdminRoleIds;

            if (ulong.TryParse(dto.EvilId, out var evilId)) entity.EvilId = evilId;
            if (ulong.TryParse(dto.LogChannelId, out var logChannelId)) entity.LogChannelId = logChannelId;
            if (ulong.TryParse(dto.EvilLogChannelId, out var evilLogChannelId)) entity.EvilLogChannelId = evilLogChannelId;

            // Only overwrite the token when a new, non-empty one is supplied.
            if (!string.IsNullOrWhiteSpace(dto.Token))
            {
                entity.Token = dto.Token;
            }

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            return Ok(ToDto(entity));
        }

        private static BotSettingDto ToDto(BotSetting b) => new BotSettingDto
        {
            BotId = b.BotId.ToString(),
            BotName = b.BotName,
            Enabled = b.Enabled,
            OpenAIModel = b.OpenAIModel,
            OpenAIPrompt = b.OpenAIPrompt,
            AdminRoleIds = b.AdminRoleIds,
            EvilId = b.EvilId.ToString(),
            LogChannelId = b.LogChannelId.ToString(),
            EvilLogChannelId = b.EvilLogChannelId.ToString(),
            HasToken = !string.IsNullOrEmpty(b.Token),
            Token = null,
        };
    }
}
