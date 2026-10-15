using AiryBotCode.Api.Services;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/channels")]
    public class AiryChannelsController : ControllerBase
    {
        private readonly IAiryChannelRepository _channels;
        private readonly IBotSettingRepository _bots;
        private readonly DiscordGuildLookup _discord;

        public AiryChannelsController(IAiryChannelRepository channels, IBotSettingRepository bots, DiscordGuildLookup discord)
        {
            _channels = channels;
            _bots = bots;
            _discord = discord;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var (names, known) = await NamesAsync(id);
            return Ok((await _channels.GetForBotAsync(id)).Select(c => ToDto(c, names, known)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string botId, [FromBody] AiryChannelDto dto)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            if (!ulong.TryParse(dto.ChannelId, out var channelId)) return BadRequest("Pick a channel.");
            if (!AnswerModes.IsValid(dto.AnswerMode)) return BadRequest("answerMode is always, mention or decide.");
            if ((await _channels.GetForBotAsync(id)).Any(c => c.ChannelId == channelId)) return Conflict("Airy already talks in this channel.");
            var created = await _channels.AddAsync(new AiryChannel { BotId = id, ChannelId = channelId, AnswerMode = dto.AnswerMode!, Enabled = dto.Enabled });
            var (names, known) = await NamesAsync(id);
            return StatusCode(201, ToDto(created, names, known));
        }

        [HttpPut("{channelRowId:int}")]
        public async Task<IActionResult> Update(int channelRowId, [FromQuery] string botId, [FromBody] AiryChannelDto dto)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            if (!AnswerModes.IsValid(dto.AnswerMode)) return BadRequest("answerMode is always, mention or decide.");
            var ok = await _channels.UpdateAsync(new AiryChannel { Id = channelRowId, BotId = id, AnswerMode = dto.AnswerMode!, Enabled = dto.Enabled });
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{channelRowId:int}")]
        public async Task<IActionResult> Delete(int channelRowId, [FromQuery] string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            return await _channels.DeleteAsync(id, channelRowId) ? NoContent() : NotFound();
        }

        private async Task<(IReadOnlyDictionary<string, DiscordEntity> Names, bool Known)> NamesAsync(ulong botId)
        {
            var bot = await _bots.GetBotSettingAsync(botId);
            var (channels, known) = await _discord.GetChannelsKnownAsync(botId, bot?.Token);
            return (channels.GroupBy(c => c.Id).ToDictionary(g => g.Key, g => g.First()), known);
        }

        public static AiryChannelDto ToDto(AiryChannel c, IReadOnlyDictionary<string, DiscordEntity> names, bool namesKnown = true)
        {
            var key = c.ChannelId.ToString();
            names.TryGetValue(key, out var named);
            return new AiryChannelDto
            {
                Id = c.Id,
                ChannelId = key,
                ChannelName = named?.Name,
                Guild = named?.Guild,
                NamesKnown = namesKnown,
                AnswerMode = c.AnswerMode,
                Enabled = c.Enabled,
                LastDecisionAt = c.LastDecisionAt,
                LastDecision = c.LastDecision,
                LastDecisionError = c.LastDecisionError,
            };
        }
    }

    public class AiryChannelDto
    {
        public int Id { get; set; }
        public string ChannelId { get; set; } = string.Empty;
        public string? ChannelName { get; set; }
        public string? Guild { get; set; }
        public bool NamesKnown { get; set; } = true;
        public string? AnswerMode { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime? LastDecisionAt { get; set; }
        public string? LastDecision { get; set; }
        public string? LastDecisionError { get; set; }
    }
}
