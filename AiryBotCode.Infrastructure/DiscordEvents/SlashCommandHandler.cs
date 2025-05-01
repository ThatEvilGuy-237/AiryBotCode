using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Infrastructure.Interfaces;
using AiryBotCode.Infrastructure.Activitys;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class SlashCommandHandler : MyEventHandeler
    {
        private readonly List<EvilEvent> _slashEvents;

        public SlashCommandHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _slashEvents = new List<EvilEvent>
            {
                serviceProvider.GetRequiredService<TimeoutEvent>(),
                serviceProvider.GetRequiredService<UntimeOutEvent>(),
                serviceProvider.GetRequiredService<UserlogsEvent>(),
                serviceProvider.GetRequiredService<ReminderEvent>(),
            };
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
            foreach (var slashEvent in _slashEvents)
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

            foreach (var slashEvent in _slashEvents)
            {
                if (slashEvent.Command.Name == command.Data.Name)
                {
                    if (slashEvent is ISlashEvent slashEventHandler)
                    {
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
