using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class VerifyUserAgeAction : EvilAction, ISlashAction
    {
        protected IConfigurationReader _config;
        public VerifyUserAgeAction(IServiceProvider serviceProvider, IConfigurationReader configuration) :
            base(serviceProvider.GetRequiredService<VerifyUserAgeCommand>(), serviceProvider)
        {
            _config = configuration;
        }

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            VerifyUserAgeCommand verifyCommand = (VerifyUserAgeCommand)Command;
            await verifyCommand.AgeVerifyUser(command);
        }
    }
}
