using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class FormHandler : MyEventHandeler
    {
        private readonly List<EvilEvent> _formEvents;

        public FormHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _formEvents = new List<EvilEvent>
            {
                serviceProvider.GetRequiredService<UserlogsEvent>(),
            };
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
                    if (Event is IFormEvent formEvent)
                    {
                        await formEvent.HanndelFormAsync(modal, button);
                        return;
                    }

                    return;
                }
            }

            await modal.RespondAsync("Unknown form submission.");
        }
    }
}
