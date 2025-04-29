using Discord.WebSocket;
using Discord;
using AiryBotCode.Tool.Frontend;

namespace AiryBotCode.Application.Frontend
{
    public class TimeoutFrontend
    {
        public static async void RespondToCommand(SocketSlashCommand command, SocketTextChannel channel, SocketGuildUser target, int durationMinutes, string reason)
        {
            // Get the current UTC time
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset timeoutEnd = now.AddMinutes(durationMinutes);
            long unixTimestamp = timeoutEnd.ToUnixTimeSeconds();

            await command.RespondAsync($"**{target.Mention} has been timed out for {durationMinutes} minutes.**", ephemeral: true);

            UserLogData userLog = new UserLogData();
            userLog.ExtractCommandData(command, LogType.Timeout);
            userLog.SetTarget(target);
            userLog.Build();

            userLog.Reason = reason;

            // Include UTC time in the action text
            userLog.Action = $"Timed user out for **{durationMinutes} minutes** at {now:yyyy-MM-dd HH:mm:ss} UTC.";

            Embed embed = LogFrontend.CreateLogEmbed(userLog);

            var component = new ComponentBuilder()
                .Build();

            // Send message with embed and buttons
            await channel.SendMessageAsync(embed: embed, components: component);
        }


    }
}