using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Hive;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiryBotCode.Tests
{
    public class SuggestionEffectRouterTests
    {
        [Fact]
        public void Routes_a_well_formed_suggestion()
        {
            var i = SuggestionEffectRouter.Route("submit_suggestion", "e1", "Add /leaderboard reset", "It would help mods", "Bob");
            Assert.NotNull(i);
            Assert.Equal("e1", i!.EffectId);
            Assert.Equal("Add /leaderboard reset", i.Title);
            Assert.Equal("It would help mods", i.Body);
            Assert.Equal("Bob", i.SubmittedBy);
        }

        [Fact]
        public void Null_for_wrong_name_or_missing_required_fields()
        {
            Assert.Null(SuggestionEffectRouter.Route("say", "e1", "t", "b", null));
            Assert.Null(SuggestionEffectRouter.Route("submit_suggestion", "", "t", "b", null));      // no effect id
            Assert.Null(SuggestionEffectRouter.Route("submit_suggestion", "e1", "  ", "b", null));    // blank title
            Assert.Null(SuggestionEffectRouter.Route("submit_suggestion", "e1", "t", "   ", null));   // blank body
        }

        [Fact]
        public void Trims_and_truncates_title_to_200_and_nulls_blank_submitter()
        {
            var longTitle = new string('x', 250);
            var i = SuggestionEffectRouter.Route("submit_suggestion", " e1 ", longTitle, " body ", "   ");
            Assert.NotNull(i);
            Assert.Equal(200, i!.Title.Length);
            Assert.Equal("body", i.Body);
            Assert.Null(i.SubmittedBy);
        }
    }

    public class SuggestionListenerRoutingTests
    {
        private sealed class FakeIngest : ISuggestionIngest
        {
            public readonly List<SuggestionIntent> Got = new();
            public Task SubmitAsync(SuggestionIntent intent, CancellationToken ct = default)
            { Got.Add(intent); return Task.CompletedTask; }
        }

        private sealed class NoopDelivery : IEffectDelivery
        {
            public Task SendAsync(ulong channelId, string text, CancellationToken ct = default) => Task.CompletedTask;
        }

        private static string Frame(string name, string id, object args) =>
            System.Text.Json.JsonSerializer.Serialize(new
            {
                type = "effect",
                call = new { id, name, arguments = args },
                context = new { userId = "405431299323461634", sessionId = "124654654000000000" },
                at = "now",
            });

        [Fact]
        public async Task Routes_submit_suggestion_to_the_ingest()
        {
            var ingest = new FakeIngest();
            var l = new HiveEffectListener("ws://unused", new NoopDelivery(), null, null, null, ingest);
            await l.HandleMessageAsync(Frame("submit_suggestion", "eff-1",
                new { title = "Dark mode", body = "Easier on the eyes at night", submittedBy = "Cookie" }));

            Assert.Single(ingest.Got);
            Assert.Equal("eff-1", ingest.Got[0].EffectId);
            Assert.Equal("Dark mode", ingest.Got[0].Title);
            Assert.Equal("Cookie", ingest.Got[0].SubmittedBy);
        }

        [Fact]
        public async Task Ignores_suggestion_missing_body()
        {
            var ingest = new FakeIngest();
            var l = new HiveEffectListener("ws://unused", new NoopDelivery(), null, null, null, ingest);
            await l.HandleMessageAsync(Frame("submit_suggestion", "eff-2", new { title = "No body here" }));
            Assert.Empty(ingest.Got);
        }
    }

    public class SuggestionIngestTests
    {
        private sealed class FakeRepo : ISuggestionRepository
        {
            public readonly List<Suggestion> Added = new();
            public Task<Suggestion> AddAsync(Suggestion suggestion) { Added.Add(suggestion); return Task.FromResult(suggestion); }
            public Task<IReadOnlyList<Suggestion>> GetAllAsync() => Task.FromResult((IReadOnlyList<Suggestion>)Added);
            public Task<Suggestion?> GetAsync(int id) => Task.FromResult<Suggestion?>(null);
            public Task UpdateAsync(Suggestion suggestion) => Task.CompletedTask;
            public Task<bool> DeleteAsync(int id) => Task.FromResult(false);
            public Task<IReadOnlyList<Suggestion>> GetUnansweredAsync() => Task.FromResult((IReadOnlyList<Suggestion>)Added);
        }

        private static (SuggestionIngest ingest, FakeRepo repo) Build()
        {
            var repo = new FakeRepo();
            var sp = new ServiceCollection()
                .AddScoped<ISuggestionRepository>(_ => repo)
                .BuildServiceProvider();
            return (new SuggestionIngest(sp), repo);
        }

        [Fact]
        public async Task Writes_a_new_row_with_via_airy_attribution()
        {
            var (ingest, repo) = Build();
            await ingest.SubmitAsync(new SuggestionIntent("e1", "Title", "Body", "Bob"));
            Assert.Single(repo.Added);
            Assert.Equal("Title", repo.Added[0].Title);
            Assert.Equal("Bob (via Airy)", repo.Added[0].SubmittedBy);
            Assert.Equal("new", repo.Added[0].Status);
        }

        [Fact]
        public async Task Falls_back_to_via_airy_when_no_submitter()
        {
            var (ingest, repo) = Build();
            await ingest.SubmitAsync(new SuggestionIntent("e1", "Title", "Body", null));
            Assert.Equal("via Airy", repo.Added[0].SubmittedBy);
        }

        [Fact]
        public async Task Dedupes_the_same_effect_id_across_listeners()
        {
            var (ingest, repo) = Build();
            // Simulate two bot listeners receiving the same broadcast effect.
            await ingest.SubmitAsync(new SuggestionIntent("dup", "T", "B", null));
            await ingest.SubmitAsync(new SuggestionIntent("dup", "T", "B", null));
            Assert.Single(repo.Added);

            // A different effect id is a different suggestion.
            await ingest.SubmitAsync(new SuggestionIntent("other", "T2", "B2", null));
            Assert.Equal(2, repo.Added.Count);
        }
    }
}
