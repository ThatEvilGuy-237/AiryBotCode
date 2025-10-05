using AiryBotCode.Application.Services.User;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.AIModels.Ollama;

namespace AiryBotCode.Application.Comands.ConversationalInteractions
{
    public class TalkToAiry : EvilCommand
    {
        public const string ActionEdit = "edit";
        protected UserService _userService;

        private string systemPrompt = "You are Airy, a friendly and helpful Discord bot. You assist users with their questions and provide information about the server. Always be polite and respectful.";

        public TalkToAiry(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "verif";
            _userService = serviceProvider.GetRequiredService<UserService>();
        }

        public void GetHistory()
        {
            // Implementation for getting history: TODO! IGNORE FOR NOW!
        }

        public async Task Conversation(SocketMessage message)
        {
            // Create Ollama client
            OllamaClient ollama = new OllamaClient();

            // Build the message list including system prompt + user message
            var messages = new List<Message>
            {
                new Message
                {
                    Role = ChatRole.System,
                    Content = systemPrompt
                },
                new Message
                {
                    Role = ChatRole.User,
                    UserName = message.Author.Username,
                    Content = message.Content
                }
            };

            // Send to Ollama and await the response
            string reply = await ollama.SendMessageAsync(messages);

            // Send reply back to Discord channel
            await message.Channel.SendMessageAsync(reply);
        }
    }
}
