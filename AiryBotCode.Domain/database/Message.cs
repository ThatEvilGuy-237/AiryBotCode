namespace AiryBotCode.Domain.database
{
    public class Message
    {
        public int Id { get; set; }
        public ulong ChannelId { get; set; }

        public ulong UserId { get; set; } // FK - ChatUser
        public ChatUser User { get; set; }

        public string Context { get; set; }

        public int ChannelConversationId { get; set; }
        public ChannelConversation ChannelConversation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
