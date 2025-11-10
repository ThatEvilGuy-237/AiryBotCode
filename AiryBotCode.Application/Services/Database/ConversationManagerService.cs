using AiryBotCode.Application.DTOs;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Services.AIService;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Configuration;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace AiryBotCode.Application.Services.Database
{
    public class ConversationManagerService : IConversationManagerService
    {
        private readonly IChannelConversationService _channelConversationService;
        private readonly IChatUserService _chatUserService;
        private readonly IMessageService _messageService;
        private readonly IConfigurationReader _config;
        private readonly OpenAIClient _openAIClient;

        public ConversationManagerService(
            IChannelConversationService channelConversationService,
            IChatUserService chatUserService,
            IMessageService messageService,
            IConfigurationReader config,
            OpenAIClient openAIClient)
        {
            _channelConversationService = channelConversationService;
            _chatUserService = chatUserService;
            _messageService = messageService;
            _config = config;
            _openAIClient = openAIClient;
        }

        public async Task<ConversationContext> GetOrCreateConversationContextAsync(ulong channelId, ulong authorId, string authorDisplayName, ulong botId)
        {
            var conv = await _channelConversationService.GetOrCreateChannelConversationAsync(channelId);
            var authorUser = await _chatUserService.GetOrCreateChatUserAsync(authorId, authorDisplayName, ChatRole.User);
            var systemUser = await _chatUserService.GetOrCreateSystemUserAsync();
            var aiUser = await _chatUserService.GetOrCreateAiUserAsync(botId);
            var history = await _messageService.GetAndManageConversationHistoryAsync(conv.Id);

            // Get unique users from history
            var uniqueUsers = history.Select(m => m.User).DistinctBy(u => u.Id).ToList();
            var userOpinions = new Dictionary<string, string>();

            foreach (var user in uniqueUsers)
            {
                if (!string.IsNullOrEmpty(user.AiOpinion))
                {
                    userOpinions[user.UserName] = user.AiOpinion;
                }
            }

            // Construct the system prompt
            var systemPrompt = _config.GetOpenAIPrompt();

            // Add Owner info to the prompt
            var ownerId = _config.GetEvilId();
            var ownerUser = await _chatUserService.GetOrCreateChatUserAsync(ownerId, "BotOwner", ChatRole.User); // Role here is just a default, won't override if user exists
            systemPrompt += $"\nThe owner and creator of this bot is '{ownerUser.UserName}'.";

            if (!string.IsNullOrEmpty(conv.ConversationSummary))
            {
                systemPrompt += $"\n\nChannel Summary: {conv.ConversationSummary}";
            }

            if (userOpinions.Any())
            {
                systemPrompt += "\n\nUser Opinions:";
                foreach (var opinion in userOpinions)
                {
                    systemPrompt += $"\n- {opinion.Key}: {opinion.Value}";
                }
            }

            return new ConversationContext
            {
                ChannelConversation = conv,
                AuthorUser = authorUser,
                SystemUser = systemUser,
                AiUser = aiUser,
                MessageHistory = history,
                SystemPrompt = systemPrompt
            };
        }

        public async Task SaveMessagesAsync(ConversationContext context, Message userMessage, Message aiResponse)
        {
            userMessage.ChannelConversation = context.ChannelConversation;
            userMessage.ChannelConversationId = context.ChannelConversation.Id;
            aiResponse.ChannelConversation = context.ChannelConversation;
            aiResponse.ChannelConversationId = context.ChannelConversation.Id;

            await _messageService.SaveMessageAsync(userMessage);
            await _messageService.SaveMessageAsync(aiResponse);

            //// Update conversation (add messages to the in-memory collection for the current context)
            //context.ChannelConversation.Messages.Add(userMessage);
            //context.ChannelConversation.Messages.Add(aiResponse);
            //// No need to call _channelConversationRepo.UpdateAsync(context.ChannelConversation) here
            //// as messages are already saved and linked via foreign keys.
            //// If ConversationSummary needs updating, it would be done here.
        }

        public async Task<string> GenerateAndSaveUserSummaryAsync(ulong userId, ulong guildId)
        {
            var user = await _chatUserService.GetOrCreateChatUserAsync(userId, "Unknown User", ChatRole.User); // Get or create user
            if (user == null)
            {
                return "User not found in database.";
            }

            var userMessageContexts = await _messageService.GetUserMessageHistoryForSummaryAsync(userId);

            if (!userMessageContexts.Any())
            {
                return "No messages found for this user to summarize.";
            }

            var promptMessages = new List<Message>
            {
                new Message
                {
                    User = new ChatUser { Role = ChatRole.System, UserName = "System" },
                    Context = $"Summarize the following conversation snippets from user '{user.UserName}'. Focus on their personality, common topics, and general sentiment. The summary should be concise, neutral, and no more than two sentences."
                }
            };

            foreach (var msgContext in userMessageContexts)
            {
                promptMessages.Add(new Message
                {
                    User = user,
                    Context = msgContext
                });
            }

            string summary = await _openAIClient.SendMessageAsync(promptMessages);

            await _chatUserService.UpdateUserAiOpinionAsync(user, summary);

            return summary;
        }
    }
}
