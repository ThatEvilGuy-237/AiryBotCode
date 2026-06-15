using System.Text.RegularExpressions;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    /// <summary>
    /// Rewrites raw Discord user mentions in an incoming message into a readable
    /// "DisplayName (id)" form BEFORE the text is forwarded to the Hive flow — so
    /// the agent and memory see a human name instead of a bare snowflake, while the
    /// id is kept so the agent can still emit <c>&lt;@id&gt;</c> to ping the user back.
    ///
    /// Pure + Discord-free (takes a name resolver) so it's unit-testable; the
    /// handler supplies the guild lookup.
    /// </summary>
    public static class MentionResolver
    {
        // <@123> and the nickname form <@!123>. Role mentions (<@&123>) are NOT
        // matched (the '&' fails the class), and neither are @everyone/@here.
        private static readonly Regex Tagged = new(@"<@!?(\d+)>", RegexOptions.Compiled);

        // A bare "@<snowflake>" not already inside a tag and not part of a word.
        // Restricted to snowflake-length digit runs so prices/handles like "@123"
        // aren't mangled.
        private static readonly Regex Bare = new(@"(?<![\w<])@(\d{16,20})\b", RegexOptions.Compiled);

        /// <param name="resolveName">id → display name (nickname ?? username), or null if unknown.</param>
        public static string Rewrite(string content, Func<ulong, string?> resolveName)
        {
            if (string.IsNullOrEmpty(content)) return content;

            string Replace(Match m)
            {
                if (!ulong.TryParse(m.Groups[1].Value, out var id)) return m.Value;
                var name = resolveName(id);
                return string.IsNullOrWhiteSpace(name) ? $"user {id}" : $"{name} ({id})";
            }

            content = Tagged.Replace(content, Replace);
            content = Bare.Replace(content, Replace);
            return content;
        }
    }
}
