using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AiryBotCode.Api.Models;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AiryBotCode.Api.Controllers
{
    // Body for the password-gated token reveal.
    public class RevealRequest
    {
        public string Password { get; set; }
    }

    // Reads/writes the real BotSettings table (the row a bot seeds on first run).
    // Editing token/name/channels here takes effect after a bot reload.
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly IBotSettingRepository _repository;
        private readonly IConfiguration _configuration;

        public SettingsController(IBotSettingRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _repository.GetAllAsync();
            return Ok(settings.Select(ToDto));
        }

        // Add a new bot to the roster. The token is set directly here (a new bot
        // needs one to log in); BotId is whatever the operator types.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BotSettingDto dto)
        {
            if (!ulong.TryParse(dto.BotId, out var id))
            {
                return BadRequest("A valid numeric Bot ID is required.");
            }
            if (string.IsNullOrWhiteSpace(dto.BotName))
            {
                return BadRequest("Bot name is required.");
            }
            if (await _repository.GetBotSettingAsync(id) != null)
            {
                return Conflict("A bot with that ID already exists.");
            }

            var entity = new BotSetting { BotId = id };
            ApplyDto(entity, dto, isCreate: true);

            await _repository.CreateBotSettingAsync(entity);
            return StatusCode(201, ToDto(entity));
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

            // BotId is the primary key. If it changed, re-key by removing the old
            // row and inserting a new one (carrying the stored token forward unless
            // a new one was supplied).
            if (ulong.TryParse(dto.BotId, out var newId) && newId != id)
            {
                if (await _repository.GetBotSettingAsync(newId) != null)
                {
                    return Conflict("A bot with the new ID already exists.");
                }

                var replacement = new BotSetting { BotId = newId, Token = entity.Token };
                ApplyDto(replacement, dto, isCreate: false);

                await _repository.DeleteAsync(entity);
                await _repository.AddAsync(replacement);
                await _repository.SaveChangesAsync();
                return Ok(ToDto(replacement));
            }

            ApplyDto(entity, dto, isCreate: false);
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return Ok(ToDto(entity));
        }

        [HttpDelete("{botId}")]
        public async Task<IActionResult> Delete(string botId)
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

            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();
            return NoContent();
        }

        // Maps editable fields from the DTO onto an entity. On create the token is
        // taken verbatim; on update a blank token means "keep the stored one".
        private static void ApplyDto(BotSetting entity, BotSettingDto dto, bool isCreate)
        {
            entity.BotName = dto.BotName ?? entity.BotName ?? string.Empty;
            entity.Enabled = dto.Enabled;
            entity.OpenAIModel = dto.OpenAIModel ?? entity.OpenAIModel ?? string.Empty;
            entity.OpenAIPrompt = dto.OpenAIPrompt;
            entity.AdminRoleIds = dto.AdminRoleIds;

            entity.EvilId = ulong.TryParse(dto.EvilId, out var evilId) ? evilId : (isCreate ? 0UL : entity.EvilId);
            entity.LogChannelId = ulong.TryParse(dto.LogChannelId, out var logCh) ? logCh : (isCreate ? 0UL : entity.LogChannelId);
            entity.EvilLogChannelId = ulong.TryParse(dto.EvilLogChannelId, out var evilCh) ? evilCh : (isCreate ? 0UL : entity.EvilLogChannelId);

            if (isCreate)
            {
                entity.Token = dto.Token ?? string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(dto.Token))
            {
                entity.Token = dto.Token;
            }
        }

        // Reveal the stored token — gated behind re-entering the access password
        // (same one as the login gate). [Authorize] still applies on top.
        [HttpPost("{botId}/token")]
        public async Task<IActionResult> RevealToken(string botId, [FromBody] RevealRequest request)
        {
            var expected = _configuration["Panel:GatePassword"];
            if (string.IsNullOrEmpty(expected))
            {
                return StatusCode(500, "Access gate is not configured.");
            }

            var provided = request?.Password ?? string.Empty;
            var providedBytes = Encoding.UTF8.GetBytes(provided);
            var expectedBytes = Encoding.UTF8.GetBytes(expected);
            if (providedBytes.Length != expectedBytes.Length ||
                !CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes))
            {
                // 403 (not 401): the JWT is valid; only the second-factor password
                // failed — so the SPA must NOT treat it as an expired session.
                return StatusCode(403);
            }

            if (!ulong.TryParse(botId, out var id))
            {
                return BadRequest("Invalid bot id.");
            }

            var entity = await _repository.GetBotSettingAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(new { token = entity.Token ?? string.Empty });
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
