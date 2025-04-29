using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Application.Services;
using AiryBotCode.Application.Services.Loging;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Interfaces;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys.SlashEvents
{
    public class UserlogsEvent : EvilEvent, ISlashEvent, IButtonEvent, IFormEvent, IClientAccess
    {
        protected DiscordSocketClient _client;
        protected IConfigurationReader _config;

        public UserlogsEvent(IServiceProvider serviceProvider, IConfigurationReader configuration) : 
            base(serviceProvider.GetRequiredService<UserlogsCommand>(), serviceProvider)
        {
            _config = configuration;
        }
        public void SetClient(DiscordSocketClient client)
        {
            _client = client;
        }
        private void ClientNullCheck()
        {
            if (_client != null) return;
            Console.WriteLine("No cient assinged");
                 throw new Exception("No cient assinged");
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            ClientNullCheck();

            UserlogsCommand userlogs = (UserlogsCommand)Command;
            bool worked = await userlogs.HandleSlashCommand(command, _client);
            //if (!worked) return;
        }

        public async Task HandleButtonPressAsync(SocketMessageComponent component, ButtonEncriptionService buttonEncription)
        {
            ClientNullCheck();

            UserlogsCommand userlogs = (UserlogsCommand)Command;
            await userlogs.HandelEditButton(component, buttonEncription);
        }

        public async Task HanndelFormAsync(SocketModal modal, ButtonEncriptionService buttonEncription)
        {
            ClientNullCheck();

            UserlogsCommand userlogs = (UserlogsCommand)Command;
            await userlogs.HandleForm(modal,_client, buttonEncription);
        }
    }
}
