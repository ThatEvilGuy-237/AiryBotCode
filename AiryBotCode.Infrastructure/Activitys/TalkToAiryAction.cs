using AiryBotCode.Application.Features.Conversational;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


using AiryBotCode.Application.Services;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class TalkToAiryAction : EvilAction, IMessageAction, IButtonAction
    {
        public TalkToAiryAction(
            IServiceProvider serviceProvider
        ) : base(serviceProvider.GetRequiredService<TalkToAiry>(), serviceProvider)
        {
        }

        public async Task HandleMessageReceivedAsync(SocketMessage message)
        {
            var talkToAiry = (TalkToAiry)Command;
            await talkToAiry.ProcessMessageAsync(message);
        }

        public async Task HandleButtonPressAsync(SocketMessageComponent component, ButtonEncriptionService buttonEncription)
        {
            var talkToAiry = (TalkToAiry)Command;
            await talkToAiry.HandleInteractionAsync(component);
        }
    }
}
