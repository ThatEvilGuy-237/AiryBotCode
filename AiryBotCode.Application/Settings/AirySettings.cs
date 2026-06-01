using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Settings
{
    /// <summary>
    /// Strongly-typed, grouped view of every configurable value the bot uses at
    /// runtime. This is the single, discoverable place handlers read settings
    /// from — no more hardcoded Discord IDs scattered through interaction logic.
    ///
    /// Values are defined once here and shared across handlers (e.g. many places
    /// read the same <c>Channels.Log</c> or <c>Owner</c>), and the grouping makes
    /// the relationships between them explicit. Built from the persisted
    /// <see cref="BotSetting"/> via <see cref="FromBotSetting"/> and preloaded at
    /// startup by <see cref="ISettingsProvider"/>.
    /// </summary>
    public sealed class AirySettings
    {
        /// <summary>Bot owner / super-admin user id. Shared by every owner-only check.</summary>
        public ulong Owner { get; init; }

        public ChannelSettings Channels { get; init; } = new();
        public RoleSettings Roles { get; init; } = new();
        public CategorySettings Categories { get; init; } = new();
        public AiSettings Ai { get; init; } = new();

        /// <summary>Project the flat persisted setting into the grouped tree.</summary>
        public static AirySettings FromBotSetting(BotSetting s) => new()
        {
            Owner = s.OwnerId,
            Channels = new ChannelSettings
            {
                Log = s.LogChannelId,
                EvilLog = s.EvilLogChannelId,
                VerifyLog = s.VerifyLogChannelId,
                Rules = s.RulesChannelId,
                GiveawayScoreboard = s.GiveawayScoreboardChannelId,
                Listen = ParseIds(s.ListenChannelIds),
            },
            Roles = new RoleSettings
            {
                Verified = s.VerifiedRoleId,
                Unverified = s.UnverifiedRoleId,
                Admin = ParseIds(s.AdminRoleIds),
            },
            Categories = new CategorySettings
            {
                Contact = s.ContactCategoryId,
            },
            Ai = new AiSettings
            {
                Model = string.IsNullOrWhiteSpace(s.OpenAIModel) ? "gpt-4.1-mini" : s.OpenAIModel,
                Prompt = s.OpenAIPrompt ?? string.Empty,
                MaxTokens = s.MaxTokens > 0 ? s.MaxTokens : 1000,
                RetryAttempts = s.RetryAttempts > 0 ? s.RetryAttempts : 2,
            },
        };

        /// <summary>
        /// Parse a comma-separated list of ulong ids — the shared storage format
        /// used by both <c>AdminRoleIds</c> and <c>ListenChannelIds</c>. Blank and
        /// unparseable entries are skipped.
        /// </summary>
        public static IReadOnlyList<ulong> ParseIds(string? csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return Array.Empty<ulong>();
            return csv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(p => ulong.TryParse(p, out var id) ? id : 0UL)
                .Where(id => id != 0)
                .ToList();
        }
    }

    public sealed class ChannelSettings
    {
        public ulong Log { get; init; }
        public ulong EvilLog { get; init; }
        public ulong VerifyLog { get; init; }
        public ulong Rules { get; init; }
        public ulong GiveawayScoreboard { get; init; }

        /// <summary>Channels Airy listens and responds in.</summary>
        public IReadOnlyList<ulong> Listen { get; init; } = Array.Empty<ulong>();
    }

    public sealed class RoleSettings
    {
        public ulong Verified { get; init; }
        public ulong Unverified { get; init; }

        /// <summary>Roles allowed to run admin/moderation commands.</summary>
        public IReadOnlyList<ulong> Admin { get; init; } = Array.Empty<ulong>();
    }

    public sealed class CategorySettings
    {
        /// <summary>Category under which private contact channels are created.</summary>
        public ulong Contact { get; init; }
    }

    public sealed class AiSettings
    {
        public string Model { get; init; } = "gpt-4.1-mini";
        public string Prompt { get; init; } = string.Empty;
        public int MaxTokens { get; init; } = 1000;
        public int RetryAttempts { get; init; } = 2;
    }
}
