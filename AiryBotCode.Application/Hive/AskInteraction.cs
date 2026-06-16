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
        // The tool/effect name + the button-customId routing command (shared by
        // DiscordAskDelivery's encoder and ButtonPressHandler's decoder).
        public const string Command = "ask_user";

        public static AskIntent? Route(string? name, string? effectId, string? question, IReadOnlyList<string>? options, string? sessionId)
        {
            if (name != Command) return null;
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
}

// NB: the Discord button customId for an ask_user answer is built with the project's
// canonical ButtonEncriptionService (c:ask_user | a:<effectId>.<idx> | u:<askerId>) in
// DiscordAskDelivery, and decoded the same way in ButtonPressHandler — so button context
// (routing, correlation key, allowed user) is consistent with every other button. The
// chosen answer is the button's label, not the customId.
