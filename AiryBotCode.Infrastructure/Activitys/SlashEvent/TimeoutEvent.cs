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
    public class TimeoutEvent : EvilEvent, ISlashEvent, IClientAccess
    {
        protected DiscordSocketClient _client;
        protected UserlogsCommand userlogsCommand;
        protected IConfigurationReader _config;
        public TimeoutEvent(IServiceProvider serviceProvider, IConfigurationReader configuration) : 
            base(serviceProvider.GetRequiredService<TimeoutCommand>(), serviceProvider)
        {
            userlogsCommand = serviceProvider.GetRequiredService<UserlogsCommand>();
            _config = configuration;
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            if (_client == null) throw new Exception("No cient assinged");

            TimeoutCommand timeoutCommand = (TimeoutCommand)Command;
            TimeoutInfo info = await timeoutCommand.TimeoutUser(command, _client);
            await userlogsCommand.SendUserLog(command, _client, 
                new LogInfo(LogType.Timeout, info.Target, info.Reason, $"Timed out for {info.DurationOption} minutes."));
        }

        public void SetClient(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}
