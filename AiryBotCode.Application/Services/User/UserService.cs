using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Application.Services.User
{
    public class UserService
    {
        public async Task<int> ClearMessage(int timeInMinutes, SocketGuildUser user)
        {
            if (user == null || user.Guild == null)
                return 0;

            int totalDeleted = 0;
            var cutoff = DateTimeOffset.UtcNow.AddMinutes(-timeInMinutes);

            try
            {
                foreach (var channel in user.Guild.TextChannels)
                {
                    // Fetch the most recent 100 messages from the channel
                    var messages = await channel.GetMessagesAsync(limit: 100).FlattenAsync();

                    // Filter the messages by user and time range
                    var userMessages = messages
                        .Where(m => m.Author.Id == user.Id && m.Timestamp >= cutoff)
                        .ToList();

                    // Delete the messages
                    if (userMessages.Count > 0)
                    {
                        await channel.DeleteMessagesAsync(userMessages);
                        totalDeleted += userMessages.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ClearMessage] Error: {ex.Message}");
            }

            return totalDeleted;
        }

        /// <summary>
        /// Delete a user's own recent messages across the guild — the same proven approach
        /// as the /timeout "clear" sweep (<see cref="ClearMessage"/>): walk the guild's text
        /// channels, gather everything authored by this user inside the window, and delete
        /// it. The spam catcher uses this so a caught burst is wiped WHOLESALE (gathered BY
        /// USER) instead of relying on the handful of message ids that tripped the threshold,
        /// so stragglers — and messages we never got a gateway event for — get cleared too.
        /// Differs from <see cref="ClearMessage"/> only in taking a seconds (not minutes)
        /// window, so it stays tight to the spam burst.
        /// </summary>
        public async Task<int> ClearUserMessages(SocketGuildUser user, int withinSeconds)
        {
            if (user?.Guild == null) return 0;

            int totalDeleted = 0;
            var cutoff = DateTimeOffset.UtcNow.AddSeconds(-Math.Max(1, withinSeconds));

            foreach (var channel in user.Guild.TextChannels)
            {
                try
                {
                    var messages = await channel.GetMessagesAsync(limit: 100).FlattenAsync();
                    var userMessages = messages
                        .Where(m => m.Author.Id == user.Id && m.Timestamp >= cutoff)
                        .ToList();

                    if (userMessages.Count > 0)
                    {
                        await channel.DeleteMessagesAsync(userMessages);
                        totalDeleted += userMessages.Count;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ClearUserMessages] channel {channel.Id}: {ex.Message}");
                }
            }

            return totalDeleted;
        }

        public async Task<ulong> GetAdminRole(SocketInteraction command)
        {
            var adminRoleId = ulong.TryParse(Environment.GetEnvironmentVariable("ADMINROLEID"), out var roleId) ? roleId : 0;
            if (adminRoleId == 0)
            {
                await command.RespondAsync("Admin role ID not found in environment variables. CONTACT evil", ephemeral: true);
                return 0;
            }
            return adminRoleId;
        }

        public async Task<bool> UserIsAdmin(SocketInteraction command)
        {
            var adminRoleId = await GetAdminRole(command);
            var user = command.User as SocketGuildUser;
            if (user == null || !user.Roles.Any(r => r.Id == adminRoleId))
            {
                await command.RespondAsync("You do not have permission to use this command.", ephemeral: true);
                return false;
            }
            return true;
        }

        public async Task TimeOutUser(SocketInteraction command, SocketGuildUser target, int durationMinutes)
        {
            if (target == null)
            {
                await command.RespondAsync("Somthing went wrong." + target.ToString(), ephemeral: true);
                return;
            }
            var timeoutDuration = TimeSpan.FromMinutes(durationMinutes);
            await target.SetTimeOutAsync(timeoutDuration);
        }
        // Overload for non-interaction callers (e.g. the spam catcher reacting to a
        // message rather than a slash command).
        public async Task TimeOutUser(SocketGuildUser target, int durationMinutes)
        {
            if (target == null) return;
            await target.SetTimeOutAsync(TimeSpan.FromMinutes(durationMinutes));
        }

        public async Task UntimeOut(SocketInteraction command, SocketGuildUser target)
        {
            if (target == null)
            {
                await command.RespondAsync("Somthing went wrong." + target.ToString(), ephemeral: true);
                return;
            }
            await target.RemoveTimeOutAsync();
        }
    }
}
