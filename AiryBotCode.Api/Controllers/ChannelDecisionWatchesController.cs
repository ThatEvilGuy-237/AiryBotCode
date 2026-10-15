using System.Text.Json;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelDecisionWatchesController : ControllerBase
    {
        private readonly IChannelDecisionWatchRepository _repository;

        public ChannelDecisionWatchesController(IChannelDecisionWatchRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            return Ok((await _repository.GetForBotAsync(id)).Select(ToDto));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string botId, [FromBody] ChannelDecisionWatchDto dto)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var (watch, problem) = FromDto(id, 0, dto, null);
            if (problem is not null) return BadRequest(problem);
            return StatusCode(201, ToDto(await _repository.AddAsync(watch!)));
        }

        [HttpPut("{watchId:int}")]
        public async Task<IActionResult> Update(int watchId, [FromQuery] string botId, [FromBody] ChannelDecisionWatchDto dto)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var existing = await _repository.GetAsync(id, watchId);
            if (existing is null) return NotFound();
            var (watch, problem) = FromDto(id, watchId, dto, existing.Secret);
            if (problem is not null) return BadRequest(problem);
            return await _repository.UpdateAsync(watch!) ? NoContent() : NotFound();
        }

        [HttpDelete("{watchId:int}")]
        public async Task<IActionResult> Delete(int watchId, [FromQuery] string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            return await _repository.DeleteAsync(id, watchId) ? NoContent() : NotFound();
        }

        public static (ChannelDecisionWatch? Watch, string? Problem) FromDto(ulong botId, int id, ChannelDecisionWatchDto dto, string? keptSecret)
        {
            if (!ulong.TryParse(dto.ChannelId, out var channelId)) return (null, "A valid channelId is required.");
            var url = (dto.ListenerUrl ?? "").Trim();
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https")) return (null, "listenerUrl is the Hive listener's http or https address.");
            ulong alertChannelId = 0;
            if (!string.IsNullOrWhiteSpace(dto.AlertChannelId) && !ulong.TryParse(dto.AlertChannelId, out alertChannelId)) return (null, "alertChannelId is a channel id, or left out for the bot's log channel.");
            var key = string.IsNullOrWhiteSpace(dto.QuestionKey) ? null : dto.QuestionKey.Trim();
            if (key is { Length: > 64 }) return (null, "questionKey is at most 64 characters.");
            var actions = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var (answer, action) in dto.Actions ?? [])
            {
                if (string.IsNullOrWhiteSpace(answer) || answer.Length > 64) return (null, "Every answer in actions is 1 to 64 characters.");
                if (!WatchActions.IsValid(action)) return (null, $"The action for '{answer}' is ignore, answer, alert or react:<emoji>.");
                actions[answer.Trim()] = action.Trim();
            }
            var unsure = string.IsNullOrWhiteSpace(dto.UnsureAction) ? WatchActions.Ignore : dto.UnsureAction.Trim();
            if (!WatchActions.IsValid(unsure)) return (null, "unsureAction is ignore, answer, alert or react:<emoji>.");
            return (new ChannelDecisionWatch
            {
                Id = id,
                BotId = botId,
                ChannelId = channelId,
                Name = string.IsNullOrWhiteSpace(dto.Name) ? "Decision watch" : dto.Name.Trim(),
                ListenerUrl = url,
                Secret = string.IsNullOrWhiteSpace(dto.Secret) ? keptSecret : dto.Secret.Trim(),
                QuestionKey = key,
                ActionsJson = JsonSerializer.Serialize(actions),
                UnsureAction = unsure,
                AlertChannelId = alertChannelId,
                Enabled = dto.Enabled,
            }, null);
        }

        private static ChannelDecisionWatchDto ToDto(ChannelDecisionWatch w) => new()
        {
            Id = w.Id,
            ChannelId = w.ChannelId.ToString(),
            Name = w.Name,
            ListenerUrl = w.ListenerUrl,
            HasSecret = !string.IsNullOrEmpty(w.Secret),
            Secret = null,
            QuestionKey = w.QuestionKey,
            Actions = DecisionWatchRules.Actions(w.ActionsJson).ToDictionary(p => p.Key, p => p.Value),
            UnsureAction = w.UnsureAction,
            AlertChannelId = w.AlertChannelId == 0 ? null : w.AlertChannelId.ToString(),
            Enabled = w.Enabled,
        };
    }

    public class ChannelDecisionWatchDto
    {
        public int Id { get; set; }
        public string ChannelId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ListenerUrl { get; set; } = string.Empty;
        public string? Secret { get; set; }
        public bool HasSecret { get; set; }
        public string? QuestionKey { get; set; }
        public Dictionary<string, string>? Actions { get; set; }
        public string? UnsureAction { get; set; }
        public string? AlertChannelId { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
