using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    /// <summary>
    /// Serves the per-bot command configuration to the control panel: which
    /// commands a bot runs (enable/disable) and each command's settings. All
    /// endpoints are scoped to a bot via the required <c>botId</c>. Requires a JWT.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandSettingsRepository _repository;
        private readonly IBotCommandRepository _botCommands;

        public CommandsController(ICommandSettingsRepository repository, IBotCommandRepository botCommands)
        {
            _repository = repository;
            _botCommands = botCommands;
        }

        // GET /api/commands?botId=123 -> commands for that bot (enabled flag + settings)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandConfigDto>>> GetAll([FromQuery] string botId)
        {
            if (!ulong.TryParse(botId, out var id))
                return BadRequest("A valid botId is required.");

            var commands = await _botCommands.GetForBotAsync(id);
            var settings = await _repository.GetAllSettingsAsync(id);
            var byCommand = settings.GroupBy(s => s.CommandName)
                .ToDictionary(g => g.Key, g => g.Select(SettingDto.FromEntity).ToList());

            var result = commands
                .OrderBy(c => c.CommandName)
                .Select(c => new CommandConfigDto
                {
                    CommandName = c.CommandName,
                    Enabled = c.Enabled,
                    Settings = byCommand.TryGetValue(c.CommandName, out var rows) ? rows : new List<SettingDto>()
                })
                .ToList();

            return Ok(result);
        }

        // PUT /api/commands/{commandName}?botId=123 -> persist edited values for that bot
        [HttpPut("{commandName}")]
        public async Task<IActionResult> Update(string commandName, [FromQuery] string botId, [FromBody] UpdateCommandDto body)
        {
            if (!ulong.TryParse(botId, out var id))
                return BadRequest("A valid botId is required.");
            if (body?.Settings == null) return BadRequest();

            foreach (var setting in body.Settings)
            {
                await _repository.UpdateValueAsync(id, commandName, setting.Key, setting.Value ?? string.Empty);
            }

            return NoContent();
        }

        // PUT /api/commands/{commandName}/enabled?botId=123 -> enable/disable for that bot
        [HttpPut("{commandName}/enabled")]
        public async Task<IActionResult> SetEnabled(string commandName, [FromQuery] string botId, [FromBody] EnabledDto body)
        {
            if (!ulong.TryParse(botId, out var id))
                return BadRequest("A valid botId is required.");

            await _botCommands.SetEnabledAsync(id, commandName, body?.Enabled ?? false);
            return NoContent();
        }
    }

    public class CommandConfigDto
    {
        public string CommandName { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public List<SettingDto> Settings { get; set; } = new();
    }

    public class SettingDto
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string UiHint { get; set; } = "text";
        public bool IsReloadable { get; set; }

        public static SettingDto FromEntity(CommandSetting c) => new()
        {
            Key = c.Key ?? string.Empty,
            Value = c.Value ?? string.Empty,
            Description = c.Description ?? string.Empty,
            Category = string.IsNullOrWhiteSpace(c.Category) ? "General" : c.Category,
            UiHint = string.IsNullOrWhiteSpace(c.UiHint) ? "text" : c.UiHint,
            IsReloadable = c.IsReloadable
        };
    }

    public class UpdateCommandDto
    {
        public List<UpdateSettingDto> Settings { get; set; } = new();
    }

    public class UpdateSettingDto
    {
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class EnabledDto
    {
        public bool Enabled { get; set; }
    }
}
