using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // Go through each text channel in the guild
                foreach (var channel in user.Guild.TextChannels)
                {
                    // Fetch the most recent 100 messages from the channel
                    var messages = await channel.GetMessagesAsync(limit: 100).FlattenAsync();

                    // Filter the messages by user and time range
                    var userMessages = messages
                        .Where(m => m.Author.Id == user.Id && m.Timestamp >= cutoff)
                        .ToList();

                    // Delete the messages in bulk if any are found
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
