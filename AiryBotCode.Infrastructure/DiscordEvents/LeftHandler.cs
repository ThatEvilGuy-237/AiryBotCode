using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    /// <summary>Routes UserLeft gateway events to every registered ILeftAction
    /// (kick detection lives in the actions — Discord has no kick event).</summary>
    public class LeftHandler : EvilEventHandler
    {
        private List<EvilAction> _leftActions = new();

        public void AssignActions(List<EvilAction> events)
        {
            _leftActions = events.Where(e => e is ILeftAction).ToList();
            Console.WriteLine("LeftHandler");
        }

        public LeftHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task HandleInteractionAsync(SocketGuild guild, SocketUser user)
        {
            foreach (var action in _leftActions)
            {
                if (action is ILeftAction handler)
                    await handler.HandleLeftAsync(guild, user);
            }
        }
    }
}
