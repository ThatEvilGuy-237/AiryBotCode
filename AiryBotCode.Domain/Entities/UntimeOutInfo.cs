using Discord.WebSocket;

namespace AiryBotCode.Domain.Entities
{
    public class UntimeoutInfo
    {
        public SocketGuildUser Target { get; set; }

        public UntimeoutInfo()
        {

        }
        public UntimeoutInfo(SocketGuildUser target)
        {
            Target = target;
        }
    }
}
