using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using System;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Services.Database
{
    public class ChannelConversationService : IChannelConversationService
    {
        private readonly IChannelConversationRepository _channelConversationRepo;

        public ChannelConversationService(IChannelConversationRepository channelConversationRepo)
        {
            _channelConversationRepo = channelConversationRepo;
        }

        public async Task<ChannelConversation> GetOrCreateChannelConversationAsync(ulong channelId)
        {
            var conv = await _channelConversationRepo.GetByChannelIdAsync(channelId);
            if (conv == null)
            {
                conv = new ChannelConversation
                {
                    ChannelId = channelId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ConversationSummary = ""
                };
                await _channelConversationRepo.AddAsync(conv);
                await _channelConversationRepo.SaveChangesAsync(); // Save the new conversation
            }
            return conv;
        }
    }
}
