using AiryBotCode.Application.Comands.ConversationalInteractions;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Database.Interfaces;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class TalkToAiryAction : EvilAction, IMessageAction
    {
        protected IConfigurationReader _config;
        private readonly IChatUserRepository _chatUserRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IChannelConversationRepository _channelConversationRepo;

        public TalkToAiryAction(
            IServiceProvider serviceProvider,
            IConfigurationReader configuration,
            IChatUserRepository chatUserRepo,
            IMessageRepository messageRepo,
            IChannelConversationRepository channelConversationRepo
        ) : base(serviceProvider.GetRequiredService<TalkToAiry>(), serviceProvider)
        {
            _chatUserRepo = chatUserRepo;
            _messageRepo = messageRepo;
            _channelConversationRepo = channelConversationRepo;
            _config = configuration;
        }

        public async Task HandleMessageReceivedAsync(SocketMessage message)
        {
            if (!message.Content.Contains($"<@{_config.GetBotId()}>")) return;

            // Get or create conversation
            var conv = await _channelConversationRepo.GetByChannelIdAsync(message.Channel.Id);
            if (conv == null)
            {
                conv = new ChannelConversation
                {
                    ChannelId = message.Channel.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ConversationSummary = ""
                };
                await _channelConversationRepo.AddAsync(conv);
            }

            // Get conversation history
            List<Message> history = (List<Message>)await _messageRepo.GetMessagesByConversationIdAsync(conv.Id);
            if (history.Count >= 12)
            {
                var messagesToDelete = history.OrderBy(m => m.CreatedAt).Take(2).ToList();
                foreach (var msg in messagesToDelete)
                {
                    await _messageRepo.DeleteAsync(msg); // assumes you have a DeleteAsync method
                    history.Remove(msg); // also remove from local list
                }
                await _messageRepo.SaveChangesAsync();
            }
            // Get or create chat user
            var chatUser = await _chatUserRepo.GetByIdAsync(message.Author.Id);
            if (chatUser == null)
            {
                chatUser = new ChatUser
                {
                    Id = message.Author.Id,
                    UserName = message.Author.Username,
                    Role = ChatRole.User,
                    CreatedAt = DateTime.UtcNow,
                    AiOpinion = "A friendly Discord user"
                };
                await _chatUserRepo.AddAsync(chatUser);
            }

            ChatUser system = await _chatUserRepo.GetByIdAsync((ulong)1);
            if(system == null)
            {
                system = new ChatUser()
                {
                    UserName = "System",
                    Id = (ulong)1,
                    Role = ChatRole.System,
                    AiOpinion = "System user",
                    CreatedAt = DateTime.UtcNow,
                    Messages = new List<Message>()
                };
                await _chatUserRepo.AddAsync(system);
            }
            ChatUser aiCharacter = await _chatUserRepo.GetByIdAsync(_config.GetBotId());
            if(aiCharacter == null)
            {
                aiCharacter = new ChatUser()
                {
                    UserName = "Asistent",
                    Id = _config.GetBotId(),
                    Role = ChatRole.Assistant,
                    AiOpinion = "A friendly and helpful Discord bot",
                    CreatedAt = DateTime.UtcNow,
                    Messages = new List<Message>()
                }; 
                await _chatUserRepo.AddAsync(aiCharacter);
            }
            // Create user message
            var userMessage = new Message
            {
                User = chatUser,
                UserId = chatUser.Id,
                ChannelId = message.Channel.Id,
                ChannelConversation = conv,
                ChannelConversationId = conv.Id,
                Context = message.Content,
                CreatedAt = message.Timestamp.UtcDateTime
            };

            // Get AI response
            var talkToAiry = (TalkToAiry)Command;
            talkToAiry.SetUsers(system, aiCharacter);   
            talkToAiry.SetConversationHistory(history);
            Message response = await talkToAiry.Conversation(userMessage, message, _config.GetOpenAIApiKey());

            // Save messages
            await _messageRepo.AddAsync(userMessage);
            response.ChannelConversation = conv;
            response.ChannelConversationId = conv.Id;
            await _messageRepo.AddAsync(response);

            // Update conversation
            conv.Messages.Add(userMessage);
            conv.Messages.Add(response);
            await _channelConversationRepo.UpdateAsync(conv);
            await _channelConversationRepo.SaveChangesAsync();
            await _messageRepo.SaveChangesAsync();
            await _chatUserRepo.SaveChangesAsync();
        }

    }
}
