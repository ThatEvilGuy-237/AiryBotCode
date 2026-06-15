namespace AiryBotCode.Application.Hive
{
    /// <summary>
    /// Spaces a channel's outbound messages so follow-ups don't arrive all at once:
    /// each message waits its <c>delaySeconds</c> **from when the previous message to
    /// that channel was actually sent** (the first message goes out immediately).
    /// Per-channel and order-preserving (messages chain), but channels are
    /// independent — a slow channel never blocks another, and the WS receive loop
    /// never blocks (enqueue returns at once).
    /// </summary>
    public sealed class MessagePacer
    {
        private readonly IEffectDelivery _delivery;
        private readonly Func<DateTimeOffset> _now;
        private readonly Func<TimeSpan, CancellationToken, Task> _delay;
        private readonly object _gate = new();
        private readonly Dictionary<ulong, ChannelState> _channels = new();

        private sealed class ChannelState
        {
            public Task Tail = Task.CompletedTask;
            public DateTimeOffset LastSentAt;
            public bool HasSent;
        }

        public MessagePacer(
            IEffectDelivery delivery,
            Func<DateTimeOffset>? now = null,
            Func<TimeSpan, CancellationToken, Task>? delay = null)
        {
            _delivery = delivery;
            _now = now ?? (() => DateTimeOffset.UtcNow);
            _delay = delay ?? ((t, ct) => Task.Delay(t, ct));
        }

        /// <summary>How long to wait before sending, given when the last message to
        /// this channel was actually sent (null = none yet → send now). Pure.</summary>
        public static TimeSpan ComputeWait(DateTimeOffset? lastSentAt, int delaySeconds, DateTimeOffset now)
        {
            if (lastSentAt is null) return TimeSpan.Zero;            // first message: immediate
            var earliest = lastSentAt.Value.AddSeconds(delaySeconds);
            return earliest > now ? earliest - now : TimeSpan.Zero;  // already overdue → now
        }

        /// <summary>Queue a message for its channel. Returns the task that completes
        /// once this message has been delivered (used by tests; callers fire-and-forget).</summary>
        public Task Enqueue(ulong channelId, string text, int delaySeconds, CancellationToken ct = default)
        {
            lock (_gate)
            {
                if (!_channels.TryGetValue(channelId, out var st))
                    _channels[channelId] = st = new ChannelState();

                var link = st.Tail.ContinueWith(async _ =>
                {
                    DateTimeOffset? last;
                    lock (_gate) last = st.HasSent ? st.LastSentAt : null;

                    var wait = ComputeWait(last, delaySeconds, _now());
                    if (wait > TimeSpan.Zero) await _delay(wait, ct);

                    await _delivery.SendAsync(channelId, text, ct);
                    lock (_gate) { st.LastSentAt = _now(); st.HasSent = true; }
                }, ct, TaskContinuationOptions.None, TaskScheduler.Default).Unwrap();

                st.Tail = link;
                return link;
            }
        }
    }
}
