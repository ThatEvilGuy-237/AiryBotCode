using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;

namespace AiryBotCode.Infrastructure.Database.Repository.ChatHistory
{
    internal class ChatUserRepository : EvilRepository<ChatUser>, IChatUserRepository
    {
        public ChatUserRepository(AIDbContext context) : base(context) { }
    }
}
