using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces;

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
