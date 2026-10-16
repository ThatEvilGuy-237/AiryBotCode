using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using AiryBotCode.Api;
using AiryBotCode.Api.Controllers;
using AiryBotCode.Api.Services;
using AiryBotCode.Application.Hive;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiryBotCode.Tests
{
    public class AiryChannelTests
    {
        private const string AgentId = "2d7b492e-27f2-49e0-8261-c9330aa3df60";
        private static readonly HiveConnection Hive = new("http://spine:7000", AgentId, "hive_ak_test");

        private const string DecidedYes = """
        {"id":"6c1f2b9e-0000-4000-8000-000000000001","status":"decided","error":null,
         "answers":{"worth":{"type":"yesno","answer":true,"yes":0.94,"confidence":0.88,"sure":true}}}
        """;

        private sealed class Hub(Func<HttpRequestMessage, (HttpStatusCode, string)> answer) : HttpMessageHandler
        {
            public List<(HttpRequestMessage Request, string? Body)> Seen { get; } = [];

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Seen.Add((request, request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken)));
                var (status, body) = answer(request);
                return new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
            }
        }

        private sealed class Down : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
                throw new HttpRequestException("connection refused");
        }

        private static WorthEvent Event(int recent = 2) => new("Airy", "general", "kirra", 30, "does anyone know when the event starts?",
            Enumerable.Range(1, recent).Select(i => new RecentLine(i % 2 == 0 ? "Airy" : "tomas", $"line {i}")).ToList());

        [Fact]
        public void TheTablesAtStartHaveNoBraceEfWouldReadAsAParameter()
        {
            var formatted = string.Format(AIDbContext.NewerTablesSql);

            Assert.Contains("\"AiryChannels\"", formatted);
            Assert.Contains("DROP TABLE IF EXISTS \"ChannelDecisionWatches\"", formatted);
        }

        [Theory]
        [InlineData(AnswerModes.Always, false, true)]
        [InlineData(AnswerModes.Mention, false, false)]
        [InlineData(AnswerModes.Mention, true, true)]
        [InlineData(AnswerModes.Decide, true, true)]
        public void AChannelAnswersByItsRuleWithoutAskingTheHive(string mode, bool targeted, bool answers)
        {
            Assert.False(AnswerRules.Watches(mode, targeted, consented: true) && mode != AnswerModes.Decide);
            Assert.Equal(answers, AnswerRules.Answers(mode, targeted, null));
        }

        [Fact]
        public void AWorthItChannelAsksOnlyForUnaddressedMessagesFromPeopleWhoAccepted()
        {
            Assert.True(AnswerRules.Watches(AnswerModes.Decide, targeted: false, consented: true));
            Assert.False(AnswerRules.Watches(AnswerModes.Decide, targeted: true, consented: true));
            Assert.False(AnswerRules.Watches(AnswerModes.Decide, targeted: false, consented: false));
            Assert.False(AnswerRules.Watches(AnswerModes.Always, targeted: false, consented: true));
        }

        [Fact]
        public void OnlyASureYesMakesAiryAnswerAnUnaddressedMessage()
        {
            Assert.True(AnswerRules.Answers(AnswerModes.Decide, false, new WorthVerdict(true, true, 0.9, "d", null)));
            Assert.False(AnswerRules.Answers(AnswerModes.Decide, false, new WorthVerdict(true, false, 0.4, "d", null)));
            Assert.False(AnswerRules.Answers(AnswerModes.Decide, false, new WorthVerdict(false, true, 0.9, "d", null)));
            Assert.False(AnswerRules.Answers(AnswerModes.Decide, false, WorthVerdict.Failed("down")));
            Assert.True(AnswerRules.Answers(AnswerModes.Decide, true, WorthVerdict.Failed("down")));
        }

        [Fact]
        public void TheConnectionIsReadFromAnOldAgentLink()
        {
            var hive = HiveConnection.FromRunUrl($"http://spine:7000/agents/{AgentId}/run", " hive_ak_test ");

            Assert.Equal(Hive, hive);
            Assert.Equal($"http://spine:7000/agents/{AgentId}/run", hive!.RunUrl);
            Assert.Equal("http://spine:7000/v1/decisions", hive.DecisionsUrl);
            Assert.Null(HiveConnection.FromRunUrl("https://thatevilserver.example/hive/api/spine/hooks/e5b22c11", "k"));
            Assert.Null(HiveConnection.FromRunUrl($"http://spine:7000/agents/{AgentId}/run", null));
            Assert.Null(HiveConnection.Of(new BotSetting { HiveUrl = "http://spine:7000", HiveAgentId = AgentId }));
        }

        [Fact]
        public void AFollowUpAsksWhetherTheTalkWithAiryGoesOn()
        {
            var body = JsonNode.Parse(WorthQuestion.Body(Event(4) with { FollowUp = true }))!;
            var instructions = body["questions"]![WorthQuestion.Key]!["instructions"]!.GetValue<string>();
            var state = body["state"]!.GetValue<string>();
            Assert.Contains("Airy is already in this conversation", instructions);
            Assert.Contains("(Airy's lines included)", state);
            Assert.Contains("Airy: line 2", state);
            Assert.EndsWith("kirra: does anyone know when the event starts?", state);
            Assert.DoesNotContain("{{", state + instructions);
        }

        [Fact]
        public void TheQuestionCarriesTheChannelTheAuthorTheMessageAndTheRecentTalk()
        {
            var body = JsonNode.Parse(WorthQuestion.Body(Event(9)))!;
            var question = body["questions"]![WorthQuestion.Key]!;
            var state = body["state"]!.GetValue<string>();

            Assert.Equal("yesno", question["type"]!.GetValue<string>());
            Assert.Contains("Should Airy join this conversation", question["instructions"]!.GetValue<string>());
            Assert.Contains("#general", state);
            Assert.EndsWith("kirra: does anyone know when the event starts?", state);
            Assert.Contains("tomas: line 1", state);
            Assert.Contains("Airy: line 8", state);
            Assert.Contains("tomas: line 9\nkirra:", state);
            Assert.DoesNotContain("{{", state);
            Assert.Equal("discord:30", body["user"]!.GetValue<string>());
            Assert.Contains("(nothing before this)", JsonNode.Parse(WorthQuestion.Body(Event(0)))!["state"]!.GetValue<string>());
            var long_ = JsonNode.Parse(WorthQuestion.Body(Event(15)))!["state"]!.GetValue<string>();
            Assert.DoesNotContain("tomas: line 3\n", long_);
            Assert.Contains("Airy: line 4", long_);
        }

        [Fact]
        public void TheHivesAnswerIsReadIntoAVerdict()
        {
            var yes = WorthQuestion.Read(DecidedYes);
            Assert.True(yes.SaysAnswer);
            Assert.Equal("yes 88%", yes.Summary);
            Assert.Equal("6c1f2b9e-0000-4000-8000-000000000001", yes.DecisionId);

            var failed = WorthQuestion.Read("""{"id":"x","status":"failed","error":"No answer for worth: no decider.","answers":{}}""");
            Assert.Equal("No answer for worth: no decider.", failed.Error);
            Assert.Equal("failed", failed.Summary);
            Assert.NotNull(WorthQuestion.Read("<html>").Error);
            Assert.Equal("no, unsure 51%", WorthQuestion.Read("""{"status":"decided","answers":{"worth":{"answer":false,"confidence":0.51,"sure":false}}}""").Summary);
        }

        [Fact]
        public async Task AiryAsksWithItsOwnKeyAndSaysWhatARefusalMeans()
        {
            var hub = new Hub(_ => (HttpStatusCode.OK, DecidedYes));
            var verdict = await new HiveAnswerClient(new HttpClient(hub)).AskAsync(Hive, Event());

            Assert.True(verdict.SaysAnswer);
            var (request, _) = hub.Seen.Single();
            Assert.Equal("http://spine:7000/v1/decisions", request.RequestUri!.ToString());
            Assert.Equal("hive_ak_test", request.Headers.GetValues("X-Hive-App-Key").Single());

            var refused = await new HiveAnswerClient(new HttpClient(new Hub(_ => (HttpStatusCode.Forbidden, "{}")))).AskAsync(Hive, Event());
            Assert.Contains("decisions.make", refused.Error);
            var down = await new HiveAnswerClient(new HttpClient(new Down())).AskAsync(Hive, Event());
            Assert.Contains("connection refused", down.Error);
        }

        [Fact]
        public async Task TheCheckSaysWhichRightTheKeyIsMissing()
        {
            var hub = new Hub(r => r.RequestUri!.AbsolutePath switch
            {
                "/v1/agents" => (HttpStatusCode.OK, $$"""[{"id":"{{AgentId}}","name":"Airy"}]"""),
                _ => (HttpStatusCode.Forbidden, """{"error":"missing scope"}"""),
            });
            var check = await new HiveAnswerClient(new HttpClient(hub)).CheckAsync("http://spine:7000/", "hive_ak_test", AgentId);

            Assert.True(check.Reachable && check.CanRun && check.AgentFound);
            Assert.False(check.CanDecide);
            Assert.Equal("Airy", check.Agents.Single().Name);
            Assert.Contains(check.Problems, p => p.Contains("decisions.make"));

            var unknown = await new HiveAnswerClient(new HttpClient(new Hub(_ => (HttpStatusCode.Unauthorized, "{}")))).CheckAsync("http://spine:7000", "bad", null);
            Assert.Contains(unknown.Problems, p => p.Contains("does not know this key"));
            var down = await new HiveAnswerClient(new HttpClient(new Down())).CheckAsync("http://spine:7000", "k", null);
            Assert.False(down.Reachable);

            var noDecider = new Hub(r => r.RequestUri!.AbsolutePath == "/v1/agents" ? (HttpStatusCode.OK, "[]") : (HttpStatusCode.OK, "[]"));
            var empty = await new HiveAnswerClient(new HttpClient(noDecider)).CheckAsync("http://spine:7000", "k", AgentId);
            Assert.True(empty.CanDecide);
            Assert.Contains(empty.Problems, p => p.Contains("no decider"));
            Assert.Contains(empty.Problems, p => p.Contains("chosen agent"));
        }

        [Fact]
        public void TheProdAgentLinkMovesOnceAndThenNothingMoves()
        {
            var links = new List<AgentLink> { new(4, 1422972239808299098, $"http://spine:7000/agents/{AgentId}/run", "hive_ak_test", true) };

            var first = AgentLinkMove.Plan(null, [], links, requireMention: false);
            Assert.Equal(Hive, first.Connection);
            Assert.Equal(new ChannelPlan(1422972239808299098, AnswerModes.Always, true), first.Channels.Single());
            Assert.Equal([4], first.RemoveLinks);

            var again = AgentLinkMove.Plan(Hive, [1422972239808299098], [], requireMention: false);
            Assert.True(again.IsEmpty);
        }

        [Fact]
        public void AMovedLinkKeepsTheMentionRuleAndALinkOnAnotherKeyStays()
        {
            var links = new List<AgentLink>
            {
                new(1, 10, $"http://spine:7000/agents/{AgentId}/run", "hive_ak_test", true),
                new(2, 11, $"http://spine:7000/agents/{AgentId}/run", "hive_ak_other", true),
                new(3, 12, $"http://spine:7000/agents/{AgentId}/run", "hive_ak_test", false),
            };

            var move = AgentLinkMove.Plan(null, [12], links, requireMention: true);

            Assert.Equal("hive_ak_test", move.Connection!.AppKey);
            Assert.Equal(new ChannelPlan(10, AnswerModes.Mention, true), move.Channels.Single());
            Assert.Equal([1, 3], move.RemoveLinks);
            Assert.True(AgentLinkMove.Plan(null, [], [new(9, 1, "https://example.test/hook", "s", true)], false).IsEmpty);
        }

        [Fact]
        public void ChannelsShowTheirNameAndAMissingOneShowsNone()
        {
            var names = new Dictionary<string, DiscordEntity> { ["10"] = new("10", "general", "Cookie & Cream") };

            var known = AiryChannelsController.ToDto(new AiryChannel { Id = 1, ChannelId = 10, AnswerMode = AnswerModes.Decide }, names);
            var gone = AiryChannelsController.ToDto(new AiryChannel { Id = 2, ChannelId = 11, AnswerMode = AnswerModes.Mention }, names);

            Assert.Equal(("general", "Cookie & Cream"), (known.ChannelName, known.Guild));
            Assert.Null(gone.ChannelName);
            Assert.Equal("hive_ak_test…", HiveConnectionController.Hint("hive_ak_test1234567"));
        }

        private sealed class Factory(HttpMessageHandler handler) : IHttpClientFactory
        {
            public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
        }

        private static (HttpStatusCode, string) Discord(HttpRequestMessage r, ref int guildCalls, bool limited)
        {
            var path = r.RequestUri!.AbsolutePath;
            if (path.EndsWith("/users/@me/guilds"))
            {
                Interlocked.Increment(ref guildCalls);
                return limited ? (HttpStatusCode.TooManyRequests, "{}") : (HttpStatusCode.OK, """[{"id":"1","name":"Cookie & Cream"}]""");
            }
            if (path.EndsWith("/channels")) return (HttpStatusCode.OK, """[{"id":"10","name":"general","type":0}]""");
            return (HttpStatusCode.OK, "[]");
        }

        [Fact]
        public async Task CallersAtTheSameTimeShareOneDiscordFetch()
        {
            var guildCalls = 0;
            var hub = new Hub(r => Discord(r, ref guildCalls, limited: false));
            var lookup = new DiscordGuildLookup(new Factory(hub));

            var both = await Task.WhenAll(lookup.GetChannelsKnownAsync(7, "token"), lookup.GetChannelsKnownAsync(7, "token"));

            Assert.All(both, b => Assert.True(b.Known));
            Assert.Equal("#general", both[0].Channels.Single().Name);
            Assert.Equal(1, guildCalls);
        }

        [Fact]
        public async Task ARateLimitIsTriedOnceMoreAndThenSaidNotHidden()
        {
            var guildCalls = 0;
            var hub = new Hub(r => Discord(r, ref guildCalls, limited: true));
            var lookup = new DiscordGuildLookup(new Factory(hub));

            var (channels, known) = await lookup.GetChannelsKnownAsync(8, "token");

            Assert.False(known);
            Assert.Empty(channels);
            Assert.Equal(2, guildCalls);
            var dto = AiryChannelsController.ToDto(new AiryChannel { Id = 1, ChannelId = 10, AnswerMode = AnswerModes.Mention }, new Dictionary<string, DiscordEntity>(), known);
            Assert.False(dto.NamesKnown);
        }

        private sealed class Bots(BotSetting bot) : AiryBotCode.Application.Interfaces.Repository.IBotSettingRepository
        {
            public int Saves { get; private set; }
            public Task<BotSetting> GetBotSettingAsync(ulong botId) => Task.FromResult(bot);
            public Task<BotSetting> GetBotSettingAsync(string botName) => Task.FromResult(bot);
            public Task CreateBotSettingAsync(BotSetting botSetting) => Task.CompletedTask;
            public Task<BotSetting?> GetByIdAsync(object id) => Task.FromResult<BotSetting?>(bot);
            public Task<IEnumerable<BotSetting>> GetAllAsync() => Task.FromResult<IEnumerable<BotSetting>>([bot]);
            public Task<BotSetting> AddAsync(BotSetting entity) => Task.FromResult(entity);
            public Task<BotSetting> UpdateAsync(BotSetting entity) => Task.FromResult(entity);
            public Task DeleteAsync(BotSetting entity) => Task.CompletedTask;
            public Task DeleteAllAsync() => Task.CompletedTask;
            public Task SaveChangesAsync() { Saves++; return Task.CompletedTask; }
        }

        [Fact]
        public async Task AConnectionChangeIsSavedAndABlankKeyKeepsTheStoredOne()
        {
            var bot = new BotSetting { BotId = 7, BotName = "Airy", HiveUrl = "http://spine:7000", HiveAgentId = AgentId, HiveAppKey = "hive_ak_kept" };
            var bots = new Bots(bot);
            var controller = new HiveConnectionController(bots, new HiveAnswerClient(new HttpClient(new Down())));

            var put = await controller.Put("7", new HiveConnectionDto { Url = "http://127.0.0.1:7002/", AgentId = AgentId, AppKey = " " });
            Assert.IsType<NoContentResult>(put);
            Assert.Equal(("http://127.0.0.1:7002", "hive_ak_kept", 1), (bot.HiveUrl, bot.HiveAppKey, bots.Saves));

            Assert.IsType<BadRequestObjectResult>(await controller.Put("7", new HiveConnectionDto { Url = "ftp://x", AgentId = AgentId }));
            Assert.IsType<NoContentResult>(await controller.Delete("7"));
            Assert.Equal((null, 2), (bot.HiveAppKey, bots.Saves));
        }

        [Fact]
        public void AiryHearsEverythingAndAnswersWithWhatWasSaidSinceItLastSpoke()
        {
            var log = new ChannelTranscript();
            log.Hear(1, 10, new HeardLine(1, "kirra", "who is up for a quiz tonight?", false));
            log.Hear(1, 10, new HeardLine(2, "Airy", "I am!", true));
            log.Hear(1, 10, new HeardLine(3, "tomas", "same, 8 pm?", false));
            log.Hear(1, 10, new HeardLine(4, "kirra", "8 works", false));
            log.Hear(1, 10, new HeardLine(5, "tomas", "@Airy can you host it?", false));
            log.Hear(1, 11, new HeardLine(6, "other", "elsewhere", false));

            var since = log.SinceBotSpoke(1, 10, beforeMessageId: 5);
            Assert.Equal(["same, 8 pm?", "8 works"], since.Select(l => l.Text));
            Assert.Equal(["I am!", "same, 8 pm?", "8 works"], log.Recent(1, 10, 5, 3).Select(l => l.Text));
            Assert.Empty(log.SinceBotSpoke(2, 10, 5));

            var said = ChannelTalk.Message("Airy", "events", "tomas", "@Airy can you host it?", since);
            Assert.Contains("#events since Airy last spoke", said);
            Assert.Contains("tomas: same, 8 pm?", said);
            Assert.EndsWith("@Airy can you host it?", said);
            Assert.DoesNotContain("{{", said);
            Assert.Equal("tomas (42) writes in #events:\nhello", ChannelTalk.Message("Airy", "events", "tomas (42)", "hello", []));
        }

        [Fact]
        public void AChannelThatStopsListeningForgetsWhatItHeard()
        {
            var log = new ChannelTranscript();
            log.Hear(1, 10, new HeardLine(1, "kirra", "hello", false));

            Assert.True(AnswerRules.Hears(listens: true, consented: true));
            Assert.False(AnswerRules.Hears(listens: false, consented: true));
            Assert.False(AnswerRules.Hears(listens: true, consented: false));

            log.Forget(1, 10);
            Assert.Empty(log.Recent(1, 10, 0, 5));
            log.HearIfKnown(1, 10, new HeardLine(2, "Airy", "still here?", true));
            Assert.Empty(log.Recent(1, 10, 0, 5));
            var dto = AiryChannelsController.ToDto(new AiryChannel { Id = 1, ChannelId = 10, AnswerMode = AnswerModes.Mention, Enabled = false, Listens = true }, new Dictionary<string, DiscordEntity>());
            Assert.Equal((false, true), (dto.Enabled, dto.Listens));
        }

        [Fact]
        public void TheLogKeepsThirtyLinesAndTheContextStaysSmall()
        {
            var log = new ChannelTranscript();
            for (ulong i = 1; i <= 50; i++) log.Hear(1, 10, new HeardLine(i, "kirra", new string('x', 300) + i, false));
            log.Hear(1, 10, new HeardLine(50, "kirra", "duplicate", false));
            log.HearIfKnown(1, 99, new HeardLine(51, "Airy", "not a channel Airy listens in", true));

            Assert.Equal(ChannelTranscript.Keep, log.Recent(1, 10, 0, 100).Count);
            var since = log.SinceBotSpoke(1, 10, 0);
            Assert.True(since.Count <= ChannelTranscript.MaxContextLines);
            Assert.True(since.Sum(l => l.Text.Length + l.Author.Length + 2) <= ChannelTranscript.MaxContextChars);
            Assert.EndsWith("50", since[^1].Text);
            Assert.Empty(log.Recent(1, 99, 0, 5));
        }

        [Fact]
        public void TheDiscordSignInAddressComesFromTheApisOwnSettings()
        {
            var url = AuthController.AuthorizeUrl("1318870826862379018", "https://thehive.thatevilguy.com/api/auth/discord/redirect", "gate token");

            Assert.StartsWith("https://discord.com/api/oauth2/authorize?client_id=1318870826862379018", url);
            Assert.Contains("redirect_uri=https%3A%2F%2Fthehive.thatevilguy.com%2Fapi%2Fauth%2Fdiscord%2Fredirect", url);
            Assert.Contains("&scope=identify&state=gate%20token", url);
        }

        [Fact]
        public void EveryPanelControllerCanBeBuiltFromThePanelServices()
        {
            var services = new ServiceCollection()
                .AddPanelServices()
                .AddLogging()
                .AddHttpClient()
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Database:Host"] = "localhost",
                    ["Database:Port"] = "5432",
                    ["Database:Name"] = "never",
                    ["Database:User"] = "test",
                    ["Database:Password"] = "test",
                }).Build())
                .AddDbContext<AIDbContext>(o => o.UseNpgsql("Host=localhost;Database=never"));
            using var provider = services.BuildServiceProvider();
            var controllers = typeof(PanelServices).Assembly.GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            Assert.Contains(typeof(AiryChannelsController), controllers);
            foreach (var controller in controllers)
            {
                using var scope = provider.CreateScope();
                var built = ActivatorUtilities.CreateInstance(scope.ServiceProvider, controller);
                Assert.NotNull(built);
            }
        }
    }
}
