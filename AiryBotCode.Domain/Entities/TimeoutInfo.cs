using Discord.WebSocket;

namespace AiryBotCode.Domain.Entities
{
    public class UntimeOutInfo
    {
        public SocketGuildUser Target { get; set; }

        public UntimeOutInfo()
        {

        }
        public UntimeOutInfo(SocketGuildUser target)
        {
            Target = target;
        }
    }
}
