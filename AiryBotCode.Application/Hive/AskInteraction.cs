namespace AiryBotCode.Application.Hive
{
    /// <summary>A resolved instruction to ask the user a question with option buttons.
    /// <c>EffectId</c> is the Hive tool-call id — it travels into each button's customId
    /// and back over the WS as the <c>effect_response</c> correlation key.</summary>
    public sealed record AskIntent(ulong ChannelId, string EffectId, string Question, IReadOnlyList<string> Options);

    /// <summary>
    /// Pure decision layer for the await-mode <c>ask_user</c> effect: validates the
    /// frame and turns it into a concrete "ask this in this channel" intent, or null.
    /// Mirrors <see cref="EffectRouter"/> but for the interactive (response-bearing)
    /// effect rather than fire-and-forget messages.
    /// </summary>
    public static class AskRouter
    {
        public const int MaxOptions = 5;

        public static AskIntent? Route(string? name, string? effectId, string? question, IReadOnlyList<string>? options, string? sessionId)
        {
            if (name != "ask_user") return null;
            if (string.IsNullOrWhiteSpace(effectId)) return null;
            if (string.IsNullOrWhiteSpace(question)) return null;
            if (!ulong.TryParse(sessionId, out var channelId)) return null;

            var opts = (options ?? Array.Empty<string>())
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim())
                .Distinct()
                .Take(MaxOptions)
                .ToList();
            if (opts.Count == 0) opts = new List<string> { "Yes", "No" };

            return new AskIntent(channelId, effectId!, question!.Trim(), opts);
        }
    }

    /// <summary>
    /// Builds/parses the Discord button customId for an <c>ask_user</c> answer.
    /// Shape: <c>effect:ask:&lt;effectId&gt;:&lt;answer&gt;</c>. The effectId carries no
    /// colon (Hive tool-call ids don't); the answer may, so parsing keeps everything
    /// after the third colon. Whole id is capped at Discord's 100-char customId limit
    /// by trimming the answer (the effectId is preserved — it's the routing key).
    /// </summary>
    public static class AskInteraction
    {
        public const string Prefix = "effect:ask";
        private const int MaxCustomId = 100;

        public static string BuildAnswerId(string effectId, string answer)
        {
            var head = $"{Prefix}:{effectId}:";
            var budget = Math.Max(0, MaxCustomId - head.Length);
            var trimmed = answer.Length > budget ? answer[..budget] : answer;
            return head + trimmed;
        }

        public static (string EffectId, string Answer)? TryParseAnswerId(string? customId)
        {
            if (string.IsNullOrEmpty(customId)) return null;
            var parts = customId.Split(':', 4);
            if (parts.Length != 4 || $"{parts[0]}:{parts[1]}" != Prefix) return null;
            if (string.IsNullOrEmpty(parts[2])) return null;
            return (parts[2], parts[3]);
        }
    }
}
