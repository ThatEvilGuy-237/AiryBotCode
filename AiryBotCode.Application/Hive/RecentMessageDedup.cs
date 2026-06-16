namespace AiryBotCode.Application.Hive
{
    /// <summary>
    /// Per-channel short-window dedup shared by the two independent paths that post
    /// agent output to Discord: the effect path (the <c>say</c> tool → DiscordEffectDelivery)
    /// and the webhook-reply path (the flow's final answer → MessageSendHandler). Each
    /// path calls <see cref="TryReserve"/> before sending; if the identical text was
    /// already posted to that channel within the window, the second caller is told to
    /// skip — preventing the "agent said its answer AND finished with the same text →
    /// double post" case, regardless of which path wins the race.
    ///
    /// This is the deterministic half of item A's reply-contract fork: the run can emit
    /// a message as either an effect or the final reply (or both) and the user sees it
    /// exactly once. Window is deliberately short so a genuinely-repeated message (e.g.
    /// the agent answers "OK" to two quick questions) isn't suppressed for long.
    /// Singleton; thread-safe.
    /// </summary>
    public sealed class RecentMessageDedup
    {
        public static readonly TimeSpan DefaultWindow = TimeSpan.FromSeconds(60);

        private readonly TimeSpan _window;
        private readonly object _lock = new();
        // channel → (normalized text → last-posted timestamp)
        private readonly Dictionary<ulong, Dictionary<string, DateTimeOffset>> _seen = new();

        public RecentMessageDedup() : this(DefaultWindow) { }
        public RecentMessageDedup(TimeSpan window) => _window = window;

        /// <summary>Returns true if this (channel, text) may be sent now — and records it.
        /// Returns false if the identical text was sent to this channel within the window
        /// (caller should skip). Blank text is always allowed through (callers guard it
        /// separately) and never recorded.</summary>
        public bool TryReserve(ulong channelId, string? text) => TryReserve(channelId, text, DateTimeOffset.UtcNow);

        // Time-injectable core for tests.
        public bool TryReserve(ulong channelId, string? text, DateTimeOffset now)
        {
            var key = (text ?? string.Empty).Trim();
            if (key.Length == 0) return true;

            lock (_lock)
            {
                if (!_seen.TryGetValue(channelId, out var byText))
                {
                    byText = new Dictionary<string, DateTimeOffset>();
                    _seen[channelId] = byText;
                }

                // Prune expired entries for this channel.
                if (byText.Count > 0)
                    foreach (var k in byText.Where(kv => now - kv.Value > _window).Select(kv => kv.Key).ToList())
                        byText.Remove(k);

                if (byText.TryGetValue(key, out var last) && now - last <= _window)
                    return false;   // duplicate within window → skip

                byText[key] = now;
                return true;
            }
        }
    }
}
