using AiryBotCode.Frontend;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Events.SlashCommands.Commands
{
    public static class TimeoutCommand
    {
        public const string name = "timeout";
        public static SlashCommandBuilder GetCommand()
        {
            var clearMessagesChoices = new[]
            {
                new ApplicationCommandOptionChoiceProperties { Name = "1 hour ago", Value = 60 },
                new ApplicationCommandOptionChoiceProperties { Name = "5 hours", Value = 5 * 60 },
                new ApplicationCommandOptionChoiceProperties { Name = "10 hours", Value = 10 * 60 },
                new ApplicationCommandOptionChoiceProperties { Name = "24 hours", Value = 24 * 60 }
            };
            return new SlashCommandBuilder()
                .WithName(name)
                .WithDescription("Timeout a user and log the action for admins")
                .AddOption("user", ApplicationCommandOptionType.User, "Select the user to timeout", isRequired: true)
                .AddOption("duration", ApplicationCommandOptionType.Integer, "Timeout duration in minutes", isRequired: true)
                .AddOption("reason", ApplicationCommandOptionType.String, "Reason for the timeout", isRequired: true)
                .AddOption("clear", ApplicationCommandOptionType.Integer, "clear messages of the past hours", 
                isRequired: false, 
                choices: clearMessagesChoices);

        }
        private static async Task ClearMessage(int time, SocketGuildUser user)
        {
            if (user == null || user.Guild == null)
                return;

            // Get the guild and text channels
            var guild = user.Guild;
            foreach (var channel in guild.TextChannels)
            {
                try
                {
                    // Fetch recent messages
                    var messages = await channel.GetMessagesAsync(100).FlattenAsync();

                    // Get messages sent by the user within the specified time range
                    var deleteBefore = DateTimeOffset.UtcNow.AddMinutes(-time);
                    var userMessages = messages
                        .Where(m => m.Author.Id == user.Id && m.Timestamp >= deleteBefore)
                        .ToList();

                    // Delete messages in bulk
                    if (userMessages.Count > 0)
                    {
                        var chunkSize = 100;
                        for (var i = 0; i < userMessages.Count; i += chunkSize)
                        {
                            var chunk = userMessages.GetRange(i, Math.Min(chunkSize, userMessages.Count - i));
                            await channel.DeleteMessagesAsync(chunk);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete messages in {channel.Name}: {ex.Message}");
                }
            }
        }

        public static async Task TimeoutUser(SocketSlashCommand command, DiscordSocketClient client)
        {
            // Get user option
            var userOption = command.Data.Options.FirstOrDefault(o => o.Name == "user")?.Value as SocketGuildUser;
            var durationOption = command.Data.Options.FirstOrDefault(o => o.Name == "duration")?.Value;
            var reason = command.Data.Options.FirstOrDefault(o => o.Name == "reason")?.Value?.ToString();
            var clearMessagesOption = command.Data.Options.FirstOrDefault(o => o.Name == "clear_messages")?.Value;
            int clearMessagesTime = clearMessagesOption != null ? Convert.ToInt32(clearMessagesOption) : 0;

            // Validate options
            if (userOption == null || durationOption == null || reason == null || !(durationOption is long durationMinutes))
            {
                await command.RespondAsync("Something went wrong", ephemeral: true);
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
                TimeoutFrontend.RespondToCommand(command, logChannel, userOption, (int)durationMinutes, reason);
            }
            // clear messages
            if (clearMessagesTime > 0)
            {
                await ClearMessage(clearMessagesTime, userOption);
            }
            // Set timeout duration
            var timeoutDuration = TimeSpan.FromMinutes(durationMinutes);
            await userOption.SetTimeOutAsync(timeoutDuration);
        }
    }
}
