namespace AiryBotCode.Domain.database
{
    // The running state of the counting game in one channel (per bot). One row per
    // (BotId, ChannelId). BossAnswer is null while a boss is pending (waiting for
    // Airy to set the puzzle answer over the Hive socket).
    public class CountingState
    {
        public int Id { get; set; }
        public ulong BotId { get; set; }
        public ulong ChannelId { get; set; }
        public long CurrentCount { get; set; }
        public ulong LastUserId { get; set; }
        public ulong LastMessageId { get; set; }
        public long HighScore { get; set; }
        public bool BossActive { get; set; }
        public double? BossAnswer { get; set; }
        public DateTime? BossSpawnedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
