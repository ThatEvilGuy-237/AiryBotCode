using AiryBotCode.Application.Hive;
using Xunit;

namespace AiryBotCode.Tests
{
    public class RecentMessageDedupTests
    {
        private static readonly DateTimeOffset T0 = new(2026, 6, 16, 8, 0, 0, TimeSpan.Zero);

        [Fact]
        public void First_send_is_allowed_duplicate_within_window_is_skipped()
        {
            var d = new RecentMessageDedup(TimeSpan.FromSeconds(60));
            Assert.True(d.TryReserve(1, "hello", T0));                               // first → send
            Assert.False(d.TryReserve(1, "hello", T0.AddSeconds(5)));                // dup within window → skip
            Assert.False(d.TryReserve(1, " hello ", T0.AddSeconds(6)));             // trimmed match → skip
        }

        [Fact]
        public void Duplicate_after_window_is_allowed_again()
        {
            var d = new RecentMessageDedup(TimeSpan.FromSeconds(60));
            Assert.True(d.TryReserve(1, "hello", T0));
            Assert.True(d.TryReserve(1, "hello", T0.AddSeconds(61)));                // window passed → send again
        }

        [Fact]
        public void Channels_are_independent()
        {
            var d = new RecentMessageDedup(TimeSpan.FromSeconds(60));
            Assert.True(d.TryReserve(1, "hello", T0));
            Assert.True(d.TryReserve(2, "hello", T0.AddSeconds(1)));                 // different channel → send
        }

        [Fact]
        public void Different_text_same_channel_is_allowed()
        {
            var d = new RecentMessageDedup(TimeSpan.FromSeconds(60));
            Assert.True(d.TryReserve(1, "hello", T0));
            Assert.True(d.TryReserve(1, "world", T0.AddSeconds(1)));
        }

        [Fact]
        public void Blank_text_is_always_allowed_and_not_recorded()
        {
            var d = new RecentMessageDedup(TimeSpan.FromSeconds(60));
            Assert.True(d.TryReserve(1, "", T0));
            Assert.True(d.TryReserve(1, "   ", T0.AddSeconds(1)));
            Assert.True(d.TryReserve(1, null, T0.AddSeconds(2)));
        }

        [Fact]
        public void Models_the_say_then_finish_double_post_being_prevented()
        {
            // Agent says its answer via the effect path, then finishes with the same text;
            // whichever posts first wins, the second is suppressed → user sees it once.
            var d = new RecentMessageDedup(TimeSpan.FromSeconds(60));
            var answer = "Here is your answer.";
            Assert.True(d.TryReserve(99, answer, T0));                  // say (effect path) posts
            Assert.False(d.TryReserve(99, answer, T0.AddSeconds(3)));   // webhook reply path → skipped
        }
    }
}
