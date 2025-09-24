
using AiryBotCode.Application.Services;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Interfaces
{
    public interface IButtonAction
    {
        Task HandleButtonPressAsync(SocketMessageComponent component, ButtonEncriptionService buttonEncription);
    }
}
