namespace AiryBotCode.Domain.database
{
    // A feature idea / suggestion for the project, submitted from the dashboard by
    // the team. Project-wide (NOT per-bot). The review fields (Why / Estimate) are
    // filled in later by the maintainer; Approved is toggled from the panel.
    public class Suggestion
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        // Who submitted it (the logged-in panel user's name, best-effort).
        public string SubmittedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // new | reviewed | approved | rejected
        public string Status { get; set; } = "new";

        // Maintainer's review, added later: why it's a good idea + a rough time
        // estimate. Null until reviewed.
        public string? ResponseWhy { get; set; }
        public string? ResponseEstimate { get; set; }
        public DateTime? RespondedAt { get; set; }

        public bool Approved { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
