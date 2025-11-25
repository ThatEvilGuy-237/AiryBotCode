using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence;

namespace AiryBotCode.Infrastructure.Database.Repository.GiveAway
{
    internal class GiveAwayUserRepository : EvilRepository<GiveAwayUser>, IGiveAwayUserRepository
    {
        public GiveAwayUserRepository(AIDbContext context) : base(context) { }
    }
}
