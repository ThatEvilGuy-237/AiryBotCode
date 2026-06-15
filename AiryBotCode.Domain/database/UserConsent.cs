namespace AiryBotCode.Domain.database
{
    // Records that a Discord user accepted Airy's data-use notice for a given bot.
    // First contact is gated until a row exists (one per bot+user).
    public class UserConsent
    {
        public int Id { get; set; }
        public ulong BotId { get; set; }
        public ulong UserId { get; set; }
        public DateTime AcceptedAt { get; set; }
    }
}
