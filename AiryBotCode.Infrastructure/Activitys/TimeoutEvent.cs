using AiryBotCode.Application.Comands;
using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Domain.Entities;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class TimeoutEvent : EvilEvent, ISlashEvent
    {
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

            TimeoutCommand timeoutCommand = (TimeoutCommand)Command;
            TimeoutInfo info = await timeoutCommand.TimeoutUser(command);
            if(info.Target != null || info.Duration != 0)
            {

            }

            var message = $"Timed out for {info.Duration} minutes." +
              (info.MessagesCleared > 0 ? $" Messages cleared: {info.MessagesCleared}" : "");
            var log = new LogInfo(LogType.Timeout, info.Target, info.Reason, message);
            await userlogsCommand.SendUserLog(command, log);
        }
    }
}
