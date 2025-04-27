using Discord.WebSocket;
using Discord;
using AiryBotCode.Events.ButtonPress;
using AiryBotCode.Frontend;

namespace AiryBotCode.Application.Comands.SlashCommands
{
    class UserLogsCommand
    {
        public const int comandId = 2;
        public const string Name = "userlog";
        public const string ActionEdit = "edit";
        //public const string ActionAddReference = "reference";

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

            var logData = new UserLogData
            {
                UserMention = target.Mention,
                Username = target.Username,
                UserId = target.Id.ToString(),
                Type = type,
                Reason = "",
                Action = "",
                Consequences = "",
                LoggedBy = command.User.Mention,
                LoggedByName = command.User.Username,
                AvatarUrl = target.GetAvatarUrl() ?? target.GetDefaultAvatarUrl()
            };

            var embed = LogFrontend.CreateLogEmbed(logData);
            var buttonEncripter = new CustomIdEncription
            {
                Command = Name,
                Action = ActionEdit,
                UsersId = new List<ulong> { command.User.Id },
                TargetsId = new List<ulong> { target.Id }
            };

            var buttonId = buttonEncripter.Encript();
            var button = new ComponentBuilder()
                .WithButton("Edit", customId: buttonId, ButtonStyle.Primary)
                .Build();

            await command.RespondAsync(embed: embed, components: button);
        }

        public static async Task<bool> HandleButtonPress(SocketMessageComponent component, CustomIdEncription button)
        {
            switch (button.Action)
            {
                case ActionEdit:
                    {
                        var embed = component.Message.Embeds.FirstOrDefault();
                        if (embed == null || string.IsNullOrWhiteSpace(embed.Description))
                        {
                            await component.RespondAsync("❌ No valid embed description to edit.", ephemeral: true);
                            return false;
                        }

                        var customId = new CustomIdEncription
                        {
                            Command = Name,
                            Action = ActionEdit,
                            MessagesId = new List<ulong> { component.Message.Id },
                            ChannelsId = new List<ulong> { component.Channel.Id },
                        };

                        var logData = LogFrontend.ExtractLogData(embed);
                        var modal = LogFrontend.CreateEditForm(logData, customId.Encript());

                        await component.RespondWithModalAsync(modal.Build());
                        break;
                    }

                default:
                    return false;
            }

            return true;
        }

        public static async Task<bool> HandleForm(SocketModal modal, DiscordSocketClient client, CustomIdEncription button)
        {
            var channel = client.GetChannel(button.ChannelsId[0]) as IMessageChannel;
            var message = await channel.GetMessageAsync(button.MessagesId[0]) as IUserMessage;
            var embed = message.Embeds.FirstOrDefault();

            if (embed == null)
            {
                await modal.RespondAsync("❌ No embed found in the message.", ephemeral: true);
                return false;
            }

            UserLogData userLogData = LogFrontend.ExtractLogData(embed);

            await LogFrontend.EditLogEmbedAsync(modal, userLogData, message);
            await modal.RespondAsync("✅ Your changes have been saved.", ephemeral: true);
            return true;
        }


    }
}
