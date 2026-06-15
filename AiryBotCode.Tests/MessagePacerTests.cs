using AiryBotCode.Application.Hive;
using Xunit;

namespace AiryBotCode.Tests
{
    public class MessagePacerTests
    {
        private sealed class RecordingDelivery : IEffectDelivery
        {
            public readonly List<(ulong Channel, string Text)> Sent = new();
            public Task SendAsync(ulong channelId, string text, CancellationToken ct = default)
            { lock (Sent) Sent.Add((channelId, text)); return Task.CompletedTask; }
        }

        [Fact]
        public void ComputeWait_first_message_is_immediate()
        {
            Assert.Equal(TimeSpan.Zero, MessagePacer.ComputeWait(null, 2, DateTimeOffset.UnixEpoch));
        }

        [Fact]
        public void ComputeWait_spaces_from_last_send()
        {
            var last = DateTimeOffset.UnixEpoch;
            // 0.5s after last send, with a 2s gap → 1.5s left to wait.
            var wait = MessagePacer.ComputeWait(last, 2, last.AddSeconds(0.5));
            Assert.Equal(TimeSpan.FromSeconds(1.5), wait);
        }

        [Fact]
        public void ComputeWait_overdue_is_immediate()
        {
            var last = DateTimeOffset.UnixEpoch;
            // 10s after last send, gap only 2s → already overdue → no wait.
            Assert.Equal(TimeSpan.Zero, MessagePacer.ComputeWait(last, 2, last.AddSeconds(10)));
        }

        [Fact]
        public async Task Delivers_in_order_and_waits_each_follow_up_by_its_delay()
        {
            var delivery = new RecordingDelivery();
            var now = DateTimeOffset.UnixEpoch;               // fixed clock
            var waits = new List<double>();
            var pacer = new MessagePacer(delivery, () => now, (t, ct) => { waits.Add(t.TotalSeconds); return Task.CompletedTask; });

            var t1 = pacer.Enqueue(100, "first", 2);
            var t2 = pacer.Enqueue(100, "second", 2);
            var t3 = pacer.Enqueue(100, "third", 5);
            await Task.WhenAll(t1, t2, t3);

            Assert.Equal(new[] { (100UL, "first"), (100UL, "second"), (100UL, "third") }, delivery.Sent);
            // first = immediate (no delay recorded); follow-ups wait their own delay
            Assert.Equal(new[] { 2.0, 5.0 }, waits);
        }

        [Fact]
        public async Task Channels_are_independent()
        {
            var delivery = new RecordingDelivery();
            var now = DateTimeOffset.UnixEpoch;
            var pacer = new MessagePacer(delivery, () => now, (t, ct) => Task.CompletedTask);

            await Task.WhenAll(pacer.Enqueue(1, "a", 9), pacer.Enqueue(2, "b", 9));
            // both first-to-their-channel → both delivered immediately, neither blocks the other
            Assert.Equal(2, delivery.Sent.Count);
            Assert.Contains((1UL, "a"), delivery.Sent);
            Assert.Contains((2UL, "b"), delivery.Sent);
        }
    }
}
