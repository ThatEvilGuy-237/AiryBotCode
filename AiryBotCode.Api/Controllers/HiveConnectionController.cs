using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/settings/{botId}/hive")]
    public class HiveConnectionController : ControllerBase
    {
        private readonly IBotSettingRepository _bots;
        private readonly HiveAnswerClient _hive;

        public HiveConnectionController(IBotSettingRepository bots, HiveAnswerClient hive)
        {
            _bots = bots;
            _hive = hive;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var bot = await _bots.GetBotSettingAsync(id);
            if (bot is null) return NotFound();
            return Ok(new HiveConnectionDto
            {
                Url = bot.HiveUrl,
                AgentId = bot.HiveAgentId,
                HasKey = !string.IsNullOrWhiteSpace(bot.HiveAppKey),
                KeyHint = Hint(bot.HiveAppKey),
            });
        }

        [HttpPut]
        public async Task<IActionResult> Put(string botId, [FromBody] HiveConnectionDto dto)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var bot = await _bots.GetBotSettingAsync(id);
            if (bot is null) return NotFound();
            if (!HiveConnection.IsHiveUrl(dto.Url)) return BadRequest("url is the Hive's http or https address.");
            if (!Guid.TryParse(dto.AgentId, out var agent)) return BadRequest("Pick the agent Airy speaks as.");
            var key = string.IsNullOrWhiteSpace(dto.AppKey) ? bot.HiveAppKey : dto.AppKey.Trim();
            if (string.IsNullOrWhiteSpace(key)) return BadRequest("appKey is the Hive app key Airy uses.");
            bot.HiveUrl = HiveConnection.NormalUrl(dto.Url!);
            bot.HiveAgentId = agent.ToString();
            bot.HiveAppKey = key;
            await _bots.UpdateAsync(bot);
            await _bots.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var bot = await _bots.GetBotSettingAsync(id);
            if (bot is null) return NotFound();
            bot.HiveUrl = null;
            bot.HiveAgentId = null;
            bot.HiveAppKey = null;
            await _bots.UpdateAsync(bot);
            await _bots.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("agents")]
        public async Task<IActionResult> Agents(string botId, [FromBody] HiveConnectionDto dto)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var bot = await _bots.GetBotSettingAsync(id);
            var url = string.IsNullOrWhiteSpace(dto.Url) ? bot?.HiveUrl : dto.Url;
            var key = string.IsNullOrWhiteSpace(dto.AppKey) ? bot?.HiveAppKey : dto.AppKey.Trim();
            if (!HiveConnection.IsHiveUrl(url) || string.IsNullOrWhiteSpace(key)) return BadRequest("An address and a key are needed to list agents.");
            var agents = await _hive.AgentsAsync(url!, key!);
            return agents is null ? StatusCode(502, HiveTexts.Say("cannot run")) : Ok(agents);
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check(string botId, [FromBody] HiveConnectionDto? dto)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var bot = await _bots.GetBotSettingAsync(id);
            var url = string.IsNullOrWhiteSpace(dto?.Url) ? bot?.HiveUrl : dto.Url;
            var key = string.IsNullOrWhiteSpace(dto?.AppKey) ? bot?.HiveAppKey : dto.AppKey.Trim();
            var agent = string.IsNullOrWhiteSpace(dto?.AgentId) ? bot?.HiveAgentId : dto.AgentId;
            if (!HiveConnection.IsHiveUrl(url) || string.IsNullOrWhiteSpace(key)) return BadRequest("An address and a key are needed to check.");
            return Ok(await _hive.CheckAsync(url!, key!, agent));
        }

        [HttpPost("try")]
        public async Task<IActionResult> Try(string botId, [FromBody] HiveTryDto dto)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            if (string.IsNullOrWhiteSpace(dto.Message)) return BadRequest("Write a message to try.");
            var bot = await _bots.GetBotSettingAsync(id);
            if (bot is null) return NotFound();
            var hive = HiveConnection.Of(bot);
            if (hive is null) return BadRequest("Connect the Hive first.");
            var channel = string.IsNullOrWhiteSpace(dto.Channel) ? "general" : dto.Channel.Trim().TrimStart('#');
            var verdict = await _hive.AskAsync(hive, new WorthEvent(bot.BotName ?? "Airy", channel, "someone", 0, dto.Message.Trim(), []));
            return Ok(new { verdict.Yes, verdict.Sure, verdict.Confidence, verdict.DecisionId, verdict.Error, verdict.Summary, Answers = verdict.SaysAnswer });
        }

        public static string? Hint(string? key) =>
            string.IsNullOrWhiteSpace(key) ? null : key.Length <= 12 ? "…" : key[..12] + "…";
    }

    public class HiveTryDto
    {
        public string? Message { get; set; }
        public string? Channel { get; set; }
    }

    public class HiveConnectionDto
    {
        public string? Url { get; set; }
        public string? AgentId { get; set; }
        public string? AppKey { get; set; }
        public bool HasKey { get; set; }
        public string? KeyHint { get; set; }
    }
}
