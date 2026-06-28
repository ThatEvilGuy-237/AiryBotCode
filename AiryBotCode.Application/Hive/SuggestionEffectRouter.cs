namespace AiryBotCode.Application.Hive
{
    /// <summary>A resolved instruction to file one suggestion on the board.</summary>
    public sealed record SuggestionIntent(string EffectId, string Title, string Body, string? SubmittedBy);

    /// <summary>
    /// Pure decision layer for the Hive <c>submit_suggestion</c> effect: the agent's
    /// in-process dispatch tool fans this out over the tools WS when a user proposes a
    /// feature/idea. Validates the payload and produces an intent to write a row on the
    /// /suggestions board — or null when it isn't a well-formed suggestion.
    ///
    /// Unlike <see cref="EffectRouter"/> this carries no channel: a suggestion is
    /// project-wide (not posted back to Discord), so there's no <c>sessionId</c> to parse.
    /// </summary>
    public static class SuggestionEffectRouter
    {
        public const string Command = "submit_suggestion";
        private const int MaxTitle = 200;

        public static SuggestionIntent? Route(string? name, string? effectId, string? title, string? body, string? submittedBy)
        {
            if (name != Command) return null;
            if (string.IsNullOrWhiteSpace(effectId)) return null;
            if (string.IsNullOrWhiteSpace(title)) return null;
            if (string.IsNullOrWhiteSpace(body)) return null;

            var t = title.Trim();
            if (t.Length > MaxTitle) t = t[..MaxTitle];

            var by = string.IsNullOrWhiteSpace(submittedBy) ? null : submittedBy.Trim();
            return new SuggestionIntent(effectId!.Trim(), t, body.Trim(), by);
        }
    }
}
