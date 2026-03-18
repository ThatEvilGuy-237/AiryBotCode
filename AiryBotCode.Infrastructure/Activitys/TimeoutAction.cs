using AiryBotCode.Application.Features.Moderation;
using AiryBotCode.Application.Features.Logging;
using AiryBotCode.Domain.Entities;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class TimeoutAction : EvilAction, ISlashAction
    {
        protected UserlogsCommand userlogsCommand;
        protected IConfigurationReader _config;
        public TimeoutAction(IServiceProvider serviceProvider, IConfigurationReader configuration) : 
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
