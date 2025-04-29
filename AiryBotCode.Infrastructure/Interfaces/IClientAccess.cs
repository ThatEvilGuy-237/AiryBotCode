using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Infrastructure.Interfaces
{
    public interface IClientAccess
    {
        public void SetClient(DiscordSocketClient client);
    } 
}
