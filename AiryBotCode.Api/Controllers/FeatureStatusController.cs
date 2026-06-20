using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiryBotCode.Application.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    // Read-only views of live feature state for the panel: the counting game's
    // per-channel progress and the leveling XP leaderboard. Nothing here mutates
    // the bot — it just surfaces what the running bot has already written to the DB.
    // Snowflake ids are serialized as STRINGS (precision-safe; the panel never
    // JSON.parse/Number's them).
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FeatureStatusController : ControllerBase
    {
        private readonly ICountingStateRepository _counting;
        private readonly ILevelUserRepository _leveling;
        private readonly IBotSettingRepository _bots;
        private readonly AiryBotCode.Api.Services.DiscordGuildLookup _discord;

        public FeatureStatusController(
            ICountingStateRepository counting,
            ILevelUserRepository leveling,
            IBotSettingRepository bots,
            AiryBotCode.Api.Services.DiscordGuildLookup discord)
        {
            _counting = counting;
            _leveling = leveling;
            _bots = bots;
            _discord = discord;
        }

        // GET /api/featurestatus/{botId}/counting -> every counting channel for the
        // bot (current count, high score, whether a boss is pending), channel names
        // resolved from Discord when the token allows it (falls back to the raw id).
        [HttpGet("{botId}/counting")]
        public async Task<IActionResult> Counting(string botId)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");

            var states = await _counting.ListByBotAsync(id);

            // Resolve channel names best-effort; the lookup never throws and returns
            // an empty list on any failure, so a missing token just yields raw ids.
            var entity = await _bots.GetBotSettingAsync(id);
            var channels = await _discord.GetChannelsAsync(id, entity?.Token);
            var nameById = channels.ToDictionary(c => c.Id, c => c.Name);

            var dto = states.Select(s => new
            {
                channelId = s.ChannelId.ToString(),
                channelName = nameById.TryGetValue(s.ChannelId.ToString(), out var n) ? n : null,
                currentCount = s.CurrentCount,
                highScore = s.HighScore,
                bossActive = s.BossActive,
                bossSpawnedAt = s.BossSpawnedAt,
                lastUserId = s.LastUserId == 0 ? null : s.LastUserId.ToString(),
                updatedAt = s.UpdatedAt,
            });

            return Ok(dto);
        }

        // GET /api/featurestatus/{botId}/leveling?skip=&take=  -> the XP leaderboard
        // (highest XP first) plus the total number of ranked users. take is clamped
        // to 1..100; skip is non-negative.
        [HttpGet("{botId}/leveling")]
        public async Task<IActionResult> Leveling(string botId, int skip = 0, int take = 25)
        {
            if (!ulong.TryParse(botId, out var id)) return BadRequest("A valid botId is required.");
            if (skip < 0) skip = 0;
            take = Math.Clamp(take, 1, 100);

            var total = await _leveling.CountAsync(id);
            var top = await _leveling.TopAsync(id, skip, take);

            var rows = top.Select((u, i) => new
            {
                rank = skip + i + 1,
                userId = u.UserId.ToString(),
                userName = u.UserName,
                xp = u.Xp,
                level = u.Level,
                lastXpAt = u.LastXpAt,
            });

            return Ok(new { total, skip, take, users = rows });
        }
    }
}
