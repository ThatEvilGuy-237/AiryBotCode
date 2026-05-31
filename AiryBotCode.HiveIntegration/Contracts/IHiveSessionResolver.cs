namespace AiryBotCode.HiveIntegration.Contracts;

// Maps Discord identifiers to a stable (mindUserId, sessionId) pair.
//
// Per-user-per-channel strategy by default: every Discord user gets their own
// thread with the bot inside each channel they speak to it in. Session ids
// are minted once and stored on the bot side so repeated turns reuse the
// same conversation in Mind.
//
// Swap implementation if a different strategy is wanted (one session per
// channel, one session per guild, etc.).
public interface IHiveSessionResolver
{
    Task<HiveSession> ResolveAsync(ulong channelId, ulong discordUserId, CancellationToken ct = default);
}

public sealed record HiveSession(string MindUserId, string SessionId);
