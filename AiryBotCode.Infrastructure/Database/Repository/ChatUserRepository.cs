using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Interfaces;
using AiryBotCode.Infrastructure.Database.Persistence;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    internal class ChatUserRepository : EvilRepository<ChatUser>, IChatUserRepository
    {
        public ChatUserRepository(AIDbContext context) : base(context) { }
    }
}
