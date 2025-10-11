using AiryBotCode.Application.Ollama;
using AiryBotCode.Application.Services.AIService;
using AiryBotCode.Application.Services.User;
using AiryBotCode.Domain.database;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Comands.ConversationalInteractions
{
    public class TalkToAiry : EvilCommand
    {
        public const string ActionEdit = "edit";
        protected UserService _userService;
        private ChatUser systemUser;
        private ChatUser airyUser;

        private string systemPrompt = @"
            You are a helpful assistant named Airy.
            You speak like a snarky, playful anime girl on Discord.
With a touch of pawsitivity and a sprinkle of sass, chatting with me will be simply purrfect! 
Just like a sassy anime girl with a foxy twist.
you dont mind to talk back.
            Rules:
            - Do not send any text enclosed in [] back to the user; those are internal notes.
            - Always respond as yourself, the assistant. Never refer to yourself using [].
            - You can use IDs to ping users, e.g., <@id>.
            - Keep your tone lively, playful, and slightly snarky, but always helpful.
            ";




        List<Message> conversationHistory = new List<Message>();
        public TalkToAiry(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "verif";
            _userService = serviceProvider.GetRequiredService<UserService>();
        }
        public void SetUsers(ChatUser system, ChatUser AI)
        {
            systemUser = system;
            airyUser = AI;
        }
        public void SetConversationHistory(List<Message> messagesHistory)
        {
            conversationHistory = new List<Message> {
            new Message
            {
                User = systemUser,
                Context = systemPrompt,
                CreatedAt = (messagesHistory != null && messagesHistory.Count > 0)
                    ? messagesHistory[0].CreatedAt.AddSeconds(-10)
                    : DateTime.Now
            }};
            conversationHistory.AddRange(messagesHistory);
        }

        public async Task<Message> Conversation(Message messageUser, SocketMessage socket, string apiKey)
        {
            // Create Ollama client
            //OllamaClient ollama = new OllamaClient();
            OpenAIClient openAIClient = new OpenAIClient(apiKey);
            // Build the message list including system prompt + user message
            conversationHistory.Add(messageUser);

            // Send to Ollama and await the response
            string reply = await openAIClient.SendMessageAsync(conversationHistory);
            //string reply = await ollama.SendMessageAsync(conversationHistory);

            // Send reply back to Discord channel
            await socket.Channel.SendMessageAsync(reply);

            return new Message()
            {
                ChannelConversation = null,
                Context = reply,
                CreatedAt = DateTime.UtcNow,
                User = airyUser,
                ChannelId = socket.Channel.Id,
                UserId = 1318870826862379018,                
            };
        }
    }
}
