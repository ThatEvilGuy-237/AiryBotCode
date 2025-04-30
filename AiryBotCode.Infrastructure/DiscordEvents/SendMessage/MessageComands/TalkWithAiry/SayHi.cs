using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.DiscordEvents.SendMessage.MessageComands.TalkWithAiry
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
