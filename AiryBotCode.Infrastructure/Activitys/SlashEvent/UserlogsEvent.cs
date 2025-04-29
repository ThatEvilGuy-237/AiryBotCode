using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Application.Services;
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
            Embed? embed = await userlogs.CreateLog(command);
            if (embed == null) return;
            ulong channelId = _config.GetLogChannelId();
            var socketTextChannel = (SocketTextChannel)await _client.GetChannelAsync(channelId);
            await userlogs.SendUserLog(command, embed, socketTextChannel);
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
