using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Hive
{
    /// <summary>
    /// Stores a mini-boss answer (pushed from the Hive) onto the channel's counting
    /// row. Scopes a fresh repository per call since the listener is long-lived.
    /// </summary>
    public class CountingBossSink : ICountingBossSink
    {
        private readonly IServiceProvider _services;

        public CountingBossSink(IServiceProvider services)
        {
            _services = services;
        }

        public async Task SetBossAnswerAsync(ulong channelId, double answer, CancellationToken ct = default)
        {
            using var scope = _services.CreateScope();
            var botId = scope.ServiceProvider.GetRequiredService<IConfigurationReader>().GetBotId();
            var repo = scope.ServiceProvider.GetRequiredService<ICountingStateRepository>();

            var state = await repo.GetAsync(botId, channelId);
            if (state == null || !state.BossActive) return;   // no pending boss here

            state.BossAnswer = answer;
            await repo.SaveAsync(state);
        }
    }
}
