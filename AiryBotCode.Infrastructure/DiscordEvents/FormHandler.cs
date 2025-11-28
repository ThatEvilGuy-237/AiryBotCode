using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class FormHandler : EvilEventHandler
    {
        private List<EvilAction> _formEvents;

        public void AssignActions(List<EvilAction> events)
        {
            _formEvents = events.OfType<IFormAction>().Cast<EvilAction>().ToList();
            Console.WriteLine("FormHandler");
        }

        public FormHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task HandleFormInteraction(SocketModal modal)
        {
            var formValue = modal.Data.CustomId;
            var button = new ButtonEncriptionService();
            button.Decrypt(formValue);

            foreach (var Event in _formEvents)
            {
                if (Event.Command.Name == button.CommandName)
                {
                    if (Event is IFormAction formEvent)
                    {
                        await formEvent.HandleFormAsync(modal, button);
                        return;
                    }

                    return;
                }
            }

            await modal.RespondAsync("Unknown form submission.");
        }
    }
}
