using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiryBotCode.Api.Models;
using AiryBotCode.Application.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    /// <summary>
    /// Read/write access to the persisted bot settings that the control-panel UI
    /// edits. Backed by <see cref="IBotSettingRepository"/> (the same
    /// <c>BotSetting</c> rows the bots seed and read at startup), so a save here
    /// is picked up by a bot on its next settings load/restart.
    /// </summary>
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

        /// <summary>All configured bots (tokens stripped).</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BotSettingDto>>> GetAll()
        {
            var settings = await _repository.GetAllBotSettingsAsync();
            return Ok(settings.Select(BotSettingDto.FromEntity));
        }

        /// <summary>A single bot by its Discord id.</summary>
        [HttpGet("{botId}")]
        public async Task<ActionResult<BotSettingDto>> Get(string botId)
        {
            if (!ulong.TryParse(botId, out var id))
            {
                return BadRequest("Invalid bot id.");
            }

            var setting = await _repository.GetBotSettingAsync(id);
            if (setting == null)
            {
                return NotFound();
            }

            return Ok(BotSettingDto.FromEntity(setting));
        }

        /// <summary>Persist edits for an existing bot.</summary>
        [HttpPut("{botId}")]
        public async Task<ActionResult<BotSettingDto>> Update(string botId, [FromBody] BotSettingDto dto)
        {
            if (!ulong.TryParse(botId, out var id))
            {
                return BadRequest("Invalid bot id.");
            }

            var setting = await _repository.GetBotSettingAsync(id);
            if (setting == null)
            {
                return NotFound();
            }

            dto.ApplyTo(setting);
            await _repository.UpdateBotSettingAsync(setting);

            return Ok(BotSettingDto.FromEntity(setting));
        }
    }
}
