using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class BanHandler : EvilEventHandler
    {
        private List<EvilAction> _banAction;
        public void AssignActions(List<EvilAction> events)
        {
            _banAction = events.OfType<ISlashAction>().Cast<EvilAction>().ToList();
            Console.WriteLine("BanHandler");
        }

        public BanHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task HandleInteractionAsync(SocketUser user, SocketGuild guild)
        {
            Console.WriteLine("[UserBan] user: ");

            foreach (var slashEvent in _banAction)
            {
                if (slashEvent is IBanAction slashEventHandler)
                {
                    //TODO: [READ ABOUT RESPONSES TO USER]
                    //await command.DeferAsync(ephemeral: true);
                    Console.WriteLine("- " + user.ToString() + " Guild: " + guild.ToString());
                    await slashEventHandler.HandleBanAsync(user, guild);
                    return;
                }
                return;
            }
        }
    }
}
