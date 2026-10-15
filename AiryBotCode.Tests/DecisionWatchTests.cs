using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using AiryBotCode.Api.Controllers;
using AiryBotCode.Application.Hive;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Xunit;

namespace AiryBotCode.Tests
{
    public class DecisionWatchTests
    {
        private const string Decided = """
        {
          "id": "6c1f2b9e-0000-4000-8000-000000000001", "status": "decided", "error": null,
          "decider": { "id": "d", "name": "laya-local" },
          "answers": {
            "forAiry": { "type": "yesno", "answer": true, "yes": 0.96, "probabilities": { "yes": 0.96, "no": 0.04 }, "confidence": 0.92, "sure": true,
                         "by": { "decider": "laya-local", "engine": "jev", "method": "jev", "model": "multilingual" } },
            "kind": { "type": "choice", "choice": "spam", "probabilities": { "chat": 0.02, "question": 0.01, "spam": 0.97 }, "confidence": 0.955, "sure": true,
                      "by": { "decider": "laya-local", "engine": "jev", "method": "jev", "model": "multilingual" } },
            "pushy": { "type": "score", "level": 1, "score": 1.2, "probabilities": null, "confidence": 0.6, "sure": false,
                       "by": { "decider": "gemma-local", "engine": "openai", "method": "structured", "model": "ai/gemma3:1b-q4_K_M" } }
          },
          "escalated": ["pushy"], "usage": { "inputTokens": 382, "outputTokens": 66, "cost": null }, "durationMs": 10145
        }
        """;

        private static ChannelDecisionWatch Watch(string actions, string unsure = WatchActions.Ignore, string? key = null) => new()
        {
            Id = 1,
            BotId = 10,
            ChannelId = 20,
            Name = "Talks to Airy",
            ListenerUrl = "http://hive.test/api/spine/hooks/decisions/t/l",
            Secret = "whsec_test",
            QuestionKey = key,
            ActionsJson = actions,
            UnsureAction = unsure,
        };

        private static readonly WatchEvent Event = new("hey airy, when does the event start?", "general", 20, "kirra", 30, "Airy", false, 40, 50);

        private sealed class Listener(HttpStatusCode status, string body) : HttpMessageHandler
        {
            public HttpRequestMessage? Request { get; private set; }
            public string? Sent { get; private set; }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Request = request;
                Sent = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
                return new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
            }
        }

        [Fact]
        public void TheEventCarriesTheMessageAndWhoWroteIt()
        {
            var body = JsonNode.Parse(DecisionWatchRules.Body(Event))!;

            Assert.Equal("hey airy, when does the event start?", body["message"]!["text"]!.GetValue<string>());
            Assert.Equal("Airy", body["message"]!["bot"]!.GetValue<string>());
            Assert.False(body["message"]!["repliesToBot"]!.GetValue<bool>());
            Assert.Equal("kirra", body["author"]!.GetValue<string>());
            Assert.Equal("30", body["authorId"]!.GetValue<string>());
            Assert.Equal("50", body["messageId"]!.GetValue<string>());
        }

        [Fact]
        public void TheVerdictIsTheNamedQuestionOrTheFirstOne()
        {
            var first = DecisionWatchRules.Read(Decided, null);
            Assert.Equal(("yes", true, 0.92), (first.Answer, first.Sure, first.Confidence));
            Assert.Equal("6c1f2b9e-0000-4000-8000-000000000001", first.DecisionId);

            Assert.Equal("spam", DecisionWatchRules.Read(Decided, "kind").Answer);
            var score = DecisionWatchRules.Read(Decided, "pushy");
            Assert.Equal(("1", false), (score.Answer, score.Sure));
            Assert.Contains("no answer to 'mood'", DecisionWatchRules.Read(Decided, "mood").Error);
        }

        [Fact]
        public void AFailedOrBrokenDecisionIsAnErrorNotAnAnswer()
        {
            var failed = DecisionWatchRules.Read("""{"id":"x","status":"failed","error":"No answer for kind: Jev refused the key (401).","answers":{}}""", null);
            Assert.Null(failed.Answer);
            Assert.Contains("Jev refused the key", failed.Error);
            Assert.NotNull(DecisionWatchRules.Read("<html>", null).Error);
            Assert.Equal(WatchActions.Ignore, DecisionWatchRules.ActionFor(Watch("""{"spam":"alert"}"""), failed).Kind);
        }

        [Fact]
        public void ASureAnswerTakesItsActionAndAnUnsureOneTheUnsureAction()
        {
            var watch = Watch("""{"yes":"answer","no":"ignore"}""", unsure: "react:🤔");

            Assert.Equal(WatchActions.Answer, DecisionWatchRules.ActionFor(watch, new WatchVerdict("yes", true, 0.9, null, "d")).Kind);
            Assert.Equal(WatchActions.Ignore, DecisionWatchRules.ActionFor(watch, new WatchVerdict("maybe", true, 0.9, null, "d")).Kind);
            var unsure = DecisionWatchRules.ActionFor(watch, new WatchVerdict("yes", false, 0.4, null, "d"));
            Assert.Equal(("react", "🤔"), (unsure.Kind, unsure.Emoji));
        }

        [Fact]
        public void OnlyAWatchThatCanAnswerDecidesWhetherAiryAnswers()
        {
            var talks = Watch("""{"yes":"answer","no":"ignore"}""");
            var spam = Watch("""{"spam":"alert"}""");
            var ignore = new WatchAction(WatchActions.Ignore, null);

            Assert.True(DecisionWatchRules.Engage([(spam, ignore), (talks, new WatchAction(WatchActions.Answer, null))]));
            Assert.False(DecisionWatchRules.Engage([(spam, ignore), (talks, ignore)]));
            Assert.Null(DecisionWatchRules.Engage([(spam, new WatchAction(WatchActions.Alert, null))]));
            Assert.Null(DecisionWatchRules.Engage([]));
        }

        [Fact]
        public void ActionsAreReadStrictly()
        {
            Assert.True(WatchActions.IsValid("answer"));
            Assert.True(WatchActions.IsValid("react:👀"));
            Assert.False(WatchActions.IsValid("react:"));
            Assert.False(WatchActions.IsValid("ban"));
            Assert.Equal(WatchActions.Ignore, WatchActions.Parse("ban").Kind);
            Assert.Empty(DecisionWatchRules.Actions("not json"));
        }

        [Fact]
        public async Task TheClientSignsTheEventTheWayTheHiveChecksIt()
        {
            var listener = new Listener(HttpStatusCode.OK, Decided);
            var client = new HiveDecisionClient(new HttpClient(listener));

            var verdict = await client.DecideAsync(Watch("""{"yes":"answer"}"""), Event);

            Assert.Equal("yes", verdict.Answer);
            var request = listener.Request!;
            Assert.Equal("http://hive.test/api/spine/hooks/decisions/t/l", request.RequestUri!.ToString());
            var timestamp = request.Headers.GetValues("X-Hive-Timestamp").Single();
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("whsec_test"));
            var expected = "sha256=" + Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes($"{timestamp}.{listener.Sent}"))).ToLowerInvariant();
            Assert.Equal(expected, request.Headers.GetValues("X-Hive-Signature").Single());
        }

        [Fact]
        public async Task ARefusingListenerIsAnErrorTheWatchIgnores()
        {
            var client = new HiveDecisionClient(new HttpClient(new Listener(HttpStatusCode.Unauthorized, """{"error":"This signed request was already received.","code":"unauthorized"}""")));

            var verdict = await client.DecideAsync(Watch("""{"yes":"answer"}"""), Event);

            Assert.Contains("401", verdict.Error);
            Assert.Equal(WatchActions.Ignore, DecisionWatchRules.ActionFor(Watch("""{"yes":"answer"}"""), verdict).Kind);
        }

        [Fact]
        public void TheTablesAtStartHaveNoBraceEfWouldReadAsAParameter()
        {
            var formatted = string.Format(AIDbContext.NewerTablesSql);

            Assert.Contains("\"ChannelDecisionWatches\"", formatted);
        }

        [Fact]
        public void ThePanelCannotSaveAWatchThatCannotWork()
        {
            var good = new ChannelDecisionWatchDto { ChannelId = "20", Name = "Spam check", ListenerUrl = "https://thehive.example/hive/api/spine/hooks/decisions/t/l", Actions = new() { ["spam"] = "alert" } };

            Assert.Null(ChannelDecisionWatchesController.FromDto(10, 0, good, null).Problem);
            Assert.Contains("listenerUrl", ChannelDecisionWatchesController.FromDto(10, 0, new ChannelDecisionWatchDto { ChannelId = "20", ListenerUrl = "ftp://x" }, null).Problem);
            Assert.Contains("'spam'", ChannelDecisionWatchesController.FromDto(10, 0, new ChannelDecisionWatchDto { ChannelId = "20", ListenerUrl = "https://x.test", Actions = new() { ["spam"] = "ban" } }, null).Problem);
            Assert.Contains("unsureAction", ChannelDecisionWatchesController.FromDto(10, 0, new ChannelDecisionWatchDto { ChannelId = "20", ListenerUrl = "https://x.test", UnsureAction = "shout" }, null).Problem);
            Assert.Equal("whsec_kept", ChannelDecisionWatchesController.FromDto(10, 3, good, "whsec_kept").Watch!.Secret);
        }
    }
}
