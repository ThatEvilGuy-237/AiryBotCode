using AiryBotCode.Application.Hive;
using Xunit;

namespace AiryBotCode.Tests
{
    public class EffectRouterTests
    {
        [Fact]
        public void Routes_say_to_the_session_channel()
        {
            var i = EffectRouter.Route("say", "hello", 0, "124654654000000000");
            Assert.NotNull(i);
            Assert.Equal(124654654000000000UL, i!.ChannelId);
            Assert.Equal("hello", i.Text);
            Assert.Equal(0, i.DelaySeconds);
        }

        [Fact]
        public void Routes_send_message_alias()
        {
            Assert.NotNull(EffectRouter.Route("send_message", "hi", 1, "222222222222222222"));
        }

        [Fact]
        public void Ignores_non_deliverable_effects()
        {
            Assert.Null(EffectRouter.Route("schedule_message", "later", 0, "222222222222222222"));
            Assert.Null(EffectRouter.Route("recall_image", "x", 0, "222222222222222222"));
        }

        [Fact]
        public void Null_when_message_empty_or_session_not_a_channel()
        {
            Assert.Null(EffectRouter.Route("say", "   ", 0, "222222222222222222"));
            Assert.Null(EffectRouter.Route("say", "hi", 0, "not-a-snowflake"));
        }

        [Fact]
        public void Default_delay_is_2_and_clamps_to_60()
        {
            Assert.Equal(2, EffectRouter.Route("say", "hi", null, "1")!.DelaySeconds);
            Assert.Equal(60, EffectRouter.Route("say", "hi", 999, "1")!.DelaySeconds);
            Assert.Equal(0, EffectRouter.Route("say", "hi", -3, "1")!.DelaySeconds);
        }
    }

    public class HiveEffectListenerTests
    {
        private sealed class FakeDelivery : IEffectDelivery
        {
            public readonly List<(ulong ChannelId, string Text)> Sent = new();
            public Task SendAsync(ulong channelId, string text, CancellationToken ct = default)
            { Sent.Add((channelId, text)); return Task.CompletedTask; }
        }

        private static string Effect(string name, string message, string sessionId, int delay = 0) =>
            System.Text.Json.JsonSerializer.Serialize(new
            {
                type = "effect",
                call = new { id = "e1", name, arguments = new { message, delaySeconds = delay } },
                context = new { userId = "u", sessionId },
                at = "now",
            });

        [Fact]
        public async Task Delivers_a_say_effect_to_its_channel()
        {
            var d = new FakeDelivery();
            var l = new HiveEffectListener("ws://unused", d);
            await l.HandleMessageAsync(Effect("say", "first answer", "124654654000000000"));
            Assert.Single(d.Sent);
            Assert.Equal(124654654000000000UL, d.Sent[0].ChannelId);
            Assert.Equal("first answer", d.Sent[0].Text);
        }

        [Fact]
        public async Task Ignores_non_effect_and_non_deliverable_frames()
        {
            var d = new FakeDelivery();
            var l = new HiveEffectListener("ws://unused", d);
            await l.HandleMessageAsync("{\"type\":\"subscribed\"}");
            await l.HandleMessageAsync(Effect("schedule_message", "later", "1"));
            Assert.Empty(d.Sent);
        }

        [Fact]
        public async Task Malformed_frame_does_not_throw_or_deliver()
        {
            var d = new FakeDelivery();
            var l = new HiveEffectListener("ws://unused", d);
            await l.HandleMessageAsync("not json at all");
            await l.HandleMessageAsync("{\"type\":\"effect\"}");   // missing call
            Assert.Empty(d.Sent);
        }
    }
}
