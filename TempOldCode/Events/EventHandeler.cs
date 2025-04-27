
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Events
{
    public class MyEventHandeler
    {
        protected DiscordSocketClient _client;
        protected readonly IServiceProvider _serviceProvider;
        public MyEventHandeler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        }
        public void AssingClient(DiscordSocketClient discordClient)
        {
            _client = discordClient;
        }
    }
}
