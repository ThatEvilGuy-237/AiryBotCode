using Discord.WebSocket;
using Discord;
using AiryBotCode.Events.ButtonPress;

namespace AiryBotCode.Events.SlashCommands.Commands
{
    class UserLogsCommand
    {
        public const int comandId = 2;
        public const string Name = "userlog";
        public const string ActionEdit = "edit";
        public const string ActionAddReference = "reference";

        private static readonly ApplicationCommandOptionChoiceProperties[] LogTypeChoices =
        {
            new ApplicationCommandOptionChoiceProperties { Name = "Warning", Value = "Warning" },
            new ApplicationCommandOptionChoiceProperties { Name = "Ban", Value = "Ban" },
            new ApplicationCommandOptionChoiceProperties { Name = "Kick", Value = "Kick" },
            new ApplicationCommandOptionChoiceProperties { Name = "Mute", Value = "Mute" }
        };

        public SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Manage user logs")
                .AddOption("type", ApplicationCommandOptionType.String, "Type of log", true, choices: LogTypeChoices)
                .AddOption("target", ApplicationCommandOptionType.User, "Target user", true);
        }
      
        public async Task Log(SocketSlashCommand command, DiscordSocketClient client)
        {
            var type = command.Data.Options.FirstOrDefault(o => o.Name == "type")?.Value?.ToString();
            var target = command.Data.Options.FirstOrDefault(o => o.Name == "target")?.Value as SocketUser;

            if (type == null || target == null)
            {
                await command.RespondAsync("Invalid input.", ephemeral: true);
                return;
            }

            var embed = LogFrontend.CreateLogEmbed(command.User, target, type);
            ButtonEncoder buttonEncripter = new ButtonEncoder
            {
                Command = Name,
                Action = ActionEdit,
                UsersId = new List<ulong> { command.User.Id },
                TargetId = new List<ulong> { target.Id }
            };
            var buttonId = buttonEncripter.Encript();
            var button = new ComponentBuilder()
                .WithButton("Edit", customId: buttonId, ButtonStyle.Primary)
                .Build();

            await command.RespondAsync(embed: embed, components: button);
        }

        public static async Task<bool> HandleButtonPress(SocketMessageComponent component, ButtonEncoder button)
        {
            var buttonValue = component.Data.CustomId;

            switch (button.Action)
            {
                case ActionEdit:
                    await component.RespondAsync($"Editing log for <@{button.TargetId[0]}>", ephemeral: true);
                    


                    break;
                case ActionAddReference:
                    await component.RespondAsync("Feature not implemented yet.", ephemeral: true);
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
