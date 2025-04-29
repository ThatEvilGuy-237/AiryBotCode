
using Discord.WebSocket;

namespace AiryBotCode.Events
{
    public class MyEventHandeler
    {
        protected DiscordSocketClient _client;
        protected readonly IServiceProvider _serviceProvider;
        public MyEventHandeler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void AssingClient(DiscordSocketClient discordClient)
        {
            _client = discordClient;
        }
    }
}
