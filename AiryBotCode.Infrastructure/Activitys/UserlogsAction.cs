using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Application.Services;
using AiryBotCode.Application.Services.Loging;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Interfaces;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class UserlogsAction : EvilAction, ISlashAction, IButtonAction, IFormAction
    {
        protected IConfigurationReader _config;

        public UserlogsAction(IServiceProvider serviceProvider, IConfigurationReader configuration) : 
            base(serviceProvider.GetRequiredService<UserlogsCommand>(), serviceProvider)
        {
            _config = configuration;
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            UserlogsCommand userlogs = (UserlogsCommand)Command;
            bool worked = await userlogs.HandleSlashCommand(command);
            //if (!worked) return;
        }

        public async Task HandleButtonPressAsync(SocketMessageComponent component, ButtonEncriptionService buttonEncription)
        {
            UserlogsCommand userlogs = (UserlogsCommand)Command;
            await userlogs.HandelEditButton(component, buttonEncription);
        }

        public async Task HanndelFormAsync(SocketModal modal, ButtonEncriptionService buttonEncription)
        {
            UserlogsCommand userlogs = (UserlogsCommand)Command;
            await userlogs.HandleForm(modal, buttonEncription);
        }
    }
}
