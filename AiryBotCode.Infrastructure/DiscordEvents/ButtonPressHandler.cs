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
            // Hive's ask_user tool. Context rides in the customId via the canonical
            // ButtonEncriptionService — c:ask_user | a:<effectId>.<idx> | u:<askerId>.
            // Restrict to the asker, read the chosen answer from the pressed button's
            // LABEL, then send it back up the WS as the effect_response (correlated by
            // effect id) so the suspended agent loop resumes.
            if (buttonValue.StartsWith($"c:{AskRouter.Command}|"))
            {
                var data = new ButtonEncriptionService().Decrypt(buttonValue);
                var effectId = (data.Action ?? "").Split('.')[0];   // strip the .<idx> uniqueness suffix

                // Only the user the agent asked may answer (when a restriction was set).
                if (data.UsersId.Count > 0 && !data.UsersId.Contains(component.User.Id))
                {
                    await component.RespondAsync("This question isn't for you.", ephemeral: true);
                    return;
                }

                // The answer is the pressed button's label (matched by customId in the
                // message's components); fall back to the option index if not found.
                var answer = component.Message?.Components
                    .SelectMany(row => row.Components)
                    .OfType<Discord.ButtonComponent>()
                    .FirstOrDefault(b => b.CustomId == buttonValue)?.Label
                    ?? (data.Action ?? "");

                // ACK the click FIRST (within Discord's 3s window) as a deferred message
                // update. Sending the answer back over the Hive WS can take longer than 3s
                // (and stalls during a WS reconnect); without this, Discord shows
                // "This interaction failed" AND re-delivers the click, firing this handler
                // twice (a duplicate effect_response). After deferring we have ~15 min to
                // edit the original message via ModifyOriginalResponseAsync.
                await component.DeferAsync();

                using var scope = _serviceProvider.CreateScope();
                var sender = scope.ServiceProvider.GetService<IHiveResponseSender>();
                var sent = sender != null && !string.IsNullOrEmpty(effectId)
                    && await sender.SendAnswerAsync(effectId, answer, component.Channel.Id.ToString(), component.User.Id.ToString());

                var note = sent ? $"\n\n*You chose: **{answer}***"
                                : "\n\n*(Couldn't reach Airy — the question may have timed out.)*";
                await component.ModifyOriginalResponseAsync(m =>
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
