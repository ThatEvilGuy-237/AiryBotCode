using AiryBotCode.Application.Comands;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys.SlashEvents
{
    public class TimeoutEvent : EvilEvent, ISlashEvent
    {
        public TimeoutEvent(IServiceProvider serviceProvider) : 
            base(serviceProvider.GetRequiredService<TimeoutCommand>(), serviceProvider)
        {
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            TimeoutCommand timeoutCommand = (TimeoutCommand)Command;
            await timeoutCommand.TimeoutUser(command, _client);
        }
    }
}
