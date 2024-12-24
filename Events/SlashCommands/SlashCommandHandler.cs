using Discord.WebSocket;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using AiryBotCode.Events.SlashCommands.Commands;

namespace AiryBotCode.Events.SlashCommands
{
    public class SlashCommandHandler : MyEventHandeler
    {
        private readonly RockPaperScissors _rockPaperScissors;
        public SlashCommandHandler(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
            _rockPaperScissors = serviceProvider.GetRequiredService<RockPaperScissors>();
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
            Console.WriteLine("guilds: ");
            foreach (var guild in guilds)
            {
                Console.WriteLine("guild: " + guild.Name + guild.NsfwLevel);
                await ClearExistingCommands(guild);

                await _rockPaperScissors.CreateCommand(guild);
            }           
        }
        public async Task ClearExistingCommands(SocketGuild guild)
        {
            var existingCommands = await guild.GetApplicationCommandsAsync();
            await guild.DeleteApplicationCommandsAsync();

            Console.WriteLine($"All comands where cleard from {guild.Name}.");
        }
        public async Task InteractionHandelingAsync(SocketInteraction interaction)
        {
            await _rockPaperScissors.InteractionHandeling(interaction);
        }

        public async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            if (interaction is not SocketSlashCommand command)
                return;

            if(command.Data.Name == _rockPaperScissors.Name)
            {
               await _rockPaperScissors.Execute(interaction, command);
            }
        }
    }
}
