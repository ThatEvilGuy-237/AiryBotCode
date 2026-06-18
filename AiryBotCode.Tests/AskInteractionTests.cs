using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Services;
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

    public class AskButtonCustomIdTests
    {
        // The ask button customId is built with the canonical ButtonEncriptionService and
        // decoded the same way; this guards that contract (routing prefix, effectId in the
        // Action with a .<idx> uniqueness suffix, asker in UsersId, short + under 100 chars).
        [Fact]
        public void Encodes_and_decodes_ask_context_via_button_codec()
        {
            var effectId = "call_7KaQ731GANaRi1Xdaka19byZ";
            var asker = 405431299323461634UL;
            var enc = new ButtonEncriptionService { CommandName = "ask_user", Action = $"{effectId}.2" };
            enc.UsersId.Add(asker);
            var customId = enc.Encript();

            Assert.StartsWith("c:ask_user|", customId);
            Assert.True(customId.Length <= 100, $"customId must fit Discord's 100-char limit (was {customId.Length})");

            var dec = new ButtonEncriptionService().Decrypt(customId);
            Assert.Equal("ask_user", dec.CommandName);
            Assert.Equal(effectId, dec.Action!.Split('.')[0]);   // strip the .<idx> suffix
            Assert.Contains(asker, dec.UsersId);
        }

        [Fact]
        public void Customid_stays_short_even_with_a_long_answer()
        {
            // The answer lives in the button LABEL, never the customId — so a long option
            // can't overflow the 100-char limit (the old bug).
            var enc = new ButtonEncriptionService { CommandName = "ask_user", Action = "call_abc.0" };
            enc.UsersId.Add(123UL);
            Assert.True(enc.Encript().Length <= 100);
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
            public readonly List<(ulong ChannelId, string EffectId, string Question, IReadOnlyList<string> Options, string? AskerUserId)> Asked = new();
            public Task SendAskAsync(ulong channelId, string effectId, string question, IReadOnlyList<string> options, string? askerUserId, CancellationToken ct = default)
            { Asked.Add((channelId, effectId, question, options, askerUserId)); return Task.CompletedTask; }
        }

        private static string AskFrame(string effectId, string question, string[] options, string sessionId, string userId = "u") =>
            System.Text.Json.JsonSerializer.Serialize(new
            {
                type = "effect",
                call = new { id = effectId, name = "ask_user", arguments = new { question, options } },
                context = new { userId, sessionId },
                at = "now",
            });

        [Fact]
        public async Task Listener_routes_ask_user_to_ask_delivery_with_asker()
        {
            var ask = new FakeAskDelivery();
            var l = new HiveEffectListener("ws://unused", new FakeEffectDelivery(), null, ask);
            await l.HandleMessageAsync(AskFrame("eff-9", "Proceed?", new[] { "Yes", "No" }, "124654654000000000", "405431299323461634"));
            Assert.Single(ask.Asked);
            Assert.Equal(124654654000000000UL, ask.Asked[0].ChannelId);
            Assert.Equal("eff-9", ask.Asked[0].EffectId);
            Assert.Equal("Proceed?", ask.Asked[0].Question);
            Assert.Equal(new[] { "Yes", "No" }, ask.Asked[0].Options);
            Assert.Equal("405431299323461634", ask.Asked[0].AskerUserId);   // asker threaded through for the button restriction
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
            public Task<bool> SendEventAsync(string type, object payload, string? sessionId, CancellationToken ct = default)
                => Task.FromResult(true);
        }
    }
}
