using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Domain.Entities;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class UntimeOutAction : EvilAction, ISlashAction
    {
        protected IConfigurationReader _config;
        public UntimeOutAction(IServiceProvider serviceProvider, IConfigurationReader configuration) : 
            base(serviceProvider.GetRequiredService<UntimeoutCommand>(), serviceProvider)
        {
            _config = configuration;
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            UntimeoutCommand timeoutCommand = (UntimeoutCommand)Command;
            UntimeoutInfo info = await timeoutCommand.UntimeoutUser(command);
        }
    }
}
