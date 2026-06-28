namespace AiryBotCode.Domain.database
{
    // The single global capability code for the PUBLIC shareable suggestions board.
    // Whoever holds the code (in the share URL) can view + submit suggestions without
    // logging in; approval stays in the dashboard. Project-wide (NOT per-bot), one row.
    // Regenerating replaces the row, which instantly invalidates the old share link.
    public class SuggestionShareCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
