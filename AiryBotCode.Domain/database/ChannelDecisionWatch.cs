namespace AiryBotCode.Domain.database
{
    public class ChannelDecisionWatch
    {
        public int Id { get; set; }
        public ulong BotId { get; set; }
        public ulong ChannelId { get; set; }
        public string Name { get; set; } = "";
        public string ListenerUrl { get; set; } = "";
        public string? Secret { get; set; }
        public string? QuestionKey { get; set; }
        public string ActionsJson { get; set; } = "{}";
        public string UnsureAction { get; set; } = "ignore";
        public ulong AlertChannelId { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
