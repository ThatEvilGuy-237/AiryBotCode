using AiryBotCode.Application.DTOs;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Services.AIService;
using AiryBotCode.Domain.database;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
namespace AiryBotCode.Application.Comands.ConversationalInteractions
{
    public class TalkToAiry : EvilCommand
    {
        private readonly IConversationManagerService _conversationManagerService;
        private readonly IConfigurationReader _config;
        private readonly OpenAIClient _openAIClient;

        public TalkToAiry(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _config = serviceProvider.GetRequiredService<IConfigurationReader>();
            _openAIClient = new OpenAIClient(_config.GetOpenAIApiKey());
            _conversationManagerService = serviceProvider.GetRequiredService<IConversationManagerService>();
        }

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            List<ulong> channelsId = new List<ulong> { 1182267222152982533, 1182267222152982535, 1182267222152982534, 1182267779135590490, 1236609199144697938 };
            if (!message.Content.Contains($"<@{_config.GetBotId()}>")) return;
            if (!channelsId.Contains(message.Channel.Id)) return; // Ignore bot spam channel

            var guildChannel = message.Channel as SocketGuildChannel;
            string displayName = (message.Author as SocketGuildUser)?.DisplayName ?? message.Author.Username;
            ulong botId = _config.GetBotId();

            ConversationContext context = await _conversationManagerService.GetOrCreateConversationContextAsync(
                message.Channel.Id,
                message.Author.Id,
                displayName,
                botId
            );

            // Create user message
            var userMessage = new Message
            {
                User = context.AuthorUser,
                UserId = context.AuthorUser.Id,
                ChannelId = message.Channel.Id,
                ChannelConversation = context.ChannelConversation,
                ChannelConversationId = context.ChannelConversation.Id,
                Context = message.Content,
                CreatedAt = message.Timestamp.UtcDateTime
            };

            // Create conversation history
            var conversationHistory = new List<Message>
            {
                new Message
                {
                    User = context.SystemUser,
                    Context = context.SystemPrompt,
                    CreatedAt = (context.MessageHistory != null && context.MessageHistory.Count > 0)
                        ? context.MessageHistory[0].CreatedAt.AddSeconds(-10)
                        : DateTime.Now
                }
            };
            conversationHistory.AddRange(context.MessageHistory);
            conversationHistory.Add(userMessage);


            // Get AI response
            string responseText = await _openAIClient.SendMessageAsync(conversationHistory);
            var responseMessage = new Message
            {
                User = context.AiUser,
                UserId = context.AiUser.Id,
                ChannelId = message.Channel.Id,
                ChannelConversation = context.ChannelConversation,
                ChannelConversationId = context.ChannelConversation.Id,
                Context = responseText,
                CreatedAt = DateTime.UtcNow
            };

            // Save messages
            await _conversationManagerService.SaveMessagesAsync(context, userMessage, responseMessage);

            // Send response to Discord
            await message.Channel.SendMessageAsync(responseText);
        }
    }
}
