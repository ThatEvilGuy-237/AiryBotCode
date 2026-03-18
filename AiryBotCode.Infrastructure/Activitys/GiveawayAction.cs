using AiryBotCode.Application.Features.Giveaway;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using AiryBotCode.Application.Services;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class GiveawayAction : EvilAction, ISlashAction, IButtonAction, IFormAction
    {
        public GiveawayAction(IServiceProvider serviceProvider) 
            : base(serviceProvider.GetRequiredService<GiveawayCommand>(), serviceProvider)
        {
        }

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var giveawayCommand = (GiveawayCommand)Command;
            await giveawayCommand.HandleStartEventCommand(command);
        }

        public async Task HandleButtonPressAsync(SocketMessageComponent component, ButtonEncriptionService buttonEncription)
        {
            var giveawayCommand = (GiveawayCommand)Command;
            await giveawayCommand.HandleButtonInteraction(component, buttonEncription);
        }

        public async Task HandleFormAsync(SocketModal modal, ButtonEncriptionService buttonEncription)
        {
            var giveawayCommand = (GiveawayCommand)Command;
            await giveawayCommand.HandleGetRandomForm(modal);
        }

    }
}