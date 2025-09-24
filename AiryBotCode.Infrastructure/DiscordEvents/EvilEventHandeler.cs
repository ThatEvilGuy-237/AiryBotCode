using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class EvilEventHandler
    {
        protected DiscordSocketClient _client;
        protected readonly IServiceProvider _serviceProvider;
        public EvilEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        }
    }
}
