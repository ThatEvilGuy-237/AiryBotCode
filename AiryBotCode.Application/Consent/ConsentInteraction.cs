namespace AiryBotCode.Application.Consent
{
    /// <summary>
    /// Pure helpers for the first-message consent gate: the Accept button's
    /// customId (carries bot+user so the handler knows whose consent to record)
    /// and the notice copy. Discord-free so it's unit-testable.
    /// </summary>
    public static class ConsentInteraction
    {
        public const string Prefix = "consent:accept";

        public static string BuildAcceptId(ulong botId, ulong userId) => $"{Prefix}:{botId}:{userId}";

        /// <summary>Parse an Accept customId back to (botId, userId); null if it isn't ours.</summary>
        public static (ulong BotId, ulong UserId)? TryParseAcceptId(string? customId)
        {
            if (string.IsNullOrEmpty(customId)) return null;
            var parts = customId.Split(':');
            // ["consent","accept",botId,userId]
            if (parts.Length != 4 || $"{parts[0]}:{parts[1]}" != Prefix) return null;
            if (!ulong.TryParse(parts[2], out var botId) || !ulong.TryParse(parts[3], out var userId)) return null;
            return (botId, userId);
        }

        /// <summary>The notice shown on first contact. <paramref name="evilId"/> is the
        /// owner to ping for questions (0 → omit the ping line).</summary>
        public static string PromptText(ulong evilId)
        {
            var ping = evilId != 0 ? $" Questions? Ping <@{evilId}>." : "";
            return "👋 Before Airy replies — a quick heads-up: your messages are stored "
                + "**only to improve your experience with Airy**, and are **never used for any "
                + "AI training**. Press **Accept** to continue." + ping;
        }
    }
}
