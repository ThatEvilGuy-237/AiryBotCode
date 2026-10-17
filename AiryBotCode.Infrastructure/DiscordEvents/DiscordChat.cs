using AiryBotCode.Application.Conversation;
using AiryBotCode.Application.Hive;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public static class DiscordChat
    {
        public static ChatMessage Read(SocketMessage message, ulong botId, IReadOnlyList<EmbedSeen>? embeds = null)
        {
            embeds ??= EmbedsOf(message);
            var stickers = message.Stickers.Select(s => new StickerSeen(s.Id, s.Name, (int)s.Format)).ToList();
            var mentioned = message.MentionedUsers.GroupBy(u => u.Id).ToDictionary(g => g.Key, g => NameOf(g.First()));
            var text = Names(message.Content, (message.Channel as SocketGuildChannel)?.Guild, botId, mentioned);
            var pinged = message.MentionedUsers.Any(u => u.Id == botId)
                || (message.Reference is not null && (message as SocketUserMessage)?.ReferencedMessage?.Author?.Id == botId);
            return new ChatMessage(
                message.Id,
                botId,
                message.Channel.Id,
                message.Channel.Name,
                message.Author.Id,
                NameOf(message.Author),
                message.Author.Username,
                ChatPictures.Readable(text, stickers, embeds),
                message.Content,
                pinged,
                ImageAttachments.Pick(message.Attachments.Select(a => ((string?)a.Url, (string?)a.Filename, (string?)a.ContentType))),
                stickers,
                embeds);
        }

        public static string Names(string content, SocketGuild? guild, ulong botId, IReadOnlyDictionary<ulong, string>? mentioned = null) =>
            MentionResolver.Rewrite(content, id =>
                mentioned is not null && mentioned.TryGetValue(id, out var n) ? n : guild?.GetUser(id) is { } user ? NameOf(user) : null, botId);

        public static IReadOnlyList<EmbedSeen> EmbedsOf(IMessage message) =>
            message.Embeds.Select(e => new EmbedSeen(e.Type.ToString(), e.Url, e.Thumbnail?.Url ?? e.Image?.Url)).ToList();

        public static string NameOf(IUser user) => (user as SocketGuildUser)?.Nickname ?? user.GlobalName ?? user.Username;
    }
}
