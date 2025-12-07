using AiryBotCode.Application.Features.Conversational;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class SummarizeUserAction : EvilAction, ISlashAction
    {
        public SummarizeUserAction(IServiceProvider serviceProvider) :
            base(serviceProvider.GetRequiredService<SummarizeUserCommand>(), serviceProvider)
        {
        }

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            SummarizeUserCommand summarizeUserCommand = (SummarizeUserCommand)Command;
            await summarizeUserCommand.HandleSlashCommand(command);
        }
    }
}
