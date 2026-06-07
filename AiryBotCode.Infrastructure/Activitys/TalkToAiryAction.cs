using AiryBotCode.Application.Features.Conversational;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


namespace AiryBotCode.Infrastructure.Activitys
{
    public class TalkToAiryAction : EvilAction, IMessageAction
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
    }
}
