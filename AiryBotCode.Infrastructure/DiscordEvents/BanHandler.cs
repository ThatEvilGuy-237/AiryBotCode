using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class BanHandler : EvilEventHandler
    {
        private List<EvilAction> _banAction = new();
        public void AssignActions(List<EvilAction> events)
        {
            // Filter on the interface this handler actually dispatches to — the old
            // ISlashAction filter only worked by coincidence.
            _banAction = events.Where(e => e is IBanAction).ToList();
            Console.WriteLine("BanHandler");
        }

        public BanHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task HandleInteractionAsync(SocketUser user, SocketGuild guild)
        {
            Console.WriteLine("[UserBan] user: ");

            foreach (var action in _banAction)
            {
                if (action is IBanAction handler)
                {
                    Console.WriteLine("- " + user.ToString() + " Guild: " + guild.ToString());
                    await handler.HandleBanAsync(user, guild);
                }
            }
        }
    }
}
