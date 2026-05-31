namespace AiryBotCode.HiveIntegration.Contracts;

// What the orchestrator hands back to the bot after one /agent round-trip.
public sealed class HiveTurnResult
{
    // Reply text to send back to Discord. Never null — empty string on error
    // and the bot decides whether to suppress.
    public string Text { get; init; } = "";

    public string SessionId { get; init; } = "";
    public string MindUserId { get; init; } = "";

    // True when Mind returned a normal response. False when the call failed
    // or the channel was not on the allow-list (Text will explain).
    public bool Ok { get; init; }

    // Set only when Ok = false.
    public string? Error { get; init; }

    public int DurationMs { get; init; }
}
