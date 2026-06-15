using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    /// <summary>
    /// Manage a bot's channel → webhook links (the "chat" routing). Messages in a
    /// linked channel are forwarded to its webhook (e.g. a Hive trigger). Per bot
    /// via the required <c>botId</c>. Requires a JWT.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelWebhooksController : ControllerBase
    {
        private readonly IChannelWebhookRepository _repository;

        public ChannelWebhooksController(IChannelWebhookRepository repository)
        {
            _repository = repository;
        }

        // GET /api/channelwebhooks?botId=123
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var links = await _repository.GetForBotAsync(id);
            return Ok(links.Select(ToDto));
        }

        // POST /api/channelwebhooks?botId=123
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string botId, [FromBody] ChannelWebhookDto dto)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            if (!ulong.TryParse(dto.ChannelId, out var channelId)) return BadRequest("A valid channelId is required.");
            if (string.IsNullOrWhiteSpace(dto.WebhookUrl)) return BadRequest("webhookUrl is required.");

            var created = await _repository.AddAsync(new ChannelWebhook
            {
                BotId = id,
                ChannelId = channelId,
                Name = string.IsNullOrWhiteSpace(dto.Name) ? "Webhook" : dto.Name,
                WebhookUrl = dto.WebhookUrl.Trim(),
                Secret = string.IsNullOrWhiteSpace(dto.Secret) ? null : dto.Secret,
                Mode = dto.Mode == "async" ? "async" : "sync",
                Enabled = dto.Enabled,
            });
            return StatusCode(201, ToDto(created));
        }

        // PUT /api/channelwebhooks/{id}?botId=123
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromQuery] string botId, [FromBody] ChannelWebhookDto dto)
        {
            if (!ulong.TryParse(botId, out var bid)) return BadRequest("A valid botId is required.");
            if (!ulong.TryParse(dto.ChannelId, out var channelId)) return BadRequest("A valid channelId is required.");

            var ok = await _repository.UpdateAsync(new ChannelWebhook
            {
                Id = id,
                BotId = bid,
                ChannelId = channelId,
                Name = string.IsNullOrWhiteSpace(dto.Name) ? "Webhook" : dto.Name,
                WebhookUrl = (dto.WebhookUrl ?? string.Empty).Trim(),
                Secret = string.IsNullOrWhiteSpace(dto.Secret) ? null : dto.Secret,
                Mode = dto.Mode == "async" ? "async" : "sync",
                Enabled = dto.Enabled,
            });
            return ok ? NoContent() : NotFound();
        }

        // DELETE /api/channelwebhooks/{id}?botId=123
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string botId)
        {
            if (!ulong.TryParse(botId, out var bid)) return BadRequest("A valid botId is required.");
            return await _repository.DeleteAsync(bid, id) ? NoContent() : NotFound();
        }

        private static ChannelWebhookDto ToDto(ChannelWebhook c) => new()
        {
            Id = c.Id,
            ChannelId = c.ChannelId.ToString(),
            Name = c.Name,
            WebhookUrl = c.WebhookUrl,
            HasSecret = !string.IsNullOrEmpty(c.Secret),
            Secret = null,
            Mode = c.Mode,
            Enabled = c.Enabled,
        };
    }

    public class ChannelWebhookDto
    {
        public int Id { get; set; }
        public string ChannelId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string WebhookUrl { get; set; } = string.Empty;
        // Write-only on input; never returned (HasSecret flags presence).
        public string? Secret { get; set; }
        public bool HasSecret { get; set; }
        public string Mode { get; set; } = "sync";
        public bool Enabled { get; set; } = true;
    }
}
