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
                    await message.Channel.SendMessageAsync(reply);
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

    }
}
