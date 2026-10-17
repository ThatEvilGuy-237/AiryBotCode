using AiryBotCode.Application.Conversation;
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

        private static CheckRequest Event(int recent = 2) => new("Airy", "general", "kirra", 30, "does anyone know when the event starts?",
            Enumerable.Range(1, recent).Select(i => new CheckLine(i % 2 == 0 ? "Airy" : "tomas", $"line {i}")).ToList());

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
            Assert.False(ConversationRules.WaitsForCheck(mode, targeted, member: true) && mode != AnswerModes.Decide);
            Assert.Equal(answers, ConversationRules.Answers(mode, targeted, null));
        }

        [Fact]
        public void AWorthItChannelAsksOnlyForUnaddressedMessagesFromPeopleWhoAccepted()
        {
            Assert.True(ConversationRules.WaitsForCheck(AnswerModes.Decide, pinged: false, member: true));
            Assert.False(ConversationRules.WaitsForCheck(AnswerModes.Decide, pinged: true, member: true));
            Assert.False(ConversationRules.WaitsForCheck(AnswerModes.Decide, pinged: false, member: false));
            Assert.False(ConversationRules.WaitsForCheck(AnswerModes.Always, pinged: false, member: true));
        }

        [Fact]
        public void OnlyASureYesMakesAiryAnswerAnUnaddressedMessage()
        {
            Assert.True(ConversationRules.Answers(AnswerModes.Decide, false, new CheckVerdict(true, true, 0.9, "d", null)));
            Assert.False(ConversationRules.Answers(AnswerModes.Decide, false, new CheckVerdict(true, false, 0.4, "d", null)));
            Assert.False(ConversationRules.Answers(AnswerModes.Decide, false, new CheckVerdict(false, true, 0.9, "d", null)));
            Assert.False(ConversationRules.Answers(AnswerModes.Decide, false, CheckVerdict.Failed("down")));
            Assert.True(ConversationRules.Answers(AnswerModes.Decide, true, CheckVerdict.Failed("down")));
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
            var body = JsonNode.Parse(JoinCheck.Body(Event(4) with { FollowUp = true }))!;
            var instructions = body["questions"]![JoinCheck.Key]!["instructions"]!.GetValue<string>();
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
            var body = JsonNode.Parse(JoinCheck.Body(Event(9)))!;
            var question = body["questions"]![JoinCheck.Key]!;
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
            Assert.Contains("(nothing before this)", JsonNode.Parse(JoinCheck.Body(Event(0)))!["state"]!.GetValue<string>());
            var long_ = JsonNode.Parse(JoinCheck.Body(Event(15)))!["state"]!.GetValue<string>();
            Assert.DoesNotContain("tomas: line 3\n", long_);
            Assert.Contains("Airy: line 4", long_);
        }

        [Fact]
        public void TheHivesAnswerIsReadIntoAVerdict()
        {
            var yes = JoinCheck.Read(DecidedYes);
            Assert.True(yes.SaysAnswer);
            Assert.Equal("yes 88%", yes.Summary);
            Assert.Equal("6c1f2b9e-0000-4000-8000-000000000001", yes.DecisionId);

            var failed = JoinCheck.Read("""{"id":"x","status":"failed","error":"No answer for worth: no decider.","answers":{}}""");
            Assert.Equal("No answer for worth: no decider.", failed.Error);
            Assert.Equal("failed", failed.Summary);
            Assert.NotNull(JoinCheck.Read("<html>").Error);
            Assert.Equal("no, unsure 51%", JoinCheck.Read("""{"status":"decided","answers":{"worth":{"answer":false,"confidence":0.51,"sure":false}}}""").Summary);
        }

        [Fact]
        public async Task AiryAsksWithItsOwnKeyAndSaysWhatARefusalMeans()
        {
            var hub = new Hub(_ => (HttpStatusCode.OK, DecidedYes));
            var verdict = await new HiveDecisionClient(new HttpClient(hub)).AskAsync(Hive, Event());

            Assert.True(verdict.SaysAnswer);
            var (request, _) = hub.Seen.Single();
            Assert.Equal("http://spine:7000/v1/decisions", request.RequestUri!.ToString());
            Assert.Equal("hive_ak_test", request.Headers.GetValues("X-Hive-App-Key").Single());

            var refused = await new HiveDecisionClient(new HttpClient(new Hub(_ => (HttpStatusCode.Forbidden, "{}")))).AskAsync(Hive, Event());
            Assert.Contains("decisions.make", refused.Error);
            var down = await new HiveDecisionClient(new HttpClient(new Down())).AskAsync(Hive, Event());
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
            var check = await new HiveDecisionClient(new HttpClient(hub)).CheckAsync("http://spine:7000/", "hive_ak_test", AgentId);

            Assert.True(check.Reachable && check.CanRun && check.AgentFound);
            Assert.False(check.CanDecide);
            Assert.Equal("Airy", check.Agents.Single().Name);
            Assert.Contains(check.Problems, p => p.Contains("decisions.make"));

            var unknown = await new HiveDecisionClient(new HttpClient(new Hub(_ => (HttpStatusCode.Unauthorized, "{}")))).CheckAsync("http://spine:7000", "bad", null);
            Assert.Contains(unknown.Problems, p => p.Contains("does not know this key"));
            var down = await new HiveDecisionClient(new HttpClient(new Down())).CheckAsync("http://spine:7000", "k", null);
            Assert.False(down.Reachable);

            var noDecider = new Hub(r => r.RequestUri!.AbsolutePath == "/v1/agents" ? (HttpStatusCode.OK, "[]") : (HttpStatusCode.OK, "[]"));
            var empty = await new HiveDecisionClient(new HttpClient(noDecider)).CheckAsync("http://spine:7000", "k", AgentId);
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
            var controller = new HiveConnectionController(bots, new HiveDecisionClient(new HttpClient(new Down())));

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
            var log = new ChannelLog();
            log.Hear(1, 10, new LogLine(1, "kirra", "who is up for a quiz tonight?"));
            log.Hear(1, 10, new LogLine(2, "Airy", "I am!", FromAiry: true));
            log.Hear(1, 10, new LogLine(3, "tomas", "same, 8 pm?"));
            log.Hear(1, 10, new LogLine(4, "kirra", "8 works"));
            log.Hear(1, 10, new LogLine(5, "tomas", "@Airy can you host it?"));
            log.Hear(1, 11, new LogLine(6, "other", "elsewhere"));

            var since = log.SinceAiry(1, 10, exceptMessageId: 5);
            Assert.Equal(["same, 8 pm?", "8 works"], since.Select(l => l.Text));
            Assert.Equal(["same, 8 pm?", "8 works", "@Airy can you host it?"], log.Recent(1, 10, 3).Select(l => l.Text));
            Assert.Empty(log.SinceAiry(2, 10, 5));

            var said = AgentMessage.Reply("Airy", "events", "tomas", "@Airy can you host it?", since);
            Assert.Contains("#events since Airy last spoke", said);
            Assert.Contains("tomas: same, 8 pm?", said);
            Assert.EndsWith("@Airy can you host it?", said);
            Assert.DoesNotContain("{{", said);
            Assert.Equal("tomas (42) writes in #events:\nhello", AgentMessage.Reply("Airy", "events", "tomas (42)", "hello", []));
        }

        [Fact]
        public void SomeoneWhoDidNotOptInShowsAsOneLineWithoutTheirWords()
        {
            var log = new ChannelLog();
            log.Hear(1, 10, new LogLine(1, "kirra", "hi all"));
            log.Hear(1, 10, LogLine.Placeholder(2));
            log.Hear(1, 10, LogLine.Placeholder(3));
            log.Hear(1, 10, new LogLine(4, "Evil", "He laughed you out. Lol"));
            log.Hear(1, 10, LogLine.Placeholder(5));

            var lines = log.SinceAiry(1, 10, exceptMessageId: 0);
            Assert.Equal(["kirra", "someone (not opted in)", "Evil", "someone (not opted in)"], lines.Select(l => l.Author));
            Assert.Equal("(message not shared)", lines[1].Text);
            Assert.True(lines[1].NotShared);
        }

        [Fact]
        public void OnlyLinesAfterTheLastOneAiryReadAreUnseen()
        {
            var log = new ChannelLog();
            log.Hear(1, 10, new LogLine(100, "kirra", "aaaa i wish"));
            log.MarkRead(1, 10, 100);
            log.Hear(1, 10, new LogLine(101, "kirra", "turn my brain off please"));
            log.Hear(1, 10, new LogLine(102, "Airy", "I get that.", FromAiry: true));
            log.Hear(1, 10, LogLine.Placeholder(103));
            Assert.Equal(["turn my brain off please"], log.Unread(1, 10).Select(l => l.Text));
            log.MarkRead(1, 10, 101);
            Assert.Empty(log.Unread(1, 10));
            log.MarkRead(1, 10, 50);
            Assert.Empty(log.Unread(1, 10));
            Assert.True(log.Knows(1, 10));
            log.Forget(1, 10);
            Assert.False(log.Knows(1, 10));
        }

        [Theory]
        [InlineData("Isn't that right foxy fox airy", true)]
        [InlineData("AIRY what do you think", true)]
        [InlineData("the fox is back", true)]
        [InlineData("@Airy hello", false)]
        [InlineData("fairy tales and foxglove", false)]
        [InlineData("good morning all", false)]
        [InlineData("#airy-talk is quiet today", false)]
        [InlineData("look https://klipy.com/gifs/fox-dance", false)]
        [InlineData("you are a sly fox.", true)]
        [InlineData("kitsune energy", true)]
        public void HerNamesAreFoundAsWords(string text, bool found) => Assert.Equal(found, AiryNames.In(text, "Airy"));

        [Fact]
        public void AChannelThatStopsListeningForgetsWhatItHeard()
        {
            var log = new ChannelLog();
            log.Hear(1, 10, new LogLine(1, "kirra", "hello"));

            Assert.True(ConversationRules.Hears(listens: true, member: true));
            Assert.False(ConversationRules.Hears(listens: false, member: true));
            Assert.False(ConversationRules.Hears(listens: true, member: false));

            log.Forget(1, 10);
            Assert.Empty(log.Recent(1, 10, 5));
            log.HearOwn(1, 10, new LogLine(2, "Airy", "still here?", FromAiry: true));
            Assert.Empty(log.Recent(1, 10, 5));
            var dto = AiryChannelsController.ToDto(new AiryChannel { Id = 1, ChannelId = 10, AnswerMode = AnswerModes.Mention, Enabled = false, Listens = true }, new Dictionary<string, DiscordEntity>());
            Assert.Equal((false, true), (dto.Enabled, dto.Listens));
        }

        [Fact]
        public void TheLogKeepsThirtyLinesAndTheContextStaysSmall()
        {
            var log = new ChannelLog();
            for (ulong i = 1; i <= 50; i++) log.Hear(1, 10, new LogLine(i, "kirra", new string('x', 300) + i));
            log.Hear(1, 10, new LogLine(50, "kirra", "duplicate"));
            log.HearOwn(1, 99, new LogLine(51, "Airy", "not a channel Airy listens in", FromAiry: true));

            Assert.Equal(ConversationSettings.Default.LogLines, log.Recent(1, 10, 100).Count);
            var since = log.SinceAiry(1, 10, 0);
            Assert.True(since.Count <= ConversationSettings.Default.ContextLines);
            Assert.True(since.Sum(l => l.Text.Length + l.Author.Length + 2) <= ConversationSettings.Default.ContextChars);
            Assert.EndsWith("50", since[^1].Text);
            Assert.Empty(log.Recent(1, 99, 5));
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
