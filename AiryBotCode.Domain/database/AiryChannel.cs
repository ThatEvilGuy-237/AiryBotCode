namespace AiryBotCode.Domain.database
{
    public class AiryChannel
    {
        public int Id { get; set; }
        public ulong BotId { get; set; }
        public ulong ChannelId { get; set; }
        public string AnswerMode { get; set; } = "mention";
        public bool Enabled { get; set; } = true;
        public bool Listens { get; set; } = true;
        public DateTime? LastDecisionAt { get; set; }
        public string? LastDecision { get; set; }
        public string? LastDecisionError { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
