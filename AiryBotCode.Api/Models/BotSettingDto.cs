using AiryBotCode.Domain.database;

namespace AiryBotCode.Api.Models
{
    /// <summary>
    /// Transport shape for <see cref="BotSetting"/> used by the control-panel UI.
    ///
    /// Every Discord snowflake id is exposed as a <see cref="string"/> on purpose:
    /// snowflakes are 64-bit and exceed JavaScript's safe-integer range (2^53),
    /// so sending them as JSON numbers would silently corrupt them in the browser.
    /// The controller maps strings back to <c>ulong</c> on the way in.
    ///
    /// The bot <see cref="Token"/> is write-only: it is never sent to the client
    /// (<see cref="HasToken"/> just signals whether one is stored) and is only
    /// applied on save when a non-empty value is supplied.
    /// </summary>
    public class BotSettingDto
    {
        public string BotId { get; set; } = string.Empty;
        public string BotName { get; set; } = string.Empty;
        public bool Enabled { get; set; }

        // AI
        public string OpenAIModel { get; set; } = string.Empty;
        public string OpenAIPrompt { get; set; } = string.Empty;
        public int MaxTokens { get; set; }
        public int RetryAttempts { get; set; }

        // Owner / identity
        public string OwnerId { get; set; } = string.Empty;
        public string EvilId { get; set; } = string.Empty;

        // Channels
        public string LogChannelId { get; set; } = string.Empty;
        public string EvilLogChannelId { get; set; } = string.Empty;
        public string VerifyLogChannelId { get; set; } = string.Empty;
        public string RulesChannelId { get; set; } = string.Empty;
        public string GiveawayScoreboardChannelId { get; set; } = string.Empty;

        /// <summary>Comma-separated channel ids Airy listens/responds in.</summary>
        public string ListenChannelIds { get; set; } = string.Empty;

        // Roles
        /// <summary>Comma-separated role ids allowed to run admin commands.</summary>
        public string AdminRoleIds { get; set; } = string.Empty;
        public string VerifiedRoleId { get; set; } = string.Empty;
        public string UnverifiedRoleId { get; set; } = string.Empty;

        // Categories
        public string ContactCategoryId { get; set; } = string.Empty;

        /// <summary>True when a bot token is stored. The token itself is never returned.</summary>
        public bool HasToken { get; set; }

        /// <summary>Write-only. When non-empty on save, replaces the stored token.</summary>
        public string? Token { get; set; }

        /// <summary>Project a persisted setting into the client shape (token stripped).</summary>
        public static BotSettingDto FromEntity(BotSetting s) => new()
        {
            BotId = s.BotId.ToString(),
            BotName = s.BotName ?? string.Empty,
            Enabled = s.Enabled,
            OpenAIModel = s.OpenAIModel ?? string.Empty,
            OpenAIPrompt = s.OpenAIPrompt ?? string.Empty,
            MaxTokens = s.MaxTokens,
            RetryAttempts = s.RetryAttempts,
            OwnerId = ToIdString(s.OwnerId),
            EvilId = ToIdString(s.EvilId),
            LogChannelId = ToIdString(s.LogChannelId),
            EvilLogChannelId = ToIdString(s.EvilLogChannelId),
            VerifyLogChannelId = ToIdString(s.VerifyLogChannelId),
            RulesChannelId = ToIdString(s.RulesChannelId),
            GiveawayScoreboardChannelId = ToIdString(s.GiveawayScoreboardChannelId),
            ListenChannelIds = s.ListenChannelIds ?? string.Empty,
            AdminRoleIds = s.AdminRoleIds ?? string.Empty,
            VerifiedRoleId = ToIdString(s.VerifiedRoleId),
            UnverifiedRoleId = ToIdString(s.UnverifiedRoleId),
            ContactCategoryId = ToIdString(s.ContactCategoryId),
            HasToken = !string.IsNullOrWhiteSpace(s.Token),
            Token = null,
        };

        /// <summary>
        /// Apply editable fields from this DTO onto an existing entity. BotId is
        /// the key and is never reassigned here; the token is only overwritten
        /// when a non-empty value was supplied.
        /// </summary>
        public void ApplyTo(BotSetting s)
        {
            s.BotName = BotName;
            s.Enabled = Enabled;
            s.OpenAIModel = OpenAIModel;
            s.OpenAIPrompt = OpenAIPrompt;
            s.MaxTokens = MaxTokens;
            s.RetryAttempts = RetryAttempts;
            s.OwnerId = ParseId(OwnerId);
            s.EvilId = ParseId(EvilId);
            s.LogChannelId = ParseId(LogChannelId);
            s.EvilLogChannelId = ParseId(EvilLogChannelId);
            s.VerifyLogChannelId = ParseId(VerifyLogChannelId);
            s.RulesChannelId = ParseId(RulesChannelId);
            s.GiveawayScoreboardChannelId = ParseId(GiveawayScoreboardChannelId);
            s.ListenChannelIds = NormalizeCsv(ListenChannelIds);
            s.AdminRoleIds = NormalizeCsv(AdminRoleIds);
            s.VerifiedRoleId = ParseId(VerifiedRoleId);
            s.UnverifiedRoleId = ParseId(UnverifiedRoleId);
            s.ContactCategoryId = ParseId(ContactCategoryId);

            if (!string.IsNullOrWhiteSpace(Token))
            {
                s.Token = Token.Trim();
            }
        }

        private static string ToIdString(ulong id) => id == 0 ? string.Empty : id.ToString();

        private static ulong ParseId(string? value)
            => ulong.TryParse((value ?? string.Empty).Trim(), out var id) ? id : 0UL;

        /// <summary>Trim and drop blank entries from a comma-separated id list.</summary>
        private static string NormalizeCsv(string? csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return string.Empty;
            var parts = csv.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
            return string.Join(",", parts);
        }
    }
}
