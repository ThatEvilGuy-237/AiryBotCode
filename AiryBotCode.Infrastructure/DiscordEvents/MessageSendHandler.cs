using System.Text;
using AiryBotCode.Application.Consent;
using AiryBotCode.Application.Frontend;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class MessageSendHandler : EvilEventHandler
    {
        private List<EvilAction> _messageAction;
        public void AssignActions(List<EvilAction> events)
        {
            _messageAction = events.OfType<IMessageAction>().Cast<EvilAction>().ToList();
            Console.WriteLine("MessageSendHandler");
        }
        public MessageSendHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // TODO: this should be reworked to handel diffrent type of messages check that we want.
        public async Task HandelMessageSend(SocketMessage message)
        {
            // Skip messages from bots (including this bot itself)
            if (message.Author.IsBot)
                return;

            // Channel → webhook chat: if this channel is linked, forward the message
            // to its webhook and relay any reply back.
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var botId = scope.ServiceProvider.GetRequiredService<IConfigurationReader>().GetBotId();
                var forwarder = scope.ServiceProvider.GetRequiredService<WebhookChatService>();

                // Resolve raw mentions (<@id> / bare @id) to "DisplayName (id)" so the
                // flow + memory see a name, not a snowflake. Prefer the users Discord
                // already resolved on the message; fall back to a guild lookup.
                var mentioned = message.MentionedUsers
                    .GroupBy(u => u.Id)
                    .ToDictionary(g => g.Key, g => (g.First() as SocketGuildUser)?.Nickname ?? g.First().Username);
                var guild = (message.Channel as SocketGuildChannel)?.Guild;
                var content = MentionResolver.Rewrite(message.Content, id =>
                    mentioned.TryGetValue(id, out var n) ? n
                        : (guild?.GetUser(id) is { } gu ? (gu.Nickname ?? gu.Username) : null));

                // First-message consent gate: only for channels linked to a Hive chat
                // (where messages are forwarded + stored). Until the user accepts the
                // data-use notice, post the notice + Accept button and don't forward.
                // Fail-open: any error in the check lets the message through so a
                // missing table / DB hiccup never bricks the bot.
                var link = await scope.ServiceProvider.GetRequiredService<IChannelWebhookRepository>()
                    .GetForChannelAsync(botId, message.Channel.Id);
                if (link != null)
                {
                    var consented = true;
                    try { consented = await scope.ServiceProvider.GetRequiredService<IUserConsentRepository>()
                            .HasConsentAsync(botId, message.Author.Id); }
                    catch (Exception cx) { Console.WriteLine($"[Consent] check failed, allowing through: {cx.Message}"); }
                    if (!consented)
                    {
                        var evilId = scope.ServiceProvider.GetRequiredService<IConfigurationReader>().GetEvilId();
                        var embed = ConsentFrontend.ConsentEmbed(evilId);
                        var components = new ComponentBuilder()
                            .WithButton("Accept", ConsentInteraction.BuildAcceptId(botId, message.Author.Id), ButtonStyle.Success)
                            .Build();
                        // DM the consent card so ONLY this user sees it (a channel message can't be
                        // ephemeral). Fall back to the channel if their DMs are closed.
                        try
                        {
                            var dm = await message.Author.CreateDMChannelAsync();
                            await dm.SendMessageAsync(embed: embed, components: components);
                        }
                        catch
                        {
                            await message.Channel.SendMessageAsync(embed: embed, components: components);
                        }
                        return;   // gated until they accept
                    }
                }

                // Forward any image attachments so the agent's vision intake can read them.
                var images = ImageAttachments.Pick(
                    message.Attachments.Select(a => ((string?)a.Url, (string?)a.Filename, (string?)a.ContentType)));

                var reply = await forwarder.TryForwardAsync(
                    botId, message.Channel.Id, message.Author.Id, message.Author.Username, content,
                    images, () => message.Channel.EnterTypingState());
                if (!string.IsNullOrWhiteSpace(reply))
                {
                    // Discord caps a message at 2000 chars — split long replies so
                    // they aren't rejected with "Message content is too long".
                    var dedup = scope.ServiceProvider.GetRequiredService<RecentMessageDedup>();
                    foreach (var chunk in SplitForDiscord(reply))
                    {
                        // Skip a chunk the agent already delivered live via the `say`
                        // effect this run — avoids the say-then-finish-same-text double post.
                        if (!dedup.TryReserve(message.Channel.Id, chunk))
                        {
                            Console.WriteLine($"[Webhook] reply chunk to {message.Channel.Id} suppressed — already said via effect.");
                            continue;
                        }
                        await message.Channel.SendMessageAsync(chunk);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Webhook] message forward error: {ex.Message}");
            }

            foreach (var Event in _messageAction)
            {
                if (Event is IMessageAction messageEvent)
                {
                    await messageEvent.HandleMessageReceivedAsync(message);
                }
            }
        }

        // Split text into <=max-char chunks for Discord, breaking on line then
        // space boundaries; only hard-cutting when a single run has no break.
        private static IEnumerable<string> SplitForDiscord(string text, int max = 2000)
        {
            if (text.Length <= max) { yield return text; yield break; }

            var sb = new StringBuilder();
            foreach (var rawLine in text.Split('\n'))
            {
                var line = rawLine;
                // A single line longer than the cap → break it (prefer a space).
                while (line.Length > max)
                {
                    if (sb.Length > 0) { yield return sb.ToString(); sb.Clear(); }
                    var cut = line.LastIndexOf(' ', Math.Min(max - 1, line.Length - 1));
                    if (cut <= 0) cut = max;
                    yield return line[..cut];
                    line = line[cut..].TrimStart();
                }
                // Flush the buffer if appending this line would overflow.
                if (sb.Length > 0 && sb.Length + line.Length + 1 > max)
                {
                    yield return sb.ToString(); sb.Clear();
                }
                if (sb.Length > 0) sb.Append('\n');
                sb.Append(line);
            }
            if (sb.Length > 0) yield return sb.ToString();
        }

    }
}
