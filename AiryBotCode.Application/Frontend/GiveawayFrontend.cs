using Discord;

namespace AiryBotCode.Application.Frontend
{
    public static class GiveawayFrontend
    {
        public static Embed CreateGiveawayEmbed()
        {
            return new EmbedBuilder()
                .WithTitle("🎁 Cookie & Cream’s Christmas Giveaway! 🎁")
                .AddField(
                    "Info",
                    "To celebrate the season, we’re hosting a special giveaway where you can win a 1 month of Patreon tier listed bellow. The event stays open until **24 December**, and the winners will be revealed on **25 December**. A small thank-you from us to the community.\n",
                    false
                )
                .AddField(
                    "Prizes",
                    "- 5 winners get **1 month of SFW Tier**\n- 5 winners get **1 month of NSFW Tier**",
                    false
                )
                .AddField(
                    "How to Join",
                    "Click the button below to register for the giveaway and wait for your Christmas present on the 25th of December. Nothing else needed!\n",
                    false
                )
                .AddField(
                    "How do I win?",
                    "There will be 5 winners for SFW and 5 diffrent winners for NSFW Patreon tier. The winners will be chosen randomly and will receive their reward through a **Discord DM**.",
                    false
                )
                .AddField(
                    "Big Thanks",
                    "Thank you all for the support and for being part of Cookie & Cream community. Good luck!",
                    false
                )
                .WithImageUrl("https://iili.io/fq5u5ue.png")
                .WithColor(Color.Gold)
                .Build();

        }

        public static Embed CreateScoreboardEmbed(int userCount, string status)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏆 Giveaway Scoreboard 🏆")
                .WithDescription("Here is the current status of the giveaway event.")
                .AddField("Registered Users", $"**{userCount}** participants", true)
                .AddField("Status", status, true)
                .WithTimestamp(DateTimeOffset.UtcNow)
                .WithFooter("AiryBot Giveaway", "https://i.imgur.com/example.png"); // Replace with a real icon URL if you have one

            if (status == "Accepting Registrations")
            {
                embedBuilder.WithColor(Color.Green);
            }
            else
            {
                embedBuilder.WithColor(Color.Red);
            }

            return embedBuilder.Build();
        }
        public static ModalBuilder CreateGetRandomForm(string customId)
        {
            return new ModalBuilder()
                .WithTitle("Get Random Users")
                .WithCustomId(customId)
                .AddTextInput("Number of users", "num_users", placeholder: "Enter the number of random users to select", required: true);
        }
    }
}
