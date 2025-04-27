using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Services
{
    public class ClientService
    {
        public readonly DiscordSocketClient _client;

        public ClientService(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        }
    }
}
