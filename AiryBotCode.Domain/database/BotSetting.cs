using System.Collections.Generic;

namespace AiryBotCode.Domain.database
{
    public class BotSetting
    {
        public string BotName { get; set; }
        public bool Enabled { get; set; }
        public string Token { get; set; }
        public string AdminRoleIds { get; set; }
        public ulong EvilId { get; set; }
        public ulong LogChannelId { get; set; }
        public ulong EvilLogChannelId { get; set; }
        public ulong BotId { get; set; }

        // Which Postgres database this bot's data lives in. Null/empty = the
        // shared default database (control plane stays there regardless).
        public string? DatabaseName { get; set; }

        // Theme palette (hex, e.g. "#e8467a"). Primary re-skins the panel + is the
        // bot's brand embed color; Accent is the panel's secondary accent.
        public string? ThemePrimary { get; set; }
        public string? ThemeAccent { get; set; }

        // Server-side per-bot image theme (so it follows across devices, not just
        // localStorage). ThemeImage = a downscaled data-URL of the chosen image
        // (also the bot's profile pic); ThemeData = the derived @hive/ui theme as
        // JSON (accent L/C/hue + base hue/chroma). Both null = no image theme set.
        public string? ThemeImage { get; set; }
        public string? ThemeData { get; set; }
    }
}
