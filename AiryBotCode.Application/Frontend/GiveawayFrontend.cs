using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Frontend
{
    public static class GiveawayFrontend
    {
        public static Embed CreateGiveawayEmbed()
        {
            return new EmbedBuilder()
                .WithTitle("🎉 Giveaway Event!")
                .AddField("Description", "Click the button below to register for the giveaway and get a chance to win amazing prizes!", false)
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
