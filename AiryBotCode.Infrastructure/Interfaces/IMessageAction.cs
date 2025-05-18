
using AiryBotCode.Application.Services;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Interfaces
{
    public interface IMessageAction
    {
        Task HandleMessageReceivedAsync(SocketMessage message);
    }
}
