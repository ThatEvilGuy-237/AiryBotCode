using System.Collections.Generic;

namespace AiryBotCode.Domain.database
{
    public class BotSetting
    {
        public string BotName { get; set; }
        public string OpenAIModel { get; set; }
        public string OpenAIPrompt { get; set; }
        public bool Enabled { get; set; }
        public string Token { get; set; }
        public string AdminRoleIds { get; set; }
        public ulong EvilId { get; set; }
        public ulong LogChannelId { get; set; }
        public ulong EvilLogChannelId { get; set; }
        public ulong BotId { get; set; }

        // --- Values previously hardcoded in interaction handlers ---
        // These back the grouped AirySettings tree so handlers read them from
        // one place instead of inline literals. All optional / default-safe.

        /// <summary>Owner / super-admin user id (owner-only checks).</summary>
        public ulong OwnerId { get; set; }

        /// <summary>Comma-separated channel ids Airy listens/responds in.</summary>
        public string? ListenChannelIds { get; set; }

        public ulong VerifiedRoleId { get; set; }
        public ulong UnverifiedRoleId { get; set; }
        public ulong VerifyLogChannelId { get; set; }
        public ulong RulesChannelId { get; set; }
        public ulong GiveawayScoreboardChannelId { get; set; }
        public ulong ContactCategoryId { get; set; }

        public int MaxTokens { get; set; }
        public int RetryAttempts { get; set; }
    }
}
