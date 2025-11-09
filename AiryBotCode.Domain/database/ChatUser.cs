namespace AiryBotCode.Domain.database
{
    public enum ChatRole
    {
        System,
        User,
        Assistant,
        Owner //ID:405431299323461634 The creator of the AI
    }

    public class ChatUser
    {
        public ulong Id { get; set; }          
        public string UserName { get; set; }
        public ChatRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string AiOpinion { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
