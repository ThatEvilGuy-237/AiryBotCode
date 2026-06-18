namespace AiryBotCode.Application.Features.Leveling
{
    /// <summary>
    /// In-memory "one XP grant per user per N seconds" gate (the classic anti-spam
    /// rule for leveling). Singleton, thread-safe — keeps the DB quiet by only
    /// letting a write through once the cooldown elapses. State is per bot process;
    /// a restart just means a user might earn one extra grant, which is harmless.
    /// </summary>
    public sealed class XpCooldown
    {
        private readonly Dictionary<ulong, DateTimeOffset> _last = new();
        private readonly object _lock = new();

        /// <summary>
        /// Returns true and arms the cooldown when the user is eligible to earn XP;
        /// false while they're still cooling down.
        /// </summary>
        public bool TryConsume(ulong userId, int cooldownSeconds, DateTimeOffset? at = null)
        {
            lock (_lock)
            {
                var now = at ?? DateTimeOffset.UtcNow;
                if (_last.TryGetValue(userId, out var prev) && now < prev.AddSeconds(cooldownSeconds))
                    return false;
                _last[userId] = now;
                return true;
            }
        }
    }
}
