using Discord.WebSocket;
using Discord;
using AiryBotCode.Tool.Frontend;
using AiryBotCode.Application.Services;
using AiryBotCode.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using System.ComponentModel;

namespace AiryBotCode.Application.Comands.SlashCommands
{
    public class UserlogsCommand : EvilCommand
    {
        public const string ActionEdit = "edit";
        //public const string ActionAddReference = "reference";

        private static readonly ApplicationCommandOptionChoiceProperties[] LogTypeChoices =
        {
            new ApplicationCommandOptionChoiceProperties { Name = "Warning", Value = "Warning" },
            new ApplicationCommandOptionChoiceProperties { Name = "Ban", Value = "Ban" },
            new ApplicationCommandOptionChoiceProperties { Name = "Kick", Value = "Kick" },
            new ApplicationCommandOptionChoiceProperties { Name = "Mute", Value = "Mute" }
        };

        public UserlogsCommand(IServiceProvider serviceProvider)
            : base()
        {
            Name = "userlog";
        }

        public override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Manage user logs")
                .AddOption("type", ApplicationCommandOptionType.String, "Type of log", true, choices: LogTypeChoices)
                .AddOption("target", ApplicationCommandOptionType.User, "Target user", true);
        }

        public async Task<Embed?> CreateLog(SocketSlashCommand command)
        {
            var typeString = command.Data.Options.FirstOrDefault(o => o.Name == "type")?.Value?.ToString();
            LogType type = Enum.TryParse(typeString, ignoreCase: true, out LogType parsedType) ? parsedType : LogType.Other;
            var target = command.Data.Options.FirstOrDefault(o => o.Name == "target")?.Value as SocketUser;

            if (typeString == null || target == null)
            {
                await command.RespondAsync("Invalid input.", ephemeral: true);
                return null;
            }

            var logData = new UserLogData
            {
                TargetMention = target.Mention,
                TargetName = target.Username,
                TargetId = target.Id.ToString(),
                Type = type,
                Reason = "",
                Action = "",
                Consequences = "",
                UserPing = command.User.Mention,
                UserName = command.User.Username,
                TargetAvatarUrl = target.GetAvatarUrl() ?? target.GetDefaultAvatarUrl()
            };
            logData.ExtractCommandData(command, type);
            logData.SetTarget(target);
            logData = logData.Build();

            Embed embed = LogFrontend.CreateLogEmbed(logData);
            await command.RespondAsync("Log prossessing...", ephemeral: true);
            return embed;
        }

        public async Task SendUserLog(SocketSlashCommand command, Embed embed, SocketTextChannel channel)
        {
            var target = command.Data.Options.FirstOrDefault(o => o.Name == "target")?.Value as SocketUser;
            ButtonEncriptionService buttonEncripter = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = ActionEdit,
                UsersId = new List<ulong> { command.User.Id },
                TargetsId = new List<ulong> { target!.Id }
            };

            string buttonId = buttonEncripter.Encript();
            var button = new ComponentBuilder()
                .WithButton("Edit", customId: buttonId, ButtonStyle.Primary)
                .Build();
            await channel.SendMessageAsync(embed: embed, components: button);
            await command.FollowupAsync($"✅ Log has been created! {channel.Mention}", ephemeral: true);
        }
        public async Task<bool> ShowEditForm(SocketMessageComponent component, ButtonEncriptionService buttonEncription)
        {
            switch (buttonEncription.Action)
            {
                case ActionEdit:
                    {
                        if (!buttonEncription.UsersId.Contains(component.User.Id))
                        {
                            string usersAccess = string.Join(", ", buttonEncription.UsersId.Select(u => $"<@{u}>"));
                            await component.RespondAsync($"Only {usersAccess} can edit this message", ephemeral: true);
                            return false;
                        }

                        var embed = component.Message.Embeds.FirstOrDefault();
                        if (embed == null || string.IsNullOrWhiteSpace(embed.Description))
                        {
                            await component.RespondAsync("❌ No valid embed description to edit.", ephemeral: true);
                            return false;
                        }

                        var customId = new ButtonEncriptionService
                        {
                            CommandName = Name,
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

        public async Task<bool> HandleForm(SocketModal modal, DiscordSocketClient client, ButtonEncriptionService buttonEncription)
        {
            var channel = client.GetChannel(buttonEncription.ChannelsId[0]) as IMessageChannel;
            var message = await channel.GetMessageAsync(buttonEncription.MessagesId[0]) as IUserMessage;
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
