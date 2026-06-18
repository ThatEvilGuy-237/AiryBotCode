using AiryBotCode.Application.Features.Leveling;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Activitys
{
    /// <summary>
    /// Leveling adapter: earns XP from messages (<see cref="IMessageAction"/>) and
    /// serves the /level slash command (<see cref="ISlashAction"/>) — one feature,
    /// two entry points, like GiveawayAction.
    /// </summary>
    public class LevelingAction : EvilAction, ISlashAction, IMessageAction
    {
        public LevelingAction(IServiceProvider serviceProvider)
            : base(serviceProvider.GetRequiredService<LevelingCommand>(), serviceProvider)
        {
        }

        public Task ExecuteSlashCommandAsync(SocketSlashCommand command) =>
            ((LevelingCommand)Command).HandleSlashAsync(command);

        public Task HandleMessageReceivedAsync(SocketMessage message) =>
            ((LevelingCommand)Command).HandleMessageAsync(message);
    }
}
