using Discord;
using Discord.WebSocket;


namespace AiryBotCode.Application.Frontend
{

    public static class VerifyFrontend
    {
        public static Embed VerifiedUserEmbed(SocketGuildUser user, SocketRole? assignedRole)
        {
            ulong rulesChannelId = 1295836490882089021;

            return new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("✅ Verification Complete")
                .WithDescription(
                    $"{user.Mention}, you are now verified and have been given the role **{assignedRole?.Name ?? "Unknown"}**.\n\n" +
                    $"Please make sure to read the rules in <#{rulesChannelId}>. After you’ve done that, let the team know so we can get you fully settled in.")
                .WithFooter("Airy Guardian")
                .WithCurrentTimestamp()
                .Build();
        }


        public static Embed VerificationLogEmbed(
            SocketGuildUser targetUser,
            SocketGuildUser executedBy,
            SocketRole? assignedRole,
            SocketRole? removedRole)
        {
            return new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithTitle("📜 Verification Log")
                .WithDescription(
                    $"{targetUser.Mention} has been verified by {executedBy.Mention}")
                .AddField("Role Given", assignedRole?.Mention ?? "Not Found", true)
                .AddField("Role Removed", removedRole?.Mention ?? "Not Found", true)
                .WithFooter("Age Verification System")
                .WithCurrentTimestamp()
                .Build();
        }

        public static Embed VerificationErrorEmbed(string reason = "Something went wrong while verifying this user.")
        {
            return new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle("❌ Verification Failed")
                .WithDescription(reason)
                .WithFooter("Age Verification System")
                .WithCurrentTimestamp()
                .Build();
        }
    }

}
