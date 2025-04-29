using Discord.WebSocket;
using Discord;

namespace AiryBotCode.Application.Frontend
{
    public class TimeoutFrontend
    {
        public static async void RespondToCommand(SocketSlashCommand command, SocketTextChannel channel, SocketGuildUser target, int durationMinutes, string reason)
        {
            // Get the current UTC time
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset timeoutEnd = now.AddMinutes(durationMinutes);
            long unixTimestamp = timeoutEnd.ToUnixTimeSeconds(); // Convert to Unix timestamp

            // Get the user's current timeout expiration (if they were already timed out)
            DateTimeOffset? previousTimeout = target.TimedOutUntil;

            // Calculate the total timeout duration
            string totalDuration;
            if (previousTimeout.HasValue && previousTimeout > now)
            {
                TimeSpan remainingTime = previousTimeout.Value - now;
                TimeSpan totalTime = remainingTime + TimeSpan.FromMinutes(durationMinutes);
                totalDuration = $"{(int)totalTime.TotalMinutes} minutes";
            }
            else
            {
                totalDuration = $"{durationMinutes} minutes"; // No previous timeout
            }

            await command.RespondAsync($"**{target.Mention} has been timed out for {durationMinutes} minutes.**", ephemeral: true);

            // Create an embed for the log
            var embed = new EmbedBuilder()
                .WithTitle("🚫 User Timeout For " + durationMinutes + " min")
                .WithColor(Color.DarkRed)
                .WithThumbnailUrl(target.GetAvatarUrl() ?? target.GetDefaultAvatarUrl()) // Added profile image
                .AddField("👤 User", target.Mention, true)
                .AddField("👤 ID",$"`{target.Id}`", true)
                .AddField("⏲ Timed out till", $"<t:{unixTimestamp}:R> <t:{unixTimestamp}:d>", false)
                .AddField("👮‍ Timed out by", command.User.Mention, true)
                .AddField("📜 Reason", reason, true)
                .WithTimestamp(now)
                .Build();

            var component = new ComponentBuilder()
                .Build();

            // Send message with embed and buttons
            await channel.SendMessageAsync(embed: embed, components: component);
        }


    }
}