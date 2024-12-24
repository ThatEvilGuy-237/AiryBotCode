using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Events.SlashCommands.Commands
{
    public enum Options
    {
        Rock,
        Paper,
        Scissors,
        Unknown
    }
    public class RockPaperScissors : BaseSlashCommand
    {

        GameStateRockPaperScissors gameStateRockPaperScissors;
        public RockPaperScissors()
        {
            gameStateRockPaperScissors = new GameStateRockPaperScissors();
            Name = "Rock-Paper-Scissors".ToLower();
            Description = "Play Rock Paper Scissors with some one";
            debug = true;
            blocked = false;
        }

        public override async Task Execute(SocketInteraction interaction, SocketSlashCommand command)
        {
            gameStateRockPaperScissors.interaction = interaction;
            // Choice
            var playerChoice = command.Data.Options
                 .FirstOrDefault(o => o.Name == "choice")?.Value?.ToString();
            Options choiceEnum = Enum.TryParse(playerChoice, ignoreCase: true, out Options result)
                ? result
                : Options.Unknown;
            // choisen opponent
            var opponentId = (command.Data.Options.FirstOrDefault(o => o.Name == "opponent")?.Value as SocketGuildUser)?.Id;

            //not null?
            if (opponentId == null  || choiceEnum == Options.Unknown)
            {
                await command.RespondAsync("Invalid input! Please provide a valid choice and opponent.", ephemeral: true);
                return;
            }
            var player1Id = command.User.Id;
            gameStateRockPaperScissors.Player1Choice = choiceEnum;
          await gameStateRockPaperScissors.StartGame(player1Id, (ulong)opponentId);
            await command.RespondAsync("response", ephemeral: true);
        }

        public async Task InteractionHandeling(SocketInteraction interaction)
        {
            if (interaction is SocketMessageComponent component)
            {
                // Pass the CustomId to your game handling logic
                await gameStateRockPaperScissors.HandleButtonPress(component);
            }
        }

        public override async Task CreateCommand(SocketGuild guild)
        {
            if (blocked) return;
            SlashCommandBuilder command = new SlashCommandBuilder()
                 .WithName(Name)
                 .WithDescription(Description);

           command
                // GAME OPTIONS
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("Choice".ToLower())
                    .WithDescription("Choose your hand!")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true)
                    .AddChoice(Options.Rock.ToString(), Options.Rock.ToString())
                    .AddChoice(Options.Paper.ToString(), Options.Paper.ToString())
                    .AddChoice(Options.Scissors.ToString(), Options.Scissors.ToString())
                    )
                  // SELECT USER THAT YOU WANT TO PLAY GAME WITH
                 .AddOption(new SlashCommandOptionBuilder()
                    .WithName("Opponent".ToLower())
                    .WithDescription("Select the user you want to challenge")
                    .WithType(ApplicationCommandOptionType.User)
                    .WithRequired(true)
                    );
            // REGISTER 
            await guild.CreateApplicationCommandAsync(command.Build());
        }
    }
}
