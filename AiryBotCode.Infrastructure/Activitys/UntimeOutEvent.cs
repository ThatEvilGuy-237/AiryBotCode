using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Domain.Entities;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class UntimeOutEvent : EvilEvent, ISlashEvent
    {
        protected IConfigurationReader _config;
        public UntimeOutEvent(IServiceProvider serviceProvider, IConfigurationReader configuration) : 
            base(serviceProvider.GetRequiredService<UntimeoutCommand>(), serviceProvider)
        {
            _config = configuration;
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            UntimeoutCommand timeoutCommand = (UntimeoutCommand)Command;
            UntimeOutInfo info = await timeoutCommand.UntimeoutUser(command);
        }
    }
}
