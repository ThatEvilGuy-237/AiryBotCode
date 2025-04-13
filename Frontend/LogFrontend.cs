using Discord.WebSocket;
using Discord;
using System.Text;

public static class LogFrontend
{
    public static Embed CreateLogEmbed(SocketUser author, SocketUser target, string type)
    {
        var descriptionBuilder = new StringBuilder()
            .AppendLine($"👤 **User:** {target.Mention}")
            .AppendLine($"🔹 **Username:** {target.Username}#{target.Discriminator}")
            .AppendLine($"🆔 **User ID:** `{target.Id}`")
            .AppendLine()
            .AppendLine($"🔍 **Type:** {type}")
            .AppendLine($"📜 **Reason:** [Fill in]")
            .AppendLine($"⚡ **Action:** [Fill in]")
            .AppendLine($"🚨 **Consequences:** [Fill in]")
            .AppendLine()
            .AppendLine($"👮 **Logged by:** {author.Mention}");

        var embed = new EmbedBuilder()
            .WithTitle($"User Log - {target.Username}")
            .WithDescription(descriptionBuilder.ToString())
            .WithFooter($"Logged by {author.Username}")
            .WithThumbnailUrl(target.GetAvatarUrl() ?? target.GetDefaultAvatarUrl());

        switch (type)
        {
            case "Warning":
                embed.WithColor(Color.Orange).WithTitle("⚠ Warning Issued");
                break;
            case "Ban":
                embed.WithColor(Color.DarkRed).WithTitle("⛔ User Banned");
                break;
            case "Kick":
                embed.WithColor(Color.Red).WithTitle("🚪 User Kicked");
                break;
            case "Mute":
                embed.WithColor(Color.LightGrey).WithTitle("🔇 User Muted");
                break;
            default:
                embed.WithColor(Color.Blue);
                break;
        }

        return embed.Build();
    }
}
