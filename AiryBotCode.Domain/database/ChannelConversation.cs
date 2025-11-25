namespace AiryBotCode.Domain.database
{
    public class ChannelConversation
    {
        public int Id { get; set; }
        public ulong ChannelId { get; set; }
        public string ConversationSummary { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}