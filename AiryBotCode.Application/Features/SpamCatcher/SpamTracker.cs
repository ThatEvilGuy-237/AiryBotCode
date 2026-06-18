namespace AiryBotCode.Application.Features.SpamCatcher
{
    /// <summary>
    /// In-memory sliding-window tracker for cross-channel spam, keyed PER USER
    /// (per guild). Every monitored member is tracked independently — messages from
    /// different role-holders are never pooled together.
    ///
    /// Registered as a singleton so the window survives across the request-scoped
    /// message handlers. Thread-safe: the Discord gateway dispatches message events
    /// concurrently (handlers are offloaded with Task.Run).
    /// </summary>
    public sealed class SpamTracker
    {
        private readonly record struct Entry(ulong ChannelId, ulong MessageId, DateTimeOffset At);

        private readonly Dictionary<(ulong Guild, ulong User), List<Entry>> _events = new();
        private readonly Dictionary<(ulong Guild, ulong User), DateTimeOffset> _cooldownUntil = new();
        private readonly object _lock = new();

        /// <summary>
        /// Record one message from a monitored user. Returns the burst of offending
        /// messages (channel + message ids) when this message tips the user over the
        /// threshold — enough messages spread across enough distinct channels inside
        /// the window — otherwise null.
        ///
        /// On a trigger the user's window is cleared and a short cooldown is set so a
        /// single burst fires exactly once while the timeout/delete is applied.
        /// </summary>
        public IReadOnlyList<(ulong ChannelId, ulong MessageId)>? Record(
            ulong guildId, ulong userId, ulong channelId, ulong messageId,
            int messageThreshold, int distinctChannels, int windowSeconds,
            DateTimeOffset? at = null)
        {
            lock (_lock)
            {
                var now = at ?? DateTimeOffset.UtcNow;
                var key = (guildId, userId);

                // Just caught this user — ignore the tail of the same burst.
                if (_cooldownUntil.TryGetValue(key, out var until) && until > now)
                    return null;

                if (!_events.TryGetValue(key, out var list))
                    _events[key] = list = new List<Entry>();

                // Drop anything that has aged out of the window, then add this message.
                var cutoff = now.AddSeconds(-windowSeconds);
                list.RemoveAll(e => e.At < cutoff);
                list.Add(new Entry(channelId, messageId, now));

                var distinct = list.Select(e => e.ChannelId).Distinct().Count();
                if (list.Count >= messageThreshold && distinct >= distinctChannels)
                {
                    var offending = list.Select(e => (e.ChannelId, e.MessageId)).ToList();
                    list.Clear();
                    _cooldownUntil[key] = now.AddSeconds(Math.Max(windowSeconds, 10));
                    return offending;
                }

                return null;
            }
        }
    }
}
