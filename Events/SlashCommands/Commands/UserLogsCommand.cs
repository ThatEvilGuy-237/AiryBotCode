using AiryBotCode.Frontend;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AiryBotCode.Events.SlashCommands.Commands
{
    class UserLogsCommand
    {
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

            var buttonId = ButtonIdEncoder.Encode(Name, ActionEdit, command.User.Id.ToString(), target.Id.ToString());
            var button = new ComponentBuilder()
                .WithButton("Edit", customId: buttonId, ButtonStyle.Primary)
                .Build();

            await command.RespondAsync(embed: embed, components: button);
        }

        public async Task<bool> HandleButtonPress(SocketMessageComponent component)
        {
            var buttonValue = component.Data.CustomId;
            var decoded = ButtonIdEncoder.Decode(buttonValue);
            //if (decoded == null || decoded.CommandName != Name) return false;

            //switch (decoded.Action)
            //{
            //    case ActionEdit:
            //        await component.RespondAsync($"Editing log for <@{decoded.TargetId}>", ephemeral: true);
            //        break;
            //    case ActionAddReference:
            //        await component.RespondAsync("Feature not implemented yet.", ephemeral: true);
            //        break;
            //    default:
            //        return false;
            //}

            return true;
        }
    }


    public class ButtonIdEncoder
    {
        public static string Encode(string commandName, string action, string actorId, string targetId)
        {
            return $"{commandName}-{action}-{actorId}-{targetId}";
        }

        public static (string CommandName, string Action, string ActorId, string TargetId)? Decode(string buttonId)
        {
            var parts = buttonId.Split('-');
            if (parts.Length < 4) return null;
            return (parts[0], parts[1], parts[2], parts[3]);
        }
    }
}
