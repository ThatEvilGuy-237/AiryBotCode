namespace AiryBotCode.Api.Models
{
    // Wire shape for the control panel's Bot Settings page. Discord snowflakes are
    // exchanged as strings (64-bit ids overflow JS's safe-integer range). `Token`
    // is write-only: the API never returns it (only `HasToken`), and an empty
    // value on update means "keep the stored token unchanged".
    public class BotSettingDto
    {
        public string BotId { get; set; }
        public string BotName { get; set; }
        public bool Enabled { get; set; }

        public string AdminRoleIds { get; set; }
        public string EvilId { get; set; }

        public string LogChannelId { get; set; }
        public string EvilLogChannelId { get; set; }

        public bool HasToken { get; set; }
        public string Token { get; set; }

        // Which database this bot's data uses. Null/empty = shared default.
        public string DatabaseName { get; set; }

        // Theme palette (hex). Read-only here; set via the dedicated theme endpoint.
        public string ThemePrimary { get; set; }
        public string ThemeAccent { get; set; }
    }
}
