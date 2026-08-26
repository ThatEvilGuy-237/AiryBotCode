using AiryBotCode.SlackBot.Spine;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

namespace AiryBotCode.SlackBot.SlackEvents;

/// <summary>
/// Bridges Slack messages to the Vinnie agent. Responds in DMs and to @mentions in
/// channels, replying in-thread. sessionId = thread (or channel) so Chronos memory
/// is conversation-scoped; userId = the Slack user.
/// </summary>
public sealed class VinnieHandler : IEventHandler<MessageEvent>
{
    private readonly ISlackApiClient _slack;
    private readonly SpineAgentClient _spine;
    private readonly string _botUserId;

    public VinnieHandler(ISlackApiClient slack, SpineAgentClient spine, string botUserId)
    {
        _slack = slack;
        _spine = spine;
        _botUserId = botUserId;
    }

    public async Task Handle(MessageEvent ev)
    {
        // Ignore bots / our own messages / edits, joins, and other subtype events.
        if (!string.IsNullOrEmpty(ev.BotId)) return;
        if (ev.User == _botUserId) return;
        if (!string.IsNullOrEmpty(ev.Subtype)) return;
        if (string.IsNullOrWhiteSpace(ev.Text)) return;

        // Only engage in DMs, or in channels when explicitly @mentioned.
        var isDm = ev.ChannelType == "im";
        var mentionsBot = ev.Text.Contains($"<@{_botUserId}>", StringComparison.Ordinal);
        if (!isDm && !mentionsBot) return;

        var prompt = ev.Text.Replace($"<@{_botUserId}>", string.Empty, StringComparison.Ordinal).Trim();
        if (prompt.Length == 0) prompt = "Hi";

        // Per-thread memory when in a thread, else per-channel.
        var sessionId = !string.IsNullOrEmpty(ev.ThreadTs) ? ev.ThreadTs : ev.Channel ?? string.Empty;
        var userId = ev.User ?? string.Empty;

        string reply;
        try
        {
            reply = await _spine.AskAsync(prompt, sessionId, userId);
        }
        catch (Exception e)
        {
            reply = $":warning: Vinnie hit an error: {e.Message}";
        }
        if (string.IsNullOrWhiteSpace(reply)) reply = "_(no reply)_";

        // Slack's per-message text cap is ~40k but UX-wise keep chunks readable.
        foreach (var chunk in Chunk(reply, 3500))
        {
            await _slack.Chat.PostMessage(new Message
            {
                Channel = ev.Channel,
                Text = chunk,
                ThreadTs = ev.ThreadTs, // reply in-thread when the trigger was threaded
            });
        }
    }

    private static IEnumerable<string> Chunk(string s, int max)
    {
        if (s.Length <= max) { yield return s; yield break; }
        var i = 0;
        while (i < s.Length)
        {
            var len = Math.Min(max, s.Length - i);
            // try to break on a newline within the window for cleaner splits
            if (i + len < s.Length)
            {
                var nl = s.LastIndexOf('\n', i + len - 1, len);
                if (nl > i) len = nl - i + 1;
            }
            yield return s.Substring(i, len);
            i += len;
        }
    }
}
