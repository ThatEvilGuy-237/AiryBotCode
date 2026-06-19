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

    // Body for setting a bot's theme palette + (optional) server-side image theme.
    public class ThemeRequest
    {
        public string Primary { get; set; }
        public string Accent { get; set; }
        // Downscaled data-URL of the chosen image (also the bot profile pic).
        public string Image { get; set; }
        // The derived @hive/ui theme as JSON (accent L/C/hue + base hue/chroma).
        public string Data { get; set; }
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
        private readonly ICommandSettingsRepository _control;
        private readonly AiryBotCode.Api.Services.DiscordGuildLookup _discord;

        public SettingsController(IBotSettingRepository repository, IConfiguration configuration, ICommandSettingsRepository control, AiryBotCode.Api.Services.DiscordGuildLookup discord)
        {
            _repository = repository;
            _configuration = configuration;
            _control = control;
            _discord = discord;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _repository.GetAllAsync();
            return Ok(settings.Select(ToDto));
        }

        // GET /api/settings/{botId}/discord/channels -> postable channels for the
        // panel's channel picker. Empty list on any failure (UI falls back to a raw
        // id field), so a missing/invalid token never breaks the settings page.
        [HttpGet("{botId}/discord/channels")]
        public async Task<IActionResult> GetDiscordChannels(string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var entity = await _repository.GetBotSettingAsync(id);
            return Ok(await _discord.GetChannelsAsync(id, entity?.Token));
        }

        // GET /api/settings/{botId}/discord/categories -> channel categories (for the
        // category picker, e.g. where contact channels are created).
        [HttpGet("{botId}/discord/categories")]
        public async Task<IActionResult> GetDiscordCategories(string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var entity = await _repository.GetBotSettingAsync(id);
            return Ok(await _discord.GetCategoriesAsync(id, entity?.Token));
        }

        // GET /api/settings/{botId}/discord/roles -> assignable roles for the role picker.
        [HttpGet("{botId}/discord/roles")]
        public async Task<IActionResult> GetDiscordRoles(string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            var entity = await _repository.GetBotSettingAsync(id);
            return Ok(await _discord.GetRolesAsync(id, entity?.Token));
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

        // PUT /api/settings/{botId}/theme -> set the bot's theme palette (panel skin
        // + brand embed color). Kept separate so the general settings save can't wipe it.
        [HttpPut("{botId}/theme")]
        public async Task<IActionResult> SetTheme(string botId, [FromBody] ThemeRequest body)
        {
            if (!ulong.TryParse(botId, out var id))
            {
                return BadRequest("Invalid bot id.");
            }
            var entity = await _repository.GetBotSettingAsync(id);
            if (entity == null) return NotFound();

            entity.ThemePrimary = NormalizeHex(body?.Primary);
            entity.ThemeAccent = NormalizeHex(body?.Accent);
            // Server-side image theme (so it follows across devices). The frontend
            // downscales the image before sending; cap defensively so a bot row
            // can't be bloated. Empty image clears the stored theme.
            entity.ThemeImage = NormalizeImage(body?.Image);
            entity.ThemeData = string.IsNullOrWhiteSpace(body?.Data) ? null : body.Data;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return Ok(ToDto(entity));
        }

        // GET /api/settings/{botId}/theme -> the bot's full theme (palette + the
        // server-stored image + derived theme JSON). Kept off the bot-list DTO so
        // listing bots stays lean; fetched per-bot on demand.
        [HttpGet("{botId}/theme")]
        public async Task<IActionResult> GetTheme(string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("Invalid bot id.");
            var entity = await _repository.GetBotSettingAsync(id);
            if (entity == null) return NotFound();
            return Ok(new
            {
                primary = entity.ThemePrimary,
                accent = entity.ThemeAccent,
                image = entity.ThemeImage,
                data = entity.ThemeData,
            });
        }

        // POST /api/settings/{botId}/avatar -> ask the bot to set its Discord avatar
        // from the saved theme image. Signals the bot via a control row (it reads
        // BotSettings.ThemeImage and calls Discord ModifyAsync). Discord rate-limits
        // avatar changes to ~2/hour, so this is a deliberate, user-triggered action.
        [HttpPost("{botId}/avatar")]
        public async Task<IActionResult> SetBotAvatar(string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("Invalid bot id.");
            var entity = await _repository.GetBotSettingAsync(id);
            if (entity == null) return NotFound();
            if (string.IsNullOrWhiteSpace(entity.ThemeImage))
                return BadRequest("No theme image saved for this bot — upload + Save an image on the Theme page first.");
            // Bump a per-bot control row; the bot's watcher picks it up within ~5s.
            await _control.SetControlValueAsync("avatar:" + id, DateTime.UtcNow.ToString("O"));
            return Accepted(new { requested = true });
        }

        // Accept a data: image URL up to ~512KB; anything else (oversized / not a
        // data URL) -> null so a malformed/huge payload can't bloat the row.
        private static string NormalizeImage(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var v = value.Trim();
            if (!v.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase)) return null;
            return v.Length > 512 * 1024 ? null : v;
        }

        // Accept "#rrggbb" / "rrggbb" (case-insensitive); anything else -> null.
        private static string NormalizeHex(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var v = value.Trim().TrimStart('#');
            if (v.Length != 6 || !v.All(Uri.IsHexDigit)) return null;
            return "#" + v.ToLowerInvariant();
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
            entity.AdminRoleIds = dto.AdminRoleIds;
            entity.DatabaseName = string.IsNullOrWhiteSpace(dto.DatabaseName) ? null : dto.DatabaseName.Trim();

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
            AdminRoleIds = b.AdminRoleIds,
            EvilId = b.EvilId.ToString(),
            LogChannelId = b.LogChannelId.ToString(),
            EvilLogChannelId = b.EvilLogChannelId.ToString(),
            HasToken = !string.IsNullOrEmpty(b.Token),
            Token = null,
            DatabaseName = b.DatabaseName,
            ThemePrimary = b.ThemePrimary,
            ThemeAccent = b.ThemeAccent,
        };
    }
}
