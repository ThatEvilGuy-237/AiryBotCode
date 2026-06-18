namespace AiryBotCode.Domain.database
{
    // One leveling row per bot+user. XP is GLOBAL (cross-server) for now: a user has
    // a single XP pool no matter which server they earn it in. Level is derived from
    // Xp and cached here for cheap leaderboard reads.
    public class LevelUser
    {
        public int Id { get; set; }
        public ulong BotId { get; set; }
        public ulong UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long Xp { get; set; }
        public int Level { get; set; }
        public DateTime LastXpAt { get; set; }
    }
}
