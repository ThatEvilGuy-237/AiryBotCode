using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Events.Comands
{
    public class TalkWithAiry
    {
        public async Task HandelTalkWithAiry(SocketMessage arg)
        {
            await arg.Channel.SendMessageAsync("hello");
        }
    }
}
