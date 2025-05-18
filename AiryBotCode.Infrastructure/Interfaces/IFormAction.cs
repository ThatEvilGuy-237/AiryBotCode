using AiryBotCode.Application.Services;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Interfaces
{
    public interface IFormAction
    {
        Task HanndelFormAsync(SocketModal modal, ButtonEncriptionService buttonEncription);
    }

}
