namespace AiryBotCode.Domain.database
{
    // Links a Discord channel to a webhook (e.g. a Hive trigger at /hooks/{id}).
    // A bot can have many; messages in a linked channel are forwarded to its URL.
    public class ChannelWebhook
    {
        public int Id { get; set; }
        public ulong BotId { get; set; }
        public ulong ChannelId { get; set; }
        public string Name { get; set; }
        public string WebhookUrl { get; set; }

        // HMAC-SHA256 secret used to sign the payload (X-Hive-Signature). Optional.
        public string? Secret { get; set; }

        // "sync" waits for/relays the run; "async" fire-and-forget.
        public string Mode { get; set; } = "sync";

        public bool Enabled { get; set; } = true;
    }
}
