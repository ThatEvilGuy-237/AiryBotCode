using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class SlashCommandHandler : EvilEventHandler
    {
        private List<EvilAction> _slashAction;
        public void AssignActions(List<EvilAction> events)
        {
            _slashAction = events.OfType<ISlashAction>().Cast<EvilAction>().ToList();
            Console.WriteLine("SlashCommandHandler");
        }

        public SlashCommandHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task RegisterCommandsAsync()
        {
            var guilds = _client.Guilds;

            if (!guilds.Any())
            {
                Console.WriteLine("[RegisterCommands] Bot is not in any guilds.");
                return;
            }

            // More optimal but eh
            //// Remove old commands
            foreach (var guild in guilds)
            {
                await ClearServerCommands(guild);
            }

            // Register new commands
            foreach (var slashEvent in _slashAction)
            {
                await slashEvent.RegisterCommandAsync(guilds);
            }
        }
        public async Task ClearServerCommands(SocketGuild guild)
        {
            var applicationCommands = await guild.GetApplicationCommandsAsync();

            foreach (var command in applicationCommands)
            {
                if (command.ApplicationId == _client.CurrentUser.Id) // correct way
                {
                    try
                    {
                        await command.DeleteAsync();
                        Console.WriteLine($"[Delete] Command '{command.Name}' deleted in guild '{guild.Name}'.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting command '{command.Name}' in guild '{guild.Name}': {ex.Message}");
                    }
                }
            }
        }

        public async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            if (interaction is not SocketSlashCommand command)
                return;

            foreach (var slashEvent in _slashAction)
            {
                if (slashEvent.Command.Name == command.Data.Name)
                {
                    if (slashEvent is ISlashAction slashEventHandler)
                    {
                        //TODO: [READ ABOUT RESPONSES TO USER]
                        //await command.DeferAsync(ephemeral: true);
                        await slashEventHandler.ExecuteSlashCommandAsync(command);
                        return;
                    }
                    return;
                }
            }

            await command.RespondAsync("Unknown command.");
        }
    }
}
