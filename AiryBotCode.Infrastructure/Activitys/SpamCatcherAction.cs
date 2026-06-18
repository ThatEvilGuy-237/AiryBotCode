using AiryBotCode.Application.Features.SpamCatcher;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys
{
    /// <summary>
    /// Adapter that feeds every received message into the spam catcher. Implements
    /// only <see cref="IMessageAction"/>, so the reflection-based ActionCatalog picks
    /// it up but the slash-command registrar ignores it.
    /// </summary>
    public class SpamCatcherAction : EvilAction, IMessageAction
    {
        public SpamCatcherAction(IServiceProvider serviceProvider)
            : base(serviceProvider.GetRequiredService<SpamCatcherCommand>(), serviceProvider)
        {
        }

        public async Task HandleMessageReceivedAsync(SocketMessage message)
        {
            await ((SpamCatcherCommand)Command).HandleMessageAsync(message);
        }
    }
}
