using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class ButtonPressHandler : MyEventHandeler
    {
        private List<EvilAction> _buttonEvents;

        public void AssignActions(List<EvilAction> events)
        {
            _buttonEvents = events.OfType<IButtonAction>().Cast<EvilAction>().ToList();
        }

        public ButtonPressHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task HandleButtonInteraction(SocketMessageComponent component)
        {
            //var guild = _client.GetGuild(component.GuildId.Value);
            var buttonValue = component.Data.CustomId;
            ButtonEncriptionService button = new ButtonEncriptionService();
            button.Decrypt(buttonValue);
            // somthing not realy needed now
            foreach (var Event in _buttonEvents)
            {
                if (Event.Command.Name == button.CommandName)
                {
                    if (Event is IButtonAction buttonEvent)
                    {
                        await buttonEvent.HandleButtonPressAsync(component, button);
                        return;
                    }
                    return;
                }
            }
        }
    }
}
