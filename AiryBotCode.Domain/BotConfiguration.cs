namespace AiryBotCode.Infrastructure.Configuration
{
    public class BotConfiguration
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Token { get; set; }
        public ulong[] AdminRoleIds { get; set; }
        public ulong EvilId { get; set; }
        public ulong LogChannelId { get; set; }
        public ulong EvilLogChannelId { get; set; }
        public ulong BotId { get; set; }
    }
}