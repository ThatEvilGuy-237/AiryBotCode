using AiryBotCode.Application.Comands;
using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Domain.Entities;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Interfaces;
using AiryBotCode.Tool.Frontend;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace AiryBotCode.Infrastructure.Activitys.SlashEvents
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
