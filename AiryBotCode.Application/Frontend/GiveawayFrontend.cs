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
                .WithDescription("Click the button below to register for the giveaway.")
                .WithColor(Color.Gold)
                .Build();
        }

        public static Embed CreateScoreboardEmbed(int userCount)
        {
            return new EmbedBuilder()
                .WithTitle("🏆 Giveaway Scoreboard")
                .WithDescription($"**{userCount}** users have registered so far.")
                .WithColor(Color.Blue)
                .Build();
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