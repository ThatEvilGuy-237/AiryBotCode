using AiryBotCode.Application.Comands;
using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys.SlashEvents
{
    public class UserlogsEvent : EvilEvent, ISlashEvent, IButtonEvent, IFormEvent, IClientAccess
    {
        protected DiscordSocketClient _client;
        public UserlogsEvent(IServiceProvider serviceProvider) : 
            base(serviceProvider.GetRequiredService<UserlogsCommand>(), serviceProvider)
        {
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
            await userlogs.SlashCommandLog(command);
        }

        public async Task HandleButtonPressAsync(SocketMessageComponent component, ButtonEncriptionService buttonEncription)
        {
            ClientNullCheck();

            UserlogsCommand userlogs = (UserlogsCommand)Command;
            await userlogs.ShowEditForm(component, buttonEncription);
        }

        public async Task HanndelFormAsync(SocketModal modal, ButtonEncriptionService buttonEncription)
        {
            ClientNullCheck();

            UserlogsCommand userlogs = (UserlogsCommand)Command;
            await userlogs.HandleForm(modal,_client, buttonEncription);
        }
    }
}
