using AiryBotCode.Application.Services;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace AiryBotCode.Infrastructure.Interfaces
{
    public interface IButtonAction
    {
        Task HandleButtonPressAsync(SocketMessageComponent component, ButtonEncriptionService buttonEncription);
    }
}