using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Interfaces
{
    public interface ISlashAction
    {
        Task RegisterCommandAsync(IReadOnlyCollection<SocketGuild?> socketGuilds);
        Task RemoveCommandAsync(IReadOnlyCollection<SocketGuild?> socketGuilds);
        Task ExecuteSlashCommandAsync(SocketSlashCommand command);
    }

}
