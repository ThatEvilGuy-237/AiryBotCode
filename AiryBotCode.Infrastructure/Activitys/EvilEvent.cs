using AiryBotCode.Application.Comands;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Activitys
{
    public abstract class EvilEvent
    {
        public EvilCommand Command { get; }
        protected SlashCommandBuilder commandBuilder;
        protected EvilEvent(EvilCommand command, IServiceProvider serviceProvider)
        {
            Command = command;
        }

        public async Task RegisterCommandAsync(IReadOnlyCollection<SocketGuild?> socketGuilds)
        {
            commandBuilder = Command.GetCommand();

            foreach (var guild in socketGuilds)
            {
                if (guild == null) continue;

                await guild.CreateApplicationCommandAsync(commandBuilder.Build());
                Console.WriteLine($"[Register] Command '{commandBuilder.Name}' registered in guild '{guild.Name}'.");
            }
        }

        public async Task RemoveCommandAsync(IReadOnlyCollection<SocketGuild?> socketGuilds)
        {
            foreach (var guild in socketGuilds)
            {
                if (guild == null) continue;

                var existingCommands = await guild.GetApplicationCommandsAsync();
                var targetName = Command.Name;

                foreach (var existingCommand in existingCommands)
                {
                    if (existingCommand.Name == targetName)
                    {
                        await existingCommand.DeleteAsync();
                        Console.WriteLine($"[Remove] Command '{existingCommand.Name}' removed from guild '{guild.Name}'.");
                    }
                }
            }
        }

        // MIGHT BE TODO: If there are more repeating functions that i can put in here.
        // dont know discord enugh
    }
}
