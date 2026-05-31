using System.Diagnostics;
using AiryBotCode.HiveIntegration.Contracts;
using AiryBotCode.HiveIntegration.Prompting;
using AiryBotCode.HiveIntegration.Tools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiryBotCode.HiveIntegration.Orchestration;

// The one entry point the Discord bot calls per incoming message. Wraps
// session resolution + prompt compose + tool fetch + Mind round-trip and
// hands back a single HiveTurnResult.
public sealed class HiveOrchestrator : IHiveOrchestrator
{
    private readonly HiveAgentOptions _options;
    private readonly IHiveSessionResolver _sessions;
    private readonly HivePromptComposer _composer;
    private readonly WraithToolFetcher _toolFetcher;
    private readonly IHiveAgentClient _agentClient;
    private readonly ILogger<HiveOrchestrator> _logger;

    public HiveOrchestrator(
        IOptions<HiveAgentOptions> options,
        IHiveSessionResolver sessions,
        HivePromptComposer composer,
        WraithToolFetcher toolFetcher,
        IHiveAgentClient agentClient,
        ILogger<HiveOrchestrator> logger)
    {
        _options     = options.Value;
        _sessions    = sessions;
        _composer    = composer;
        _toolFetcher = toolFetcher;
        _agentClient = agentClient;
        _logger      = logger;
    }

    public async Task<HiveTurnResult> RespondAsync(
        ulong channelId,
        ulong discordUserId,
        string discordDisplayName,
        string userMessage,
        CancellationToken ct = default)
    {
        // Allow-list short-circuit. Empty list = no restriction.
        if (_options.EnabledChannels is { Count: > 0 } && !_options.EnabledChannels.Contains(channelId))
        {
            return new HiveTurnResult
            {
                Ok = false,
                Error = "channel_not_enabled",
                Text = ""
            };
        }

        var sw = Stopwatch.StartNew();
        var session = await _sessions.ResolveAsync(channelId, discordUserId, ct);

        var systemPrompt = _composer.Compose(_options.SystemPrompt, discordUserId, discordDisplayName);

        IReadOnlyList<object>? tools = null;
        if (_options.ToolingEnabled)
            tools = await _toolFetcher.GetToolsAsync(_options.WraithUrl, ct);

        var req = new HiveAgentRequest
        {
            MindUrl      = _options.MindUrl,
            Provider     = _options.Provider,
            Model        = _options.Model,
            SessionId    = session.SessionId,
            UserId       = session.MindUserId,
            UserMessage  = userMessage,
            SystemPrompt = systemPrompt,
            Tooling      = _options.ToolingEnabled,
            Reasoning    = _options.ReasoningEnabled,
            Timeout      = _options.Timeout,
            Temperature  = _options.Temperature,
            MaxTokens    = _options.MaxTokens,
            Tools        = tools
        };

        try
        {
            var resp = await _agentClient.SendAsync(req, ct);
            return new HiveTurnResult
            {
                Ok         = true,
                Text       = resp.Content,
                SessionId  = session.SessionId,
                MindUserId = session.MindUserId,
                DurationMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Hive] Mind /agent failed for channel={Channel} user={User}.",
                channelId, discordUserId);
            return new HiveTurnResult
            {
                Ok         = false,
                Error      = ex.Message,
                Text       = "",
                SessionId  = session.SessionId,
                MindUserId = session.MindUserId,
                DurationMs = (int)sw.ElapsedMilliseconds
            };
        }
    }
}
