namespace AiryBotCode.HiveIntegration.Contracts;

// Low-level transport for the Mind /agent endpoint. One method, one HTTP call.
public interface IHiveAgentClient
{
    Task<HiveAgentResponse> SendAsync(HiveAgentRequest request, CancellationToken ct = default);
}

// JSON shape the HTTP client builds. Names match what Mind's Elysia validator
// expects — see Obsidian-Mind/src/routes/agentRoute.ts.
public sealed class HiveAgentRequest
{
    public required string  MindUrl       { get; init; }
    public required string  Provider      { get; init; }
    public required string  Model         { get; init; }
    public required string  SessionId     { get; init; }
    public required string  UserId        { get; init; }
    public required string  UserMessage   { get; init; }
    public required string  SystemPrompt  { get; init; }

    public bool   Tooling     { get; init; } = true;
    public bool   Reasoning   { get; init; } = true;
    public string Timeout     { get; init; } = "2m";
    public double Temperature { get; init; } = 0.7;
    public int    MaxTokens   { get; init; } = 600;

    // Optional tool catalog (pass-through to Mind). Each entry is a JSON-ready
    // object with { name, description?, parameters }. We treat them as opaque
    // so the bot doesn't have to know about MCP schema details.
    public IReadOnlyList<object>? Tools { get; init; }

    // Optional prior history. Each entry is { role, content }. Mind keeps
    // server-side state per session, but sending history mirrors what the
    // test-ui does — useful when we want to be explicit.
    public IReadOnlyList<object>? History { get; init; }
}

public sealed class HiveAgentResponse
{
    public string Content { get; init; } = "";
    public int    DurationMs { get; init; }
}
