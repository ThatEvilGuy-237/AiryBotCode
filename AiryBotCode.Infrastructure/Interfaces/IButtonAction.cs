
using AiryBotCode.Application.Services;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Interfaces
{
    public interface IBanAction
    {
        Task HandleBanAsync(SocketUser user, SocketGuild guild);
    }
}
