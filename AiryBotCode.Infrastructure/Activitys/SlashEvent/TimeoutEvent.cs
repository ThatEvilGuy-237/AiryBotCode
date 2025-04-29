using AiryBotCode.Application.Comands;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys.SlashEvents
{
    public class TimeoutEvent : EvilEvent, ISlashEvent, IClientAccess
    {
        protected DiscordSocketClient _client;
        public TimeoutEvent(IServiceProvider serviceProvider) : 
            base(serviceProvider.GetRequiredService<TimeoutCommand>(), serviceProvider)
        {
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            if (_client == null) throw new Exception("No cient assinged");

            TimeoutCommand timeoutCommand = (TimeoutCommand)Command;
            await timeoutCommand.TimeoutUser(command, _client);
        }

        public void SetClient(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}
