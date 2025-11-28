namespace AiryBotCode.Domain.database
{
    public class GiveAwayUser
    {
        public ulong Id { get; set; }
        public string UserName { get; set; }
        public DateTime SubmitedAt { get; set; } = DateTime.UtcNow;
    }
}
