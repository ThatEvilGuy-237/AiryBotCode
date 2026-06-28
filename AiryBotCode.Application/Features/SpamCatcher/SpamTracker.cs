namespace AiryBotCode.Application.Features.SpamCatcher
{
    /// <summary>What recording a message means for the caller.</summary>
    public enum SpamOutcome
    {
        /// <summary>Not spam (yet) — do nothing.</summary>
        None,
        /// <summary>This message tipped the user over the threshold — apply the full
        /// action (timeout + delete the burst + log).</summary>
        Tripped,
        /// <summary>A straggler from a burst we ALREADY caught — don't time out or log
        /// again, but the message is still part of the spam, so delete it too.</summary>
        Tail,
    }

    /// <summary>
    /// Result of <see cref="SpamTracker.Record"/>. <see cref="Messages"/> holds the
    /// message(s) to delete: the whole burst on <see cref="SpamOutcome.Tripped"/>, or
    /// just the straggler on <see cref="SpamOutcome.Tail"/>; empty on
    /// <see cref="SpamOutcome.None"/>.
    /// </summary>
    public readonly record struct SpamResult(
        SpamOutcome Outcome,
        IReadOnlyList<(ulong ChannelId, ulong MessageId)> Messages)
    {
        public static readonly SpamResult None =
            new(SpamOutcome.None, Array.Empty<(ulong, ulong)>());
    }

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
        /// Record one message from a monitored user and decide what to do with it.
        ///
        /// Returns <see cref="SpamOutcome.Tripped"/> (with the whole burst) when this
        /// message tips the user over the threshold — enough messages spread across
        /// enough distinct channels inside the window. On a trigger the user's window is
        /// cleared and a short cooldown is set so a single burst fires the timeout/log
        /// exactly once.
        ///
        /// During that cooldown the rest of the same burst keeps arriving (the gateway
        /// already had those messages queued before the timeout landed). Those return
        /// <see cref="SpamOutcome.Tail"/> so the caller deletes them too instead of
        /// leaving half the spam behind — otherwise only the messages that happened to
        /// trip the threshold get removed.
        /// </summary>
        public SpamResult Record(
            ulong guildId, ulong userId, ulong channelId, ulong messageId,
            int messageThreshold, int distinctChannels, int windowSeconds,
            DateTimeOffset? at = null)
        {
            lock (_lock)
            {
                var now = at ?? DateTimeOffset.UtcNow;
                var key = (guildId, userId);

                // Just caught this user — this is the tail of the same burst. Don't
                // re-fire the timeout/log, but hand the message back to be deleted.
                if (_cooldownUntil.TryGetValue(key, out var until) && until > now)
                    return new SpamResult(SpamOutcome.Tail, new[] { (channelId, messageId) });

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
                    return new SpamResult(SpamOutcome.Tripped, offending);
                }

                return SpamResult.None;
            }
        }
    }
}
