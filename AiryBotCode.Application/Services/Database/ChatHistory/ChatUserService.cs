using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using System;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Services.Database.ChatHistory
{
    public class ChatUserService : IChatUserService
    {
        private readonly IChatUserRepository _chatUserRepo;

        public ChatUserService(IChatUserRepository chatUserRepo)
        {
            _chatUserRepo = chatUserRepo;
        }

        public async Task<ChatUser> GetOrCreateChatUserAsync(ulong userId, string userName, ChatRole role, string defaultAiOpinion = "A friendly Discord user")
        {
            var user = await _chatUserRepo.GetByIdAsync(userId);
            if (user == null)
            {
                user = new ChatUser
                {
                    Id = userId,
                    UserName = userName,
                    Role = role,
                    CreatedAt = DateTime.UtcNow,
                    AiOpinion = defaultAiOpinion
                };
                await _chatUserRepo.AddAsync(user);
                await _chatUserRepo.SaveChangesAsync();
            }
            return user;
        }

        public async Task<ChatUser> GetOrCreateSystemUserAsync()
        {
            return await GetOrCreateChatUserAsync((ulong)1, "System", ChatRole.System, "System user");
        }

        public async Task<ChatUser> GetOrCreateAiUserAsync(ulong botId)
        {
            return await GetOrCreateChatUserAsync(botId, "Asistent", ChatRole.Assistant, "you*");
        }

        public async Task UpdateUserAiOpinionAsync(ChatUser user, string aiOpinion)
        {
            user.AiOpinion = aiOpinion;
            await _chatUserRepo.UpdateAsync(user);
            await _chatUserRepo.SaveChangesAsync();
        }
    }
}
