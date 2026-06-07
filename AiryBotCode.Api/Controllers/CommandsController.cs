using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    /// <summary>
    /// Serves the per-command settings (projected from the command attributes and
    /// stored in the CommandSettings table) to the control panel, and persists
    /// edits made there.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandSettingsRepository _repository;

        public CommandsController(ICommandSettingsRepository repository)
        {
            _repository = repository;
        }

        // GET /api/commands -> settings grouped per command
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandConfigDto>>> GetAll()
        {
            var rows = await _repository.GetAllSettingsAsync();

            var grouped = rows
                .GroupBy(r => r.CommandName)
                .Select(g => new CommandConfigDto
                {
                    CommandName = g.Key,
                    Settings = g.Select(SettingDto.FromEntity).ToList()
                })
                .ToList();

            return Ok(grouped);
        }

        // PUT /api/commands/{commandName} -> persist edited values
        [HttpPut("{commandName}")]
        public async Task<IActionResult> Update(string commandName, [FromBody] UpdateCommandDto body)
        {
            if (body?.Settings == null) return BadRequest();

            foreach (var setting in body.Settings)
            {
                await _repository.UpdateValueAsync(commandName, setting.Key, setting.Value ?? string.Empty);
            }

            return NoContent();
        }
    }

    public class CommandConfigDto
    {
        public string CommandName { get; set; } = string.Empty;
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
}
