using AiryBotCode.Domain.database;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiryBotCode.Infrastructure.Database.Repository
{
    internal class ChatUserRepository : EvilRepository<ChatUser>, IChatUserRepository
    {
        public ChatUserRepository(AIDbContext context) : base(context) { }
    }
}
