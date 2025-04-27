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
        public async Task ClearMessage(int time, SocketGuildUser user)
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
    }
}
