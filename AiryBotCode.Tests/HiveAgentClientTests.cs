using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Services;
using AiryBotCode.Domain.database;
using Xunit;

namespace AiryBotCode.Tests
{
    public class HiveAgentClientTests
    {
        private const string RunUrl = "http://spine:7000/agents/00000000-0000-4000-8000-000000000001/run";
        private const string AppKey = "hive_ak_0123456789abcdef_testkey";

        private sealed class Spine(HttpStatusCode status, string body) : HttpMessageHandler
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

        private sealed class Links(ChannelWebhook? link) : IChannelWebhookRepository
        {
            public Task<List<ChannelWebhook>> GetForBotAsync(ulong botId) => Task.FromResult(link is null ? new List<ChannelWebhook>() : [link]);
            public Task<ChannelWebhook?> GetForChannelAsync(ulong botId, ulong channelId) => Task.FromResult(link);
            public Task<ChannelWebhook> AddAsync(ChannelWebhook l) => Task.FromResult(l);
            public Task<bool> UpdateAsync(ChannelWebhook l) => Task.FromResult(true);
            public Task<bool> DeleteAsync(ulong botId, int id) => Task.FromResult(true);
        }

        private static ChannelWebhook AgentLink(string? appKey = AppKey)
            => new() { Id = 1, BotId = 111111111111111111, ChannelId = 222222222222222222, Name = "Airy", WebhookUrl = RunUrl, Secret = appKey, Mode = WebhookChatService.AgentMode };

        [Fact]
        public async Task A_chat_message_becomes_an_agent_run_with_the_app_key()
        {
            var spine = new Spine(HttpStatusCode.OK, "{\"text\":\"hi there\",\"hadErrors\":false}");
            var client = new HiveAgentClient(new HttpClient(spine));

            var reply = await client.RunAsync(RunUrl, AppKey, 111111111111111111, 222222222222222222, 42, "Thatevilguy", "hello",
                [new ForwardedImage("https://cdn.example/cat.png", "cat.png", "image/png")]);

            Assert.Equal(new AgentReply("hi there", false), reply);
            Assert.Equal(HttpMethod.Post, spine.Request!.Method);
            Assert.Equal(RunUrl, spine.Request.RequestUri!.ToString());
            Assert.Equal(AppKey, spine.Request.Headers.GetValues(HiveAgentClient.AppKeyHeader).Single());
            using var sent = JsonDocument.Parse(spine.Sent!);
            var root = sent.RootElement;
            Assert.Equal("hello", root.GetProperty("message").GetString());
            Assert.Equal("222222222222222222", root.GetProperty("sessionId").GetString());
            Assert.Equal("42", root.GetProperty("userId").GetString());
            Assert.Equal("Thatevilguy", root.GetProperty("author").GetString());
            Assert.Equal("111111111111111111", root.GetProperty("botId").GetString());
            Assert.Equal("cat.png", root.GetProperty("images")[0].GetProperty("name").GetString());
        }

        [Theory]
        [InlineData("{\"text\":\"hello\"}", "hello", false)]
        [InlineData("{\"text\":null,\"hadErrors\":true,\"error\":\"model refused\"}", null, true)]
        [InlineData("{\"text\":\"\",\"hadErrors\":false}", null, false)]
        [InlineData("not json", null, true)]
        public void The_reply_is_read_from_the_run_output(string body, string? text, bool failed)
            => Assert.Equal(new AgentReply(text, failed), HiveAgentClient.Read(body));

        [Fact]
        public async Task A_refused_run_counts_as_a_failure()
        {
            var client = new HiveAgentClient(new HttpClient(new Spine(HttpStatusCode.Forbidden, "{\"error\":\"This app key lacks the required scope 'agents.run'.\"}")));

            var reply = await client.RunAsync(RunUrl, AppKey, 1, 2, 3, "a", "b", null);

            Assert.Equal(new AgentReply(null, true), reply);
        }

        [Fact]
        public async Task A_linked_channel_gets_the_agents_answer()
        {
            var service = new WebhookChatService(new Links(AgentLink()), new HiveAgentClient(new HttpClient(new Spine(HttpStatusCode.OK, "{\"text\":\"*swishes tail*\"}"))));

            Assert.Equal("*swishes tail*", await service.TryForwardAsync(111111111111111111, 222222222222222222, 42, "Thatevilguy", "hey Airy"));
        }

        [Fact]
        public async Task A_failed_run_says_the_bot_is_offline_and_a_quiet_one_says_nothing()
        {
            var failed = new WebhookChatService(new Links(AgentLink()), new HiveAgentClient(new HttpClient(new Spine(HttpStatusCode.BadGateway, "{}"))));
            var quiet = new WebhookChatService(new Links(AgentLink()), new HiveAgentClient(new HttpClient(new Spine(HttpStatusCode.OK, "{\"text\":\"\"}"))));

            Assert.Contains("servers are down", await failed.TryForwardAsync(1, 2, 3, "a", "b"));
            Assert.Null(await quiet.TryForwardAsync(1, 2, 3, "a", "b"));
        }

        [Fact]
        public async Task An_agent_link_without_an_app_key_never_calls_the_hive()
        {
            var spine = new Spine(HttpStatusCode.OK, "{\"text\":\"should not happen\"}");
            var service = new WebhookChatService(new Links(AgentLink(appKey: null)), new HiveAgentClient(new HttpClient(spine)));

            Assert.Contains("servers are down", await service.TryForwardAsync(1, 2, 3, "a", "b"));
            Assert.Null(spine.Request);
        }

        [Fact]
        public async Task An_unlinked_channel_stays_silent()
        {
            var service = new WebhookChatService(new Links(null), new HiveAgentClient(new HttpClient(new Spine(HttpStatusCode.OK, "{}"))));

            Assert.Null(await service.TryForwardAsync(1, 2, 3, "a", "b"));
        }
    }
}
