using AiryBotCode.Application.Comands.SlashCommands;
using AiryBotCode.Domain.Entities;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces;


namespace AiryBotCode.Infrastructure.Activitys
{
    public class ReminderAction : EvilAction, ISlashAction
    {
        protected IConfigurationReader _config;
        public ReminderAction(IServiceProvider serviceProvider, IConfigurationReader configuration) : 
            base(serviceProvider.GetRequiredService<ReminderCommand>(), serviceProvider)
        {
            _config = configuration;
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            ReminderCommand reminderCommand = (ReminderCommand)Command;
            Reminder info = await reminderCommand.CreateReminder(command);

        }
    }
}
