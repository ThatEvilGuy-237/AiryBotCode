using Discord.WebSocket;
using Discord;

namespace AiryBotCode.Frontend
{
    public class UntimeoutFrontend
    {
        public static async void RespondToCommand(SocketSlashCommand command, SocketTextChannel channel, SocketGuildUser target)
        {
            await command.RespondAsync($"**{target.Mention} has been un-timed out.**", ephemeral: true);

            // Create an embed for the log
            var embed = new EmbedBuilder()
                .WithTitle("✅ User Un-Timed Out")
                .WithColor(Color.Green)
                .WithThumbnailUrl(target.GetAvatarUrl() ?? target.GetDefaultAvatarUrl()) // Added profile image
                .AddField("👤 User", target.Mention, true)
                .AddField("Un-timed out by", command.User.Mention, false)
                .WithTimestamp(DateTimeOffset.Now)
                .Build();

            var component = new ComponentBuilder()
                .Build();

            // Send message with embed and buttons
            await channel.SendMessageAsync(embed: embed, components: component);
        }

    }
}