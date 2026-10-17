using System.Text;
using AiryBotCode.Application.Consent;
using AiryBotCode.Application.Conversation;
using AiryBotCode.Application.Frontend;
using AiryBotCode.Application.Hive;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public sealed class DiscordChatChannel(SocketMessage message, ulong botId, ulong evilId, RecentMessageDedup dedup, ConversationSettings settings)
        : IChatChannel
    {
        public string BotName => (message.Channel as SocketGuildChannel)?.Guild.CurrentUser is { } me ? DiscordChat.NameOf(me) : "Airy";

        public IDisposable Typing() => message.Channel.EnterTypingState();

        public async Task<IReadOnlyList<SentMessage>> SendAsync(string text)
        {
            var sent = new List<SentMessage>();
            var guild = (message.Channel as SocketGuildChannel)?.Guild;
            foreach (var chunk in Split(text))
            {
                if (!dedup.TryReserve(message.Channel.Id, chunk))
                {
                    Console.WriteLine($"[AiryChannel] reply chunk to {message.Channel.Id} skipped: already said through an effect.");
                    continue;
                }
                var posted = await message.Channel.SendMessageAsync(chunk);
                sent.Add(new SentMessage(posted.Id, ChatPictures.Readable(DiscordChat.Names(chunk, guild, botId), [], [])));
            }
            return sent;
        }

        public async Task AskConsentAsync()
        {
            var embed = ConsentFrontend.ConsentEmbed(evilId);
            var components = new ComponentBuilder()
                .WithButton("Accept", ConsentInteraction.BuildAcceptId(botId, message.Author.Id), ButtonStyle.Success)
                .Build();
            try
            {
                var dm = await message.Author.CreateDMChannelAsync();
                await dm.SendMessageAsync(embed: embed, components: components);
            }
            catch
            {
                await message.Channel.SendMessageAsync(embed: embed, components: components);
            }
        }

        public async Task<ChatMessage> WithPreviewsAsync(ChatMessage chat)
        {
            if (chat.Id != message.Id || !chat.WaitsForPreviews) return chat;
            try
            {
                await Task.Delay(settings.PreviewWait);
                var fresh = await message.Channel.GetMessageAsync(message.Id, CacheMode.AllowDownload);
                return fresh is null ? chat : DiscordChat.Read(message, botId, DiscordChat.EmbedsOf(fresh));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiryChannel] could not read the link previews: {ex.Message}");
                return chat;
            }
        }

        public static IEnumerable<string> Split(string text, int max = 2000)
        {
            if (text.Length <= max)
            {
                yield return text;
                yield break;
            }
            var buffer = new StringBuilder();
            foreach (var rawLine in text.Split('\n'))
            {
                var line = rawLine;
                while (line.Length > max)
                {
                    if (buffer.Length > 0)
                    {
                        yield return buffer.ToString();
                        buffer.Clear();
                    }
                    var cut = line.LastIndexOf(' ', Math.Min(max - 1, line.Length - 1));
                    if (cut <= 0) cut = max;
                    yield return line[..cut];
                    line = line[cut..].TrimStart();
                }
                if (buffer.Length > 0 && buffer.Length + line.Length + 1 > max)
                {
                    yield return buffer.ToString();
                    buffer.Clear();
                }
                if (buffer.Length > 0) buffer.Append('\n');
                buffer.Append(line);
            }
            if (buffer.Length > 0) yield return buffer.ToString();
        }
    }
}
