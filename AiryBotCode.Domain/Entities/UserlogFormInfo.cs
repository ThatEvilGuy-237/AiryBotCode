using Discord;

namespace AiryBotCode.Domain.Entities
{
    public class UserlogFormInfo
    {
        public IMessageChannel Channel { get; set; }
        public IUserMessage Message { get; set; }
        public IEmbed Embed { get; set; }
        public UserlogFormInfo()
        {
        }
        public UserlogFormInfo(IMessageChannel channel, IUserMessage message, Embed embed)
        {
            this.Channel = channel;
            this.Message = message;
            this.Embed = embed;
        }
    }
}
