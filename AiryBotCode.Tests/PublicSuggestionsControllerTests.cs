using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiryBotCode.Api.Controllers;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AiryBotCode.Tests
{
    public class PublicSuggestionsControllerTests
    {
        private const string Good = "the-valid-share-code";

        private static PublicSuggestionsController Build(out FakeSuggestionRepo repo)
        {
            repo = new FakeSuggestionRepo();
            return new PublicSuggestionsController(repo, new FakeShareCode(Good));
        }

        // ── the security gate: a wrong/missing code is a 404 on BOTH verbs ──────────
        [Fact]
        public async Task List_with_bad_code_is_404()
        {
            var c = Build(out _);
            Assert.IsType<NotFoundResult>(await c.List("wrong-code"));
            Assert.IsType<NotFoundResult>(await c.List(""));
        }

        [Fact]
        public async Task Create_with_bad_code_is_404_and_writes_nothing()
        {
            var c = Build(out var repo);
            var res = await c.Create("wrong-code",
                new PublicSuggestionsController.CreateRequest { Title = "x", Body = "y" });
            Assert.IsType<NotFoundResult>(res);
            Assert.Empty(repo.Items);
        }

        [Fact]
        public async Task List_with_good_code_returns_the_board()
        {
            var c = Build(out var repo);
            repo.Items.Add(new Suggestion { Id = 1, Title = "Idea", Body = "Body" });
            var res = Assert.IsType<OkObjectResult>(await c.List(Good));
            var items = Assert.IsAssignableFrom<System.Collections.IEnumerable>(res.Value);
            Assert.Single(items.Cast<object>());
        }

        [Fact]
        public async Task Create_with_good_code_saves_and_201()
        {
            var c = Build(out var repo);
            var res = (ObjectResult)await c.Create(Good,
                new PublicSuggestionsController.CreateRequest { Title = "  Add dark mode  ", Body = "please" });
            Assert.Equal(201, res.StatusCode);
            Assert.Single(repo.Items);
            Assert.Equal("Add dark mode", repo.Items[0].Title);   // trimmed
            Assert.Equal("anonymous", repo.Items[0].SubmittedBy); // default
        }

        [Fact]
        public async Task Create_rejects_empty_or_oversized()
        {
            var c = Build(out _);
            Assert.IsType<BadRequestObjectResult>(await c.Create(Good,
                new PublicSuggestionsController.CreateRequest { Title = "   ", Body = "y" }));
            Assert.IsType<BadRequestObjectResult>(await c.Create(Good,
                new PublicSuggestionsController.CreateRequest { Title = "ok", Body = "" }));
            Assert.IsType<BadRequestObjectResult>(await c.Create(Good,
                new PublicSuggestionsController.CreateRequest { Title = new string('a', 201), Body = "y" }));
            Assert.IsType<BadRequestObjectResult>(await c.Create(Good,
                new PublicSuggestionsController.CreateRequest { Title = "ok", Body = new string('b', 4001) }));
        }

        // ── fakes ───────────────────────────────────────────────────────────────────
        private sealed class FakeShareCode : IShareCodeService
        {
            private readonly string _valid;
            public FakeShareCode(string valid) => _valid = valid;
            public Task<string> GetOrCreateAsync() => Task.FromResult(_valid);
            public Task<string> RegenerateAsync() => Task.FromResult(_valid);
            public Task<bool> ValidateAsync(string? provided) => Task.FromResult(provided == _valid);
        }

        private sealed class FakeSuggestionRepo : ISuggestionRepository
        {
            public readonly List<Suggestion> Items = new();
            public Task<IReadOnlyList<Suggestion>> GetAllAsync() => Task.FromResult((IReadOnlyList<Suggestion>)Items);
            public Task<Suggestion?> GetAsync(int id) => Task.FromResult(Items.FirstOrDefault(s => s.Id == id));
            public Task<Suggestion> AddAsync(Suggestion s) { s.Id = Items.Count + 1; Items.Add(s); return Task.FromResult(s); }
            public Task UpdateAsync(Suggestion s) => Task.CompletedTask;
            public Task<bool> DeleteAsync(int id) => Task.FromResult(Items.RemoveAll(s => s.Id == id) > 0);
            public Task<IReadOnlyList<Suggestion>> GetUnansweredAsync() => Task.FromResult((IReadOnlyList<Suggestion>)Items);
        }
    }
}
