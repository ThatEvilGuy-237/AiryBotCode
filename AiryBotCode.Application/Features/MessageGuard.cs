using Discord.WebSocket;

namespace AiryBotCode.Application.Features
{
    /// <summary>
    /// Shared guard for message-driven features (spam catcher, leveling, counting):
    /// accepts only a real user's message posted in a guild text channel, yielding
    /// the guild member + channel. Bots, DMs and system messages are rejected.
    /// </summary>
    public static class MessageGuard
    {
        public static bool TryGuildMessage(SocketMessage message, out SocketGuildUser member, out SocketGuildChannel channel)
        {
            member = null!;
            channel = null!;
            if (message.Author.IsBot) return false;
            if (message is not SocketUserMessage) return false;
            if (message.Channel is not SocketGuildChannel ch) return false;
            if (message.Author is not SocketGuildUser m) return false;
            member = m;
            channel = ch;
            return true;
        }
    }
}
