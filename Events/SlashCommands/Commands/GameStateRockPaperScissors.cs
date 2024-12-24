using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System.ComponentModel;

namespace AiryBotCode.Events.SlashCommands.Commands
{
    public class GameStateRockPaperScissors
    {

        public ulong Player1Id { get; set; }
        public Options Player1Choice { get; set; }
        public ulong Player2Id { get; set; }
        public Options Player2Choice { get; set; }
        public SocketInteraction interaction { get; set; }
        public RestUserMessage gameEmbed { get; set; }

        List<string> buttonIds = new List<string>();

        public async Task StartGame(ulong userStarted, ulong targetedUser)
        {
            Player1Id = userStarted;
            Player2Id = targetedUser;

            // Create an embed for the game
            var embed = new EmbedBuilder()
               .WithTitle("Rock-Paper-Scissors Game")
               .WithDescription($"<@{Player1Id}> has challenged <@{Player2Id}> to a game of Rock-Paper-Scissors! <@{Player2Id}>, make your choice by pressing one of the buttons below.")
               .WithColor(Color.Blue)
               .AddField("Player 1", $"<@{Player1Id}>")
               .AddField("Player 2", $"<@{Player2Id}>")
               .Build();

            // buttons id
            string buttonRockId = $"rps-{Options.Rock.ToString()}_{Player2Id}";
            string buttonPaperId = $"rps-{Options.Paper.ToString()}_{Player2Id}";
            string buttonScissorsId = $"rps-{Options.Scissors.ToString()}_{Player2Id}";

            buttonIds.Add(buttonRockId);
            buttonIds.Add(buttonPaperId);
            buttonIds.Add(buttonScissorsId);
            // Create buttons for Player 2
            var components = new ComponentBuilder()
                .WithButton(Options.Rock.ToString(), buttonRockId, ButtonStyle.Primary)
                .WithButton(Options.Paper.ToString(), buttonPaperId, ButtonStyle.Primary)
                .WithButton(Options.Scissors.ToString(), buttonScissorsId, ButtonStyle.Primary)
                .Build();

            // Send the embed with buttons
            await interaction.RespondAsync(embed: embed, components: components);

            gameEmbed = await interaction.GetOriginalResponseAsync();
        }

        public async Task HandleButtonPress(SocketMessageComponent component)
        {
            if (component.Data.CustomId == null) return;
            // Extract the action from the button ID
            Options action = component.Data.CustomId switch
            {
                var id when id.Contains(Options.Rock.ToString()) => Options.Rock,
                var id when id.Contains(Options.Paper.ToString()) => Options.Paper,
                var id when id.Contains(Options.Scissors.ToString()) => Options.Scissors,
                _ => Options.Unknown
            };

            string buttonRockId = $"rps-{Options.Rock.ToString()}_{Player2Id}";
            string buttonPaperId = $"rps-{Options.Paper.ToString()}_{Player2Id}";
            string buttonScissorsId = $"rps-{Options.Scissors.ToString()}_{Player2Id}";
            // Rebuild the buttons as disabled
            var disabledComponents = new ComponentBuilder()
                                .WithButton(Options.Rock.ToString(), buttonRockId, ButtonStyle.Primary, disabled: true)
                .WithButton(Options.Paper.ToString(), buttonPaperId, ButtonStyle.Primary, disabled: true)
                .WithButton(Options.Scissors.ToString(), buttonScissorsId, ButtonStyle.Primary, disabled: true)
                .Build();
            // If the action is unknown, return early
            if (action == Options.Unknown) return;

            await DisableButtons(component);
            Player2Choice = action;

            await DetermineWinner(component);
        }

        public async Task DetermineWinner(SocketMessageComponent component)
        {
            ulong winner = 0;
            ulong loser = 0;
            string actionWinner = "";
            string actionLoser = "";
            if (Player1Choice == Player2Choice)
            {
                // It's a tie, no winner or loser
                winner = 0;
                loser = 0;
                await component.RespondAsync("Stell mate.");
                return;
            }

            bool player1Wins = (Player1Choice == Options.Rock && Player2Choice == Options.Scissors) ||
                               (Player1Choice == Options.Paper && Player2Choice == Options.Rock) ||
                               (Player1Choice == Options.Scissors && Player2Choice == Options.Paper);

            if (player1Wins)
            {
                winner = Player1Id;
                loser = Player2Id;
                actionWinner = Player1Choice.ToString();
                actionLoser = Player2Choice.ToString();
            }
            else
            {
                winner = Player2Id;
                loser = Player1Id;
                actionWinner = Player2Choice.ToString();
                actionLoser = Player1Choice.ToString();
            }

            var embed = new EmbedBuilder()
            .WithTitle("Rock-Paper-Scissors Game Results")
            .WithDescription($"The game between <@{Player1Id}> and <@{Player2Id}> has concluded!")
            .WithColor(Color.Green)
            .AddField("WINNER", $"🎉 <@{winner}> chose **{actionWinner}**", true)
            .AddField("LOSER!", $"😢 <@{loser}> chose **{actionLoser}**", true)
            .Build();

            await gameEmbed.Channel.SendMessageAsync(embed:embed, messageReference: new MessageReference(gameEmbed.Id));
        }

        public async Task DisableButtons(SocketMessageComponent component)
        {
            // Rebuild the buttons as disabled
            var disabledComponentsBuilder = new ComponentBuilder();

            foreach (var id in buttonIds)
            {
                string label = id.Contains(Options.Rock.ToString()) ? Options.Rock.ToString() :
                               id.Contains(Options.Paper.ToString()) ? Options.Paper.ToString() :
                               id.Contains(Options.Scissors.ToString()) ? Options.Paper.ToString() : Options.Unknown.ToString();

                disabledComponentsBuilder.WithButton(label, id, ButtonStyle.Primary, disabled: true);
            }

            // Update the original message to gray out buttons
            await component.Message.ModifyAsync(msg =>
            {
                msg.Components = disabledComponentsBuilder.Build();
            });
        }

    }
}
