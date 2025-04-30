using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class MyEventHandeler
    {
        protected DiscordSocketClient _client;
        protected readonly IServiceProvider _serviceProvider;
        public MyEventHandeler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        }
    }
}
