using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Events.SendMessage.MessageComands.TalkWithAiry
{

    public class SayHi
    {
        public static TalkWithAiryComandList CommandType { get; }
        = TalkWithAiryComandList.SayHi;

        public static string Key = "hi";

        public static async Task Execute(SocketMessage arg)
        {
            await arg.Channel.SendMessageAsync("Hello");
        }
    }
}
