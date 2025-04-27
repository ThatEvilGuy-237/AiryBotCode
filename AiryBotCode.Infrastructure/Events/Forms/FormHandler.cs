using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using AiryBotCode.Events.ButtonPress;
using AiryBotCode.Events.SlashCommands.Commands;

namespace AiryBotCode.Events.Forms
{
    public class FormHandler : MyEventHandeler
    {
        public FormHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task HandleFormInteraction(SocketModal modal)
        {
            var buttonValue = modal.Data.CustomId;
            CustomIdEncription button = new CustomIdEncription();
            button.Decrypt(buttonValue);

            switch (button.Command)
            {
                case UserLogsCommand.Name:
                    await UserLogsCommand.HandleForm(modal, _client, button);
                    break;
                default:
                    break;
            }
        }

    }
}
