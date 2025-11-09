using AiryBotCode.Domain.database;
using System.Collections.Generic;

namespace AiryBotCode.Application.DTOs
{
    public class ConversationContext
    {
        public ChannelConversation ChannelConversation { get; set; }
        public ChatUser AuthorUser { get; set; }
        public ChatUser SystemUser { get; set; }
        public ChatUser AiUser { get; set; }
        public List<Message> MessageHistory { get; set; } = new List<Message>();
    }
}
