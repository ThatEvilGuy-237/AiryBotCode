using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Interfaces
{
    /// <summary>A member left the guild — voluntarily or via kick (Discord has no
    /// kick event; implementations check the audit log to tell them apart).</summary>
    public interface ILeftAction
    {
        Task HandleLeftAsync(SocketGuild guild, SocketUser user);
    }
}
