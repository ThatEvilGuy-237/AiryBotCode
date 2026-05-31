namespace AiryBotCode.HiveIntegration.Contracts;

// Configuration bound from the "Hive" section of appsettings.
//
// Example:
//   "Hive": {
//     "MindUrl":   "http://localhost:3000",
//     "WraithUrl": "http://localhost:5000",
//     "Provider":  "openai",
//     "Model":     "gpt-4o-mini",
//     "SystemPrompt": "You are Airy, a Discord assistant. ...",
//     "Timeout":   "2m",
//     "Temperature": 0.7,
//     "MaxTokens":   600,
//     "EnabledChannels": [1182267222152982533, ...],
//     "ToolingEnabled": true,
//     "ReasoningEnabled": true,
//     "UserIdPrefix": "discord:",
//     "MindUserIdForServerWide": null
//   }
public sealed class HiveAgentOptions
{
    public const string SectionName = "Hive";

    // Mind (Obsidian-Mind) base URL. The agent endpoint is appended.
    public string MindUrl { get; set; } = "http://localhost:3000";

    // Wraith-Worker base URL. Used to fetch the live MCP tool catalog per turn.
    // Set to null/empty to disable tool injection.
    public string? WraithUrl { get; set; } = "http://localhost:5000";

    // LLM provider + model passed straight through to Mind.
    public string Provider { get; set; } = "openai";
    public string Model    { get; set; } = "gpt-4o-mini";

    // Default system prompt for the bot. Mind / Chronos still inject
    // [USER PROFILE] / [MEMORY] / [AGENT PROFILE] on top per turn.
    public string SystemPrompt { get; set; } = "";

    // Mind timeout duration (e.g. "1m", "2m 30s"). Parsed by Mind.
    public string Timeout { get; set; } = "2m";

    public double Temperature { get; set; } = 0.7;
    public int    MaxTokens   { get; set; } = 600;

    public bool ToolingEnabled   { get; set; } = true;
    public bool ReasoningEnabled { get; set; } = true;

    // Optional allow-list. When non-empty, only messages in these channels
    // are forwarded to Mind. Empty list = forward from any channel the bot
    // is mentioned in.
    public List<ulong> EnabledChannels { get; set; } = new();

    // Prefix prepended to Discord user IDs to form Mind userId.
    // Default `discord:` keeps Discord identities namespaced cleanly.
    public string UserIdPrefix { get; set; } = "discord:";

    // HttpClient timeout for the Mind call. Should be larger than the Mind
    // request timeout so we get Mind's error message rather than aborting on
    // the client side.
    public int HttpTimeoutSeconds { get; set; } = 180;
}
