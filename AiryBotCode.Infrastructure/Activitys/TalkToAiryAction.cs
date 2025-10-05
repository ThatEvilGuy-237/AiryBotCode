using AiryBotCode.Application.Comands;
using AiryBotCode.Application.Comands.ConversationalInteractions;
using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class TalkToAiryAction : EvilAction, IMessageAction
    {
        protected IConfigurationReader _config;
        public TalkToAiryAction(IServiceProvider serviceProvider, IConfigurationReader configuration) :
            base(serviceProvider.GetRequiredService<TalkToAiry>(), serviceProvider)
        {
        }

        public async Task HandleMessageReceivedAsync(SocketMessage message)
        {
            TalkToAiry talkToAiry = (TalkToAiry)Command;
            await talkToAiry.Conversation(message);
        }
    }
}
