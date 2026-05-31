using AiryBotCode.HiveIntegration.Contracts;
using Microsoft.Extensions.Options;

namespace AiryBotCode.HiveIntegration.Sessions;

// Deterministic, stateless resolver. No DB.
//
// We derive both ids from (channelId, discordUserId) so the same user in the
// same channel always gets the same Mind sessionId across restarts. The
// userId is namespaced with the configured prefix ("discord:" by default)
// so cross-source identity collisions are impossible — a Slack `discord:123`
// would just never appear.
//
// Trade-off: there is no operator-driven "reset chat in this channel" yet.
// When we want that, swap in a DB-backed implementation that mints a fresh
// sessionId on demand and stores it.
public sealed class HiveSessionResolver : IHiveSessionResolver
{
    private readonly HiveAgentOptions _options;

    public HiveSessionResolver(IOptions<HiveAgentOptions> options)
    {
        _options = options.Value;
    }

    public Task<HiveSession> ResolveAsync(ulong channelId, ulong discordUserId, CancellationToken ct = default)
    {
        var prefix = _options.UserIdPrefix ?? "discord:";
        var mindUserId = $"{prefix}{discordUserId}";
        // sessionId is per-(channel, user). One thread of conversation per
        // person inside each channel; cross-channel memory still flows via
        // the shared userId in Chronos.
        var sessionId = $"discord-c{channelId}-u{discordUserId}";

        return Task.FromResult(new HiveSession(mindUserId, sessionId));
    }
}
