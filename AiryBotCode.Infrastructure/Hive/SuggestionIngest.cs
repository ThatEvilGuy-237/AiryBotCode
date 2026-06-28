using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Hive
{
    /// <summary>
    /// Writes an agent-filed <c>submit_suggestion</c> effect to the /suggestions board.
    /// Scopes a fresh repository per call (the listener is long-lived). Because the tools
    /// WS broadcasts every effect to ALL subscribed bot listeners, this singleton also
    /// de-dupes on the effect id so a single suggestion is written exactly once no matter
    /// how many bots are running in the shared host.
    /// </summary>
    public sealed class SuggestionIngest : ISuggestionIngest
    {
        private static readonly TimeSpan DedupWindow = TimeSpan.FromMinutes(10);

        private readonly IServiceProvider _services;
        private readonly object _lock = new();
        private readonly Dictionary<string, DateTimeOffset> _seen = new();

        public SuggestionIngest(IServiceProvider services)
        {
            _services = services;
        }

        public async Task SubmitAsync(SuggestionIntent intent, CancellationToken ct = default)
        {
            if (!TryReserve(intent.EffectId)) return;   // another listener already filed this one

            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ISuggestionRepository>();

            var who = string.IsNullOrWhiteSpace(intent.SubmittedBy)
                ? "via Airy"
                : $"{intent.SubmittedBy} (via Airy)";

            await repo.AddAsync(new Suggestion
            {
                Title = intent.Title,
                Body = intent.Body,
                SubmittedBy = who,
                CreatedAt = DateTime.UtcNow,
                Status = "new",
            });
        }

        /// <summary>True the first time an effect id is seen within the window; false after,
        /// so concurrent listeners don't each write the same suggestion. Thread-safe.</summary>
        private bool TryReserve(string effectId)
        {
            var now = DateTimeOffset.UtcNow;
            lock (_lock)
            {
                foreach (var k in _seen.Where(kv => now - kv.Value > DedupWindow).Select(kv => kv.Key).ToList())
                    _seen.Remove(k);

                if (_seen.ContainsKey(effectId)) return false;
                _seen[effectId] = now;
                return true;
            }
        }
    }
}
