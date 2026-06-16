using AiryBotCode.Application.Consent;
using AiryBotCode.Application.Hive;
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

            // Await-mode ask_user answer: the user tapped an option button raised by the
            // Hive's ask_user tool. Send their choice back up the WS as the effect_response
            // (correlated by effect id) so the suspended agent loop resumes — then edit the
            // message to reflect the choice. Handled first; it isn't a command button.
            var ask = AskInteraction.TryParseAnswerId(buttonValue);
            if (ask != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var sender = scope.ServiceProvider.GetService<IHiveResponseSender>();
                var sent = sender != null && await sender.SendAnswerAsync(
                    ask.Value.EffectId, ask.Value.Answer,
                    component.Channel.Id.ToString(), component.User.Id.ToString());

                // Edit the original message: show the picked answer, drop the buttons.
                var note = sent ? $"\n\n*You chose: **{ask.Value.Answer}***"
                                : "\n\n*(Couldn't reach Airy — the question may have timed out.)*";
                await component.UpdateAsync(m =>
                {
                    m.Content = (component.Message?.Content ?? "") + note;
                    m.Components = new Discord.ComponentBuilder().Build();
                });
                return;
            }

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
