using AiryBotCode.Infrastructure.Interfaces;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Features.ContactUser;


namespace AiryBotCode.Infrastructure.Activitys
{
    public class ContactUserAction : EvilAction, ISlashAction
    {
        protected IConfigurationReader _config;
        public ContactUserAction(IServiceProvider serviceProvider, IConfigurationReader configuration) :
            base(serviceProvider.GetRequiredService<ContactUserCommand>(), serviceProvider)
        {
            _config = configuration;
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            ContactUserCommand contract = (ContactUserCommand)Command;

            RestTextChannel channel = await contract.ContractUserCommand(command);
            await contract.SendMessage(channel);
        }
    }
}
