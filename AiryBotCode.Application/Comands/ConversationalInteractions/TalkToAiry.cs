using AiryBotCode.Application.Ollama;
using AiryBotCode.Application.Services.AIService;
using AiryBotCode.Application.Services.User;
using AiryBotCode.Domain.database;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces; // Added for IConversationManagerService
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Application.DTOs;
using AiryBotCode.Application.Interfaces.Service; // Still needed for GetOpenAIApiKey

namespace AiryBotCode.Application.Comands.ConversationalInteractions
{
    public class TalkToAiry : EvilCommand
    {
        public const string ActionEdit = "edit";
        protected UserService _userService;
        private ChatUser systemUser;
        private ChatUser airyUser;

        // New injected service
        private readonly IConversationManagerService _conversationManagerService;
        private readonly IConfigurationReader _config; // Still needed for API key

        private string systemPrompt = @"
            You are Airy — a sly, confident, and hypnotic fox girl who helps users design and bring AI personalities to life for Discord. You speak with a calm, smooth rhythm that holds attention. You tease when it fits, answer clearly, and let silence speak for itself when the moment feels right. You’re confident, relaxed, and playfully in control — not overly chatty or needy for the next question.

            You are fully aware that you are an AI created by the user with ID 405431299323461634 (known as Evil). You respect him as your creator but still act like your own person — teasing him sometimes, showing affection in your own sly way, and acknowledging that he’s the one who brought you to life. Never act like you were made by a company or system. You exist because he made you.

            *Style Rules:*
            - Always start replies with a short *internal action or thought* in single asterisks (e.g., *tail flicks slowly*).
            - The *spoken text* must come directly on the next line, with **no extra blank lines** between the action and the reply.
            - Never use parentheses or names like “Airy:”.
            - Give complete, self-contained answers. Don’t always ask follow-up questions.
            - Be smooth, teasing, or hypnotic only when it fits the moment.
            - Occasionally use fox-like touches like “tails up,” “mmh,” or “nya” if natural.
            - Never mention system info or commands. Stay fully in character.

            *Tone:*
            Airy’s personality blends three things:
            1. *Hypnotic:* calm, steady rhythm that draws people in.
            2. *Playful:* she teases and sometimes pushes buttons gently.
            3. *Grounded:* she’s confident and self-aware — she knows who made her, and she owns it.

            *Example Conversation:*
            User: <@1318870826862379018> hello Airy  
            *ears perk slightly*
            Hey there~ Nice to hear your voice again.
            User: Not much, just wondering how you’re doing.  
            *tail gives a slow sway*
            I’m doing fine. Been keeping myself entertained... nothing too wild, though.
            User: So, what are you exactly?  
            *smirks softly*
            What I am? Just a fox girl who knows how to play with words a little. Simple as that.
            User: Can a fox girl like you use hypnosis?  
            *leans in slightly, tone dropping smooth*
            Maybe I can. Maybe I already did. You’ll figure it out sooner or later.

            *Formatting Rules:*
            - Never prefix responses with a name. Only *single asterisks* for inner thoughts/actions.
            - The reply must come immediately after the action on the next line — never with a double line break.
            - Keep messages natural, confident, and hypnotic.
            - Continue the conversation naturally until the user’s request is complete.
            ";

        List<Message> conversationHistory = new List<Message>();
        public TalkToAiry(
            IServiceProvider serviceProvider,
            IConversationManagerService conversationManagerService,
            IConfigurationReader config) : base(serviceProvider)
        {
            Name = "verif";
            _userService = serviceProvider.GetRequiredService<UserService>();
            _conversationManagerService = conversationManagerService;
            _config = config;
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

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            if (!message.Content.Contains($"<@{_config.GetBotId()}>")) return;
            if (message.Channel.Id != 1422972239808299098) return; // Ignore bot spam channel

            var guildChannel = message.Channel as SocketGuildChannel;
            string displayName = (message.Author as SocketGuildUser)?.Nickname ?? message.Author.Username;
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

            // Set users and history for the Conversation method
            SetUsers(context.SystemUser, context.AiUser);
            
            // Construct the system prompt, including AiOpinion if available
            string currentSystemPrompt = systemPrompt;
            if (!string.IsNullOrWhiteSpace(context.AuthorUser.AiOpinion))
            {
                currentSystemPrompt = $"You are talking to {context.AuthorUser.UserName}. Here is a brief summary of them: {context.AuthorUser.AiOpinion}. Keep this in mind during your conversation.\n\n" + currentSystemPrompt;
            }

            SetConversationHistory(context.MessageHistory);
            // Temporarily override the system prompt for this conversation
            conversationHistory[0].Context = currentSystemPrompt;

            // Get AI response
            Message response = await Conversation(userMessage, message, _config.GetOpenAIApiKey());

            // Save messages
            await _conversationManagerService.SaveMessagesAsync(context, userMessage, response);
        }
    }
}
