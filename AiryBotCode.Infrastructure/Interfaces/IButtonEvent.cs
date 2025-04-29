
using AiryBotCode.Application.Services;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Interfaces
{
    public interface IButtonEvent
    {
        Task HandleButtonPressAsync(SocketMessageComponent component, ButtonEncriptionService buttonEncription);
    }
}
