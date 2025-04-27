using Discord.WebSocket;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Events.SlashCommands.Commands;
using AiryBotCode.Events.SendMessage.MessageComands.TalkWithAiry;

namespace AiryBotCode.Events.SlashCommands
{
    public enum CommandType
    {
        Timeout,
    }
    public class SlashCommandHandler : MyEventHandeler
    {
        private readonly TimeoutCommand _timeoutCommand;
        private readonly UntimeoutCommand _untimeoutCommand;
        private readonly UserLogsCommand _userLogsCommand;
        public SlashCommandHandler(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
            _timeoutCommand = _serviceProvider.GetRequiredService<TimeoutCommand>();
            _untimeoutCommand = _serviceProvider.GetRequiredService<UntimeoutCommand>();
            _userLogsCommand = _serviceProvider.GetRequiredService<UserLogsCommand>();
        }
        public async Task RegisterComands()
        {
            // Get all of the servers
            IReadOnlyCollection<SocketGuild> guilds = _client.Guilds;

            if (!guilds.Any())
            {
                Console.WriteLine("Bot is not in any guilds.");
                return;
            }

            // Define the commands to register
            var commands = new List<SlashCommandBuilder>
            {
               _timeoutCommand.GetCommand(),
               _untimeoutCommand.GetCommand(),
               _userLogsCommand.GetCommand(),
            };

            // Register commands for all guilds
            foreach (var guild in guilds)
            {
                Console.WriteLine($"Registering commands in guild: {guild.Name} ({guild.Id})");
                var existingCommands = await guild.GetApplicationCommandsAsync();
                foreach (var command in existingCommands)
                {
                    await command.DeleteAsync();
                    Console.WriteLine($"Deleted command: {command.Name}");
                }

                foreach (var command in commands)
                {
                    try
                    {
                        await guild.CreateApplicationCommandAsync(command.Build());
                        Console.WriteLine($"Command {command.Name} registered in guild {guild.Name}.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error registering command {command.Name} in guild {guild.Name}: {ex.Message}");
                    }
                }
            }
        }

        public async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            if (interaction is not SocketSlashCommand command)
                return;

            switch (command.Data.Name)
            {
                case TimeoutCommand.name:
                    await _timeoutCommand.TimeoutUser(command, _client);
                    break;
                case UntimeoutCommand.name:
                    await _untimeoutCommand.UntimeoutUser(command, _client);
                    break;
                case UserLogsCommand.Name:
                    await _userLogsCommand.Log(command, _client);
                    break;
                case "greet":
                    var name = command.Data.Options.FirstOrDefault()?.Value?.ToString();
                    await command.RespondAsync($"Hello, {name}!");
                    break;

                default:
                    await command.RespondAsync("Unknown command.");
                    break;
            }
        }
    }
}
