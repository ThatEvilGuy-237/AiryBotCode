using AiryBotCode.Application.Features.SpamCatcher;
using Xunit;

namespace AiryBotCode.Tests
{
    public class SpamTrackerTests
    {
        private static readonly DateTimeOffset T0 = new(2026, 6, 16, 8, 0, 0, TimeSpan.Zero);

        // Base config: 3 messages across 2 distinct channels within 6 seconds.
        private const ulong G = 1, U = 100, C1 = 10, C2 = 20, C3 = 30;
        private const int Threshold = 3, Distinct = 2, Window = 6;

        private static IReadOnlyList<(ulong, ulong)>? Rec(
            SpamTracker t, ulong channel, ulong message, DateTimeOffset at, ulong user = U) =>
            t.Record(G, user, channel, message, Threshold, Distinct, Window, at);

        [Fact]
        public void Below_message_threshold_does_not_trigger()
        {
            var t = new SpamTracker();
            Assert.Null(Rec(t, C1, 1, T0));
            Assert.Null(Rec(t, C2, 2, T0.AddSeconds(1)));   // only 2 messages
        }

        [Fact]
        public void Enough_messages_but_one_channel_does_not_trigger()
        {
            var t = new SpamTracker();
            Assert.Null(Rec(t, C1, 1, T0));
            Assert.Null(Rec(t, C1, 2, T0.AddSeconds(1)));
            Assert.Null(Rec(t, C1, 3, T0.AddSeconds(2)));   // 3 msgs, only 1 distinct channel
        }

        [Fact]
        public void Burst_across_channels_within_window_triggers_and_returns_burst()
        {
            var t = new SpamTracker();
            Assert.Null(Rec(t, C1, 1, T0));
            Assert.Null(Rec(t, C2, 2, T0.AddSeconds(2)));
            var hit = Rec(t, C3, 3, T0.AddSeconds(4));       // 3 msgs / 3 channels / 4s

            Assert.NotNull(hit);
            Assert.Equal(3, hit!.Count);
            Assert.Equal(new[] { (C1, 1UL), (C2, 2UL), (C3, 3UL) }, hit);
        }

        [Fact]
        public void Messages_outside_the_window_are_dropped()
        {
            var t = new SpamTracker();
            Assert.Null(Rec(t, C1, 1, T0));
            Assert.Null(Rec(t, C2, 2, T0.AddSeconds(2)));
            // 3rd lands after the first has aged out → window holds only msgs 2 & 3.
            Assert.Null(Rec(t, C3, 3, T0.AddSeconds(7)));
        }

        [Fact]
        public void Each_user_is_tracked_independently()
        {
            var t = new SpamTracker();
            // Two different users each send across two channels — neither alone trips.
            Assert.Null(Rec(t, C1, 1, T0, user: 100));
            Assert.Null(Rec(t, C2, 2, T0.AddSeconds(1), user: 200));
            Assert.Null(Rec(t, C2, 3, T0.AddSeconds(2), user: 100)); // user 100: 2 msgs
            Assert.Null(Rec(t, C1, 4, T0.AddSeconds(3), user: 200)); // user 200: 2 msgs
        }

        [Fact]
        public void Cooldown_prevents_immediate_retrigger_from_the_same_burst()
        {
            var t = new SpamTracker();
            Assert.Null(Rec(t, C1, 1, T0));
            Assert.Null(Rec(t, C2, 2, T0.AddSeconds(1)));
            Assert.NotNull(Rec(t, C3, 3, T0.AddSeconds(2)));         // trips
            // The tail of the same burst must not fire a second timeout.
            Assert.Null(Rec(t, C1, 4, T0.AddSeconds(3)));
            Assert.Null(Rec(t, C2, 5, T0.AddSeconds(4)));
        }
    }
}
