using AiryBotCode.Application.Services;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Interfaces
{
    public interface IFormAction
    {
        Task HandleFormAsync(SocketModal modal, ButtonEncriptionService buttonEncription);
    }

}
