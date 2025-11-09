using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace AiryBotCode.Application.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;

        public DiscordService(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task<RestTextChannel?> CreateTextChannelAsync(SocketGuild guild, string channelName, ulong? categoryId = null)
        {
            var channel = await guild.CreateTextChannelAsync(channelName, props =>
            {
                if (categoryId.HasValue)
                {
                    props.CategoryId = categoryId.Value;
                }
            });
            return channel;
        }

        public async Task AddPermissionsToChannelAsync(RestTextChannel channel, List<SocketGuildUser>? usersWithPermissions = null, List<SocketRole>? rolesWithPermissions = null, OverwritePermissions? userPermissions = null, OverwritePermissions? rolePermissions = null)
        {
            // Define default permissions if not provided
            var defaultUserPermissions = userPermissions ?? new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow);
            var defaultRolePermissions = rolePermissions ?? new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow);

            if (usersWithPermissions != null)
            {
                foreach (var user in usersWithPermissions)
                {
                    if (user != null)
                    {
                        await channel.AddPermissionOverwriteAsync(user, defaultUserPermissions);
                    }
                }
            }

            if (rolesWithPermissions != null)
            {
                foreach (var role in rolesWithPermissions)
                {
                    if (role != null)
                    {
                        await channel.AddPermissionOverwriteAsync(role, defaultRolePermissions);
                    }
                }
            }
        }

        public async Task SendSimpleEmbedAsync(IMessageChannel channel, string title, string description, Color color)
        {
            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp()
                .Build();

            await channel.SendMessageAsync(embed: embed);
        }

        public async Task RespondToCommandAsync(SocketSlashCommand command, string message = null, bool ephemeral = false, Embed embed = null, MessageComponent components = null)
        {
            await command.RespondAsync(text: message, ephemeral: ephemeral, embed: embed, components: components);
        }

        public async Task SendMessageToChannelAsync(ulong channelId, string message = null, Embed embed = null, MessageComponent components = null)
        {
            if (_client.GetChannel(channelId) is IMessageChannel channel)
            {
                await channel.SendMessageAsync(text: message, embed: embed, components: components);
            }
        }

        public async Task AddRoleToUserAsync(SocketGuildUser user, ulong roleId)
        {
            var role = user.Guild.GetRole(roleId);
            if (role != null && !user.Roles.Contains(role))
            {
                await user.AddRoleAsync(role);
            }
        }

        public async Task RemoveRoleFromUserAsync(SocketGuildUser user, ulong roleId)
        {
            var role = user.Guild.GetRole(roleId);
            if (role != null && user.Roles.Contains(role))
            {
                await user.RemoveRoleAsync(role);
            }
        }

        public async Task<SocketTextChannel?> GetTextChannelAsync(ulong channelId)
        {
            return await _client.GetChannelAsync(channelId) as SocketTextChannel;
        }
    }
}