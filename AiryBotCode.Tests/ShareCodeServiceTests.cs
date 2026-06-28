using System;
using System.Threading.Tasks;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Services;
using AiryBotCode.Domain.database;
using Xunit;

namespace AiryBotCode.Tests
{
    // Covers the service the dashboard share-link endpoints (phase 4) call into.
    public class ShareCodeServiceTests
    {
        [Fact]
        public async Task GetOrCreate_generates_persists_and_is_stable()
        {
            var svc = new ShareCodeService(new InMemoryShareCodeRepo());

            var first = await svc.GetOrCreateAsync();
            Assert.False(string.IsNullOrEmpty(first));
            Assert.Equal(first, await svc.GetOrCreateAsync());   // same code on re-get
            Assert.True(await svc.ValidateAsync(first));
            Assert.False(await svc.ValidateAsync("not-the-code"));
        }

        [Fact]
        public async Task Regenerate_changes_the_code_and_kills_the_old_one()
        {
            var svc = new ShareCodeService(new InMemoryShareCodeRepo());

            var old = await svc.GetOrCreateAsync();
            var fresh = await svc.RegenerateAsync();

            Assert.NotEqual(old, fresh);
            Assert.True(await svc.ValidateAsync(fresh));
            Assert.False(await svc.ValidateAsync(old));   // old share link is dead
        }

        private sealed class InMemoryShareCodeRepo : ISuggestionShareCodeRepository
        {
            private SuggestionShareCode? _row;
            public Task<SuggestionShareCode?> GetAsync() => Task.FromResult(_row);
            public Task SetAsync(string code)
            {
                _row = new SuggestionShareCode { Id = 1, Code = code, CreatedAt = DateTime.UtcNow };
                return Task.CompletedTask;
            }
        }
    }
}
