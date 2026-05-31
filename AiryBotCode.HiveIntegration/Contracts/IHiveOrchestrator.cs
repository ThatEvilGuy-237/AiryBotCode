namespace AiryBotCode.HiveIntegration.Contracts;

// High-level entry point a Discord bot calls per incoming message.
//
// Implementation handles:
//   - Channel allow-list check
//   - Session resolution (channelId + speaker -> mindUserId + sessionId)
//   - System prompt composition (default prompt + per-turn speaker hint)
//   - Tool catalog fetch (from Wraith) when ToolingEnabled
//   - POST to Mind /agent
//   - Mapping the response into HiveTurnResult
public interface IHiveOrchestrator
{
    Task<HiveTurnResult> RespondAsync(
        ulong channelId,
        ulong discordUserId,
        string discordDisplayName,
        string userMessage,
        CancellationToken ct = default);
}
