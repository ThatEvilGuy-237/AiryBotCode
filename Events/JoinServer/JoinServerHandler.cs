using Discord.WebSocket;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Events.JoinServer
{
    public class JoinServerHandler : MyEventHandeler
    {
        public JoinServerHandler(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
        }
       
    }
}
