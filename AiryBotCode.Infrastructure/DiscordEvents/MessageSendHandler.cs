using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Conversation;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class MessageSendHandler : EvilEventHandler
    {
        private List<EvilAction> _messageAction = [];

        public MessageSendHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void AssignActions(List<EvilAction> events)
        {
            _messageAction = events.OfType<IMessageAction>().Cast<EvilAction>().ToList();
            Console.WriteLine("MessageSendHandler");
        }

        public async Task HandelMessageSend(SocketMessage message)
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var config = services.GetRequiredService<IConfigurationReader>();
            var botId = config.GetBotId();
            var conversation = services.GetRequiredService<AiryConversation>();

            if (message.Author.IsBot)
            {
                if (message.Author.Id == botId) NoteOwnMessage(conversation, message, botId);
                return;
            }

            foreach (var action in _messageAction.OfType<IMessageAction>())
                await action.HandleMessageReceivedAsync(message);

            try
            {
                var chat = DiscordChat.Read(message, botId);
                var channel = new DiscordChatChannel(message, botId, config.GetEvilId(),
                    services.GetRequiredService<RecentMessageDedup>(), services.GetRequiredService<ConversationSettings>());
                if (!await conversation.OnMessageAsync(chat, channel))
                    await ForwardToWebhookAsync(services, message, chat, channel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiryChannel] message {message.Id} in {message.Channel.Id} failed: {ex.Message}");
            }
        }

        private static void NoteOwnMessage(AiryConversation conversation, SocketMessage message, ulong botId)
        {
            try
            {
                conversation.OnOwnMessage(DiscordChat.Read(message, botId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiryChannel] could not note Airy's own message: {ex.Message}");
            }
        }

        private static async Task ForwardToWebhookAsync(IServiceProvider services, SocketMessage message, ChatMessage chat, IChatChannel channel)
        {
            try
            {
                var link = await services.GetRequiredService<IChannelWebhookRepository>().GetForChannelAsync(chat.BotId, chat.ChannelId);
                if (link is null) return;
                if (await RequiresMentionAsync(services, chat.BotId) && !chat.Pinged) return;
                if (!await services.GetRequiredService<IConversationStore>().IsMemberAsync(chat.BotId, chat.AuthorId))
                {
                    await channel.AskConsentAsync();
                    return;
                }
                var reply = await services.GetRequiredService<WebhookChatService>().TryForwardAsync(
                    chat.BotId, chat.ChannelId, chat.AuthorId, chat.AuthorAccount, chat.Text, chat.Attachments, () => message.Channel.EnterTypingState());
                if (!string.IsNullOrWhiteSpace(reply)) await channel.SendAsync(reply);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Webhook] message forward error: {ex.Message}");
            }
        }

        private static async Task<bool> RequiresMentionAsync(IServiceProvider services, ulong botId)
        {
            try
            {
                var settings = await services.GetRequiredService<ICommandSettingsRepository>().GetByCommandAsync(botId, "HiveChatCommand");
                var row = settings.FirstOrDefault(s => s.Key == "RequireMention");
                return row is not null && bool.TryParse(row.Value, out var required) && required;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HiveChat] setting read failed, allowing through: {ex.Message}");
                return false;
            }
        }
    }
}
