using Discord;
using Discord.WebSocket;
using AiryBotCode.Frontend;

namespace AiryBotCode.Application.Comands.SlashCommands
{
    public class UntimeoutCommand
    {
        public const string name = "untimeout";

        public SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(name)
                .WithDescription("Remove a user's timeout and log the action for admins")
                .AddOption("user", ApplicationCommandOptionType.User, "Select the user to untimeout", isRequired: true);
        }

        public async Task UntimeoutUser(SocketSlashCommand command, DiscordSocketClient client)
        {
            // Get user option
            var userOption = command.Data.Options.FirstOrDefault(o => o.Name == "user")?.Value as SocketGuildUser;

            // Validate options
            if (userOption == null)
            {
                await command.RespondAsync("Something went wrong", ephemeral: true);
                return;
            }
            if (userOption.TimedOutUntil == null || userOption.TimedOutUntil <= DateTimeOffset.UtcNow)
            {
                await command.RespondAsync($"{userOption.Mention} is not currently timed out.", ephemeral: true);
                return;
            }
            // Get admin role ID
            var adminRoleId = ulong.TryParse(Environment.GetEnvironmentVariable("ADMINROLEID"), out var roleId) ? roleId : 0;
            if (adminRoleId == 0)
            {
                await command.RespondAsync("Admin role ID not found in environment variables. CONTACT evil", ephemeral: true);
                return;
            }

            // Check user permissions
            var user = command.User as SocketGuildUser;
            if (user == null || !user.Roles.Any(r => r.Id == adminRoleId))
            {
                await command.RespondAsync("You do not have permission to use this command.", ephemeral: true);
                return;
            }

            // Log action
            var logChannel = client.GetGuild(command.GuildId!.Value)?.GetTextChannel(1182267222152982533);
            if (logChannel != null)
            {
                UntimeoutFrontend.RespondToCommand(command, logChannel, userOption);
            }

            // Remove timeout
            await userOption.RemoveTimeOutAsync();
            await command.RespondAsync($"Timeout removed for {userOption.Username}.", ephemeral: true);
        }
    }
}
