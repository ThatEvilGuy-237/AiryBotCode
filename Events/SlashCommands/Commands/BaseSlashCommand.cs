using Discord.WebSocket;
using Discord;
namespace AiryBotCode.Events.SlashCommands.Commands
{
    public class BaseSlashCommand
    {
        public string Name { get; set; } = "no name Name";
        protected string Description { get; set; } = "no name Description";
        protected bool debug { get; set; } = true;
        protected bool blocked { get; set; } = true;
        public BaseSlashCommand()
        {

        }
        public virtual Task Execute(SocketInteraction interaction, SocketSlashCommand command)
        {
            throw new Exception("You cant user this class to Execute command BaseSlashCommand");
        }
        public virtual async Task CreateCommand(SocketGuild guild)
        {
            throw new Exception("You cant user this class to create command BaseSlashCommand");
        }
        public virtual async Task InteractionHandeling(SocketInteraction interaction)
        {
            throw new Exception("You cant user this class for Interaction command BaseSlashCommand");
        }

        public bool LimitedAccess()
        {
            return debug;
        }
    }
}
