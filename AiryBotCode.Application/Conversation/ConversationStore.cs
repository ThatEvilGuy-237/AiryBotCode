using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Conversation
{
    public interface IConversationStore
    {
        Task<AiryChannel?> ChannelAsync(ulong botId, ulong channelId);

        Task<HiveConnection?> HiveAsync(ulong botId);

        Task<bool> IsMemberAsync(ulong botId, ulong userId);

        Task RecordAsync(int channelRowId, string summary, string? error);
    }

    public sealed class ConversationStore(IServiceScopeFactory scopes) : IConversationStore
    {
        public async Task<AiryChannel?> ChannelAsync(ulong botId, ulong channelId)
        {
            using var scope = scopes.CreateScope();
            try
            {
                return await scope.ServiceProvider.GetRequiredService<IAiryChannelRepository>().GetForChannelAsync(botId, channelId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiryChannel] could not read the channels: {ex.Message}");
                return null;
            }
        }

        public async Task<HiveConnection?> HiveAsync(ulong botId)
        {
            using var scope = scopes.CreateScope();
            var bot = await scope.ServiceProvider.GetRequiredService<IBotSettingRepository>().GetBotSettingAsync(botId);
            return bot is null ? null : HiveConnection.Of(bot);
        }

        public async Task<bool> IsMemberAsync(ulong botId, ulong userId)
        {
            using var scope = scopes.CreateScope();
            try
            {
                return await scope.ServiceProvider.GetRequiredService<IUserConsentRepository>().HasConsentAsync(botId, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Consent] check failed, allowing through: {ex.Message}");
                return true;
            }
        }

        public async Task RecordAsync(int channelRowId, string summary, string? error)
        {
            using var scope = scopes.CreateScope();
            try
            {
                await scope.ServiceProvider.GetRequiredService<IAiryChannelRepository>().RecordDecisionAsync(channelRowId, DateTime.UtcNow, summary, error);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiryChannel] could not record the decision: {ex.Message}");
            }
        }
    }
}
