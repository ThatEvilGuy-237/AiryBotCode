using Discord.WebSocket;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Events.SlashCommands
{
    public class JoinServerHandler : MyEventHandeler
    {
        public JoinServerHandler(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
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
                new SlashCommandBuilder()
                    .WithName("ping")
                    .WithDescription("Replies with Pong!"),

                new SlashCommandBuilder()
                    .WithName("greet")
                    .WithDescription("Greets the user.")
                    .AddOption("name", ApplicationCommandOptionType.String, "Your name", isRequired: true),

                // Add more commands as needed
            };

            // Register commands in each guild
            foreach (var guild in guilds)
            {
                Console.WriteLine($"Registering commands in guild: {guild.Name} ({guild.Id})");
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
                case "ping":
                    await command.RespondAsync("Pong!");
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
