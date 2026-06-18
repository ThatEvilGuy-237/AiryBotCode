using AiryBotCode.Application.Features.Counting;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys
{
    /// <summary>
    /// Counting-game adapter: feeds messages into the game (<see cref="IMessageAction"/>)
    /// and serves the /counting status command (<see cref="ISlashAction"/>).
    /// </summary>
    public class CountingAction : EvilAction, ISlashAction, IMessageAction
    {
        public CountingAction(IServiceProvider serviceProvider)
            : base(serviceProvider.GetRequiredService<CountingCommand>(), serviceProvider)
        {
        }

        public Task ExecuteSlashCommandAsync(SocketSlashCommand command) =>
            ((CountingCommand)Command).HandleStatusAsync(command);

        public Task HandleMessageReceivedAsync(SocketMessage message) =>
            ((CountingCommand)Command).HandleMessageAsync(message);
    }
}
