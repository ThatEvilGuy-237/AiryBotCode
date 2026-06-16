using AiryBotCode.Application.Hive;
using Xunit;

namespace AiryBotCode.Tests
{
    public class AskRouterTests
    {
        [Fact]
        public void Routes_ask_user_with_options()
        {
            var i = AskRouter.Route("ask_user", "e1", "Pick one", new[] { "A", "B" }, "124654654000000000");
            Assert.NotNull(i);
            Assert.Equal(124654654000000000UL, i!.ChannelId);
            Assert.Equal("e1", i.EffectId);
            Assert.Equal("Pick one", i.Question);
            Assert.Equal(new[] { "A", "B" }, i.Options);
        }

        [Fact]
        public void Null_when_not_ask_user_or_missing_fields()
        {
            Assert.Null(AskRouter.Route("say", "e1", "q", new[] { "A" }, "1"));
            Assert.Null(AskRouter.Route("ask_user", "", "q", new[] { "A" }, "1"));
            Assert.Null(AskRouter.Route("ask_user", "e1", "  ", new[] { "A" }, "1"));
            Assert.Null(AskRouter.Route("ask_user", "e1", "q", new[] { "A" }, "not-a-channel"));
        }

        [Fact]
        public void Dedupes_trims_and_caps_options_and_defaults_when_empty()
        {
            var capped = AskRouter.Route("ask_user", "e1", "q", new[] { "a", "a", "b", "c", "d", "e", "f" }, "1");
            Assert.Equal(AskRouter.MaxOptions, capped!.Options.Count);

            var trimmed = AskRouter.Route("ask_user", "e1", "q", new[] { " x ", "x" }, "1");
            Assert.Equal(new[] { "x" }, trimmed!.Options);

            var defaulted = AskRouter.Route("ask_user", "e1", "q", new string[0], "1");
            Assert.Equal(new[] { "Yes", "No" }, defaulted!.Options);
        }
    }

    public class AskInteractionCustomIdTests
    {
        [Fact]
        public void Build_and_parse_roundtrip()
        {
            var id = AskInteraction.BuildAnswerId("eff-123", "Blue");
            var parsed = AskInteraction.TryParseAnswerId(id);
            Assert.NotNull(parsed);
            Assert.Equal("eff-123", parsed!.Value.EffectId);
            Assert.Equal("Blue", parsed.Value.Answer);
        }

        [Fact]
        public void Answer_may_contain_colons()
        {
            var id = AskInteraction.BuildAnswerId("eff-1", "12:30 PM");
            var parsed = AskInteraction.TryParseAnswerId(id);
            Assert.Equal("12:30 PM", parsed!.Value.Answer);
        }

        [Fact]
        public void Caps_custom_id_at_100_chars_preserving_effect_id()
        {
            var id = AskInteraction.BuildAnswerId("eff-keep", new string('x', 200));
            Assert.True(id.Length <= 100);
            Assert.Equal("eff-keep", AskInteraction.TryParseAnswerId(id)!.Value.EffectId);
        }

        [Fact]
        public void Rejects_foreign_custom_ids()
        {
            Assert.Null(AskInteraction.TryParseAnswerId("consent:accept:1:2"));
            Assert.Null(AskInteraction.TryParseAnswerId(null));
            Assert.Null(AskInteraction.TryParseAnswerId("effect:ask:"));
        }
    }

    public class AskDeliveryAndGatewayTests
    {
        private sealed class FakeEffectDelivery : IEffectDelivery
        {
            public Task SendAsync(ulong channelId, string text, CancellationToken ct = default) => Task.CompletedTask;
        }

        private sealed class FakeAskDelivery : IAskDelivery
        {
            public readonly List<(ulong ChannelId, string EffectId, string Question, IReadOnlyList<string> Options)> Asked = new();
            public Task SendAskAsync(ulong channelId, string effectId, string question, IReadOnlyList<string> options, CancellationToken ct = default)
            { Asked.Add((channelId, effectId, question, options)); return Task.CompletedTask; }
        }

        private static string AskFrame(string effectId, string question, string[] options, string sessionId) =>
            System.Text.Json.JsonSerializer.Serialize(new
            {
                type = "effect",
                call = new { id = effectId, name = "ask_user", arguments = new { question, options } },
                context = new { userId = "u", sessionId },
                at = "now",
            });

        [Fact]
        public async Task Listener_routes_ask_user_to_ask_delivery()
        {
            var ask = new FakeAskDelivery();
            var l = new HiveEffectListener("ws://unused", new FakeEffectDelivery(), null, ask);
            await l.HandleMessageAsync(AskFrame("eff-9", "Proceed?", new[] { "Yes", "No" }, "124654654000000000"));
            Assert.Single(ask.Asked);
            Assert.Equal(124654654000000000UL, ask.Asked[0].ChannelId);
            Assert.Equal("eff-9", ask.Asked[0].EffectId);
            Assert.Equal("Proceed?", ask.Asked[0].Question);
            Assert.Equal(new[] { "Yes", "No" }, ask.Asked[0].Options);
        }

        [Fact]
        public async Task Gateway_returns_false_when_unbound_then_delegates_when_bound()
        {
            var gw = new HiveEffectGateway();
            Assert.False(gw.IsConnected);
            Assert.False(await gw.SendAnswerAsync("e", "a", "s", "u"));

            var fake = new FakeSender();
            gw.Bind(fake);
            Assert.True(gw.IsConnected);
            Assert.True(await gw.SendAnswerAsync("e", "a", "s", "u"));
            Assert.Equal(("e", "a"), fake.Last);
        }

        private sealed class FakeSender : IHiveResponseSender
        {
            public (string, string) Last;
            public bool IsConnected => true;
            public Task<bool> SendAnswerAsync(string effectId, string answer, string? sessionId, string? userId, CancellationToken ct = default)
            { Last = (effectId, answer); return Task.FromResult(true); }
        }
    }
}
