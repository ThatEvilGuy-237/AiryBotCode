﻿using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class MessageSendHandler : MyEventHandeler
    {
        private List<EvilAction> _messageAction;
        public void AssignActions(List<EvilAction> events)
        {
            _messageAction = events.OfType<IMessageAction>().Cast<EvilAction>().ToList();
        }
        public MessageSendHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // TODO: this should be reworked to handel diffrent type of messages check that we want.
        public async Task HandelMessageSend(SocketMessage message)
        {
            // somthing not realy needed now
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
