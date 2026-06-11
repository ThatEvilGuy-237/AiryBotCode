using System.Text;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
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
                var reply = await forwarder.TryForwardAsync(
                    botId, message.Channel.Id, message.Author.Id, message.Author.Username, message.Content,
                    () => message.Channel.EnterTypingState());
                if (!string.IsNullOrWhiteSpace(reply))
                    // Discord caps a message at 2000 chars — split long replies so
                    // they aren't rejected with "Message content is too long".
                    foreach (var chunk in SplitForDiscord(reply))
                        await message.Channel.SendMessageAsync(chunk);
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
