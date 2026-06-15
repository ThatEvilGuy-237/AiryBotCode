using AiryBotCode.Application.Consent;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class ButtonPressHandler : EvilEventHandler
    {
        private List<EvilAction> _buttonEvents;

        public ButtonPressHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void AssignActions(List<EvilAction> events)
        {
            _buttonEvents = events.OfType<IButtonAction>().Cast<EvilAction>().ToList();
            Console.WriteLine("ButtonPressHandler");
        }


        public async Task HandleButtonInteraction(SocketMessageComponent component)
        {
            //var guild = _client.GetGuild(component.GuildId.Value);
            var buttonValue = component.Data.CustomId;

            // First-message consent gate's Accept button (handled before the normal
            // command-button dispatch — it isn't an encrypted command button).
            var consent = ConsentInteraction.TryParseAcceptId(buttonValue);
            if (consent != null)
            {
                if (component.User.Id != consent.Value.UserId)
                {
                    await component.RespondAsync("This consent prompt isn't for you.", ephemeral: true);
                    return;
                }
                using var scope = _serviceProvider.CreateScope();
                await scope.ServiceProvider.GetRequiredService<IUserConsentRepository>()
                    .GrantAsync(consent.Value.BotId, consent.Value.UserId);
                await component.RespondAsync("✅ Thanks — you're all set! Send your message again and Airy will reply.", ephemeral: true);
                return;
            }

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
