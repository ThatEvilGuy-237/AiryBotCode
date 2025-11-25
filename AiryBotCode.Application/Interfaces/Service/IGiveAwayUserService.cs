using AiryBotCode.Domain.database;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Interfaces.Service
{
    public interface IGiveAwayUserService
    {
        Task<List<GiveAwayUser>> GetRandomUsers(int ammount);
        Task<List<GiveAwayUser>> GetAllUsers();
        Task CreateUser(GiveAwayUser user);
        Task<bool> IsUserRegistered(ulong userId);
    }
}
