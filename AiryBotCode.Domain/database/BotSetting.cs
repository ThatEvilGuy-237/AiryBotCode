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
    }
}
