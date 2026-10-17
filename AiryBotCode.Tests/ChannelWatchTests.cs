using AiryBotCode.Application.Conversation;
using AiryBotCode.Application.Hive;
using Xunit;

namespace AiryBotCode.Tests
{
    public class ChannelWatchTests
    {
        private static readonly ConversationSettings Fast = new() { Quiet = TimeSpan.FromMilliseconds(150), MaxMessages = 3, MaxWait = TimeSpan.FromMilliseconds(600), Cooldown = TimeSpan.FromMilliseconds(400), QuickQuiet = TimeSpan.FromMilliseconds(40), InTalkFor = TimeSpan.FromMilliseconds(400) };

        private static ChannelWatch Watch() => new(Fast, TimeProvider.System);

        [Fact]
        public async Task A_stretch_is_checked_once_after_the_room_goes_quiet()
        {
            var watch = Watch();
            var checks = 0;
            watch.CheckAfterPause(1, 10, () => { checks++; return Task.CompletedTask; });
            await Task.Delay(60);
            watch.CheckAfterPause(1, 10, () => { checks++; return Task.CompletedTask; });
            await Task.Delay(60);
            Assert.Equal(0, checks);
            await Task.Delay(300);
            Assert.Equal(1, checks);
        }

        [Fact]
        public async Task Enough_messages_are_checked_at_once_with_the_newest_check()
        {
            var watch = Watch();
            var seen = new List<int>();
            for (var i = 1; i <= 3; i++)
            {
                var n = i;
                watch.CheckAfterPause(1, 10, () => { seen.Add(n); return Task.CompletedTask; });
            }
            await Task.Delay(50);
            Assert.Equal([3], seen);
        }

        [Fact]
        public async Task A_steady_stream_is_still_checked_within_the_longest_wait()
        {
            var watch = Watch();
            var checks = 0;
            var started = DateTime.UtcNow;
            while (checks == 0 && DateTime.UtcNow - started < TimeSpan.FromSeconds(2))
            {
                watch.CheckAfterPause(1, 10, () => { checks++; return Task.CompletedTask; });
                await Task.Delay(100);
                watch.Cancel(2, 20);
            }
            Assert.Equal(1, checks);
            Assert.True(DateTime.UtcNow - started < TimeSpan.FromMilliseconds(1200));
        }

        [Fact]
        public async Task A_ping_cancels_the_waiting_check_and_other_channels_keep_theirs()
        {
            var watch = Watch();
            var here = 0;
            var there = 0;
            watch.CheckAfterPause(1, 10, () => { here++; return Task.CompletedTask; });
            watch.CheckAfterPause(1, 11, () => { there++; return Task.CompletedTask; });
            watch.Cancel(1, 10);
            await Task.Delay(300);
            Assert.Equal(0, here);
            Assert.Equal(1, there);
        }

        [Fact]
        public async Task A_follow_up_waits_only_a_short_moment()
        {
            var watch = Watch();
            var checks = 0;
            watch.CheckAfterPause(1, 10, () => { checks++; return Task.CompletedTask; }, quick: true);
            await Task.Delay(110);
            Assert.Equal(1, checks);
        }

        [Fact]
        public async Task After_speaking_the_channel_is_in_talk_for_a_while()
        {
            var watch = Watch();
            Assert.False(watch.InTalk(1, 10));
            watch.Spoke(1, 10);
            Assert.True(watch.InTalk(1, 10));
            Assert.False(watch.InTalk(1, 11));
            Assert.False(watch.CoolingDown(1, 10));
            await Task.Delay(450);
            Assert.False(watch.InTalk(1, 10));
        }

        [Fact]
        public async Task After_joining_in_the_channel_cools_down()
        {
            var watch = Watch();
            Assert.False(watch.CoolingDown(1, 10));
            watch.Joined(1, 10);
            Assert.True(watch.CoolingDown(1, 10));
            Assert.False(watch.CoolingDown(1, 11));
            await Task.Delay(450);
            Assert.False(watch.CoolingDown(1, 10));
        }
    }
}
