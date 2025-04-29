using Discord.WebSocket;
using Discord;
using AiryBotCode.Tool.Frontend;
using AiryBotCode.Application.Services;
using AiryBotCode.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using AiryBotCode.Application.Services.Loging;
using AiryBotCode.Domain.Entities;

namespace AiryBotCode.Application.Comands.SlashCommands
{
    public class UserlogsCommand : EvilCommand
    {
        public const string ActionEdit = "edit";

        private static readonly ApplicationCommandOptionChoiceProperties[] LogTypeChoices =
        {
            new() { Name = "Warning", Value = "Warning" },
            new() { Name = "Ban", Value = "Ban" },
            new() { Name = "Kick", Value = "Kick" },
            new() { Name = "Mute", Value = "Mute" }
        };

        public UserlogsCommand(IServiceProvider serviceProvider) : base()
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

        public async Task<bool> HandleSlashCommand(SocketSlashCommand command, DiscordSocketClient client)
        {
            var typeString = command.Data.Options.FirstOrDefault(o => o.Name == "type")?.Value?.ToString();
            LogType logType = Enum.TryParse(typeString, ignoreCase: true, out LogType parsedType) ? parsedType : LogType.Other;
            var target = command.Data.Options.FirstOrDefault(o => o.Name == "target")?.Value as SocketGuildUser;

            if (string.IsNullOrEmpty(typeString) || target == null)
            {
                await command.RespondAsync("Invalid input.", ephemeral: true);
                return false;
            }

            var logInfo = new LogInfo(logType, target);
            await SendUserLog(command, client, logInfo);
            return true;
        }

        public async Task SendUserLog(SocketSlashCommand command, DiscordSocketClient client, LogInfo logInfo)
        {
            var buttonEncripter = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = ActionEdit,
                UsersId = new List<ulong> { command.User.Id },
                TargetsId = new List<ulong> { logInfo.Target.Id }
            };

            var logData = new UserLogData();
            logData.ExtractCommandData(command, logInfo.Type);
            logData.SetTarget(logInfo.Target);
            logData.Reason = logInfo.Reason;
            logData.Action = logInfo.Action;
            logData.Consequences = logInfo.Consequences;
            logData = logData.Build();

            var embed = LogFrontend.CreateLogEmbed(logData);

            var buttonId = buttonEncripter.Encript();
            var button = new ComponentBuilder()
                .WithButton("Edit", customId: buttonId, ButtonStyle.Primary)
                .Build();

            var channel = (SocketTextChannel)await client.GetChannelAsync(LogService.LogChannelId);
            await channel.SendMessageAsync(embed: embed, components: button);
            await command.FollowupAsync($"✅ Log has been created! {channel.Mention}", ephemeral: true);
        }

        public async Task<bool> HandelEditButton(SocketMessageComponent component, ButtonEncriptionService buttonEncription)
        {
            if (buttonEncription.Action != ActionEdit) return false;

            if (!buttonEncription.UsersId.Contains(component.User.Id))
            {
                var usersAccess = string.Join(", ", buttonEncription.UsersId.Select(u => $"<@{u}>"));
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
            return true;
        }

        public async Task<bool> HandleForm(SocketModal modal, DiscordSocketClient client, ButtonEncriptionService buttonEncription)
        {
            var channel = client.GetChannel(buttonEncription.ChannelsId[0]) as IMessageChannel;
            var message = await channel?.GetMessageAsync(buttonEncription.MessagesId[0]) as IUserMessage;
            var embed = message?.Embeds.FirstOrDefault();

            if (embed == null || message == null)
            {
                await modal.RespondAsync("❌ No embed found in the message." + message, ephemeral: true);
                return false;
            }

            var logData = LogFrontend.ExtractLogData(embed);
            await LogFrontend.EditLogEmbedAsync(modal, logData, message);
            await modal.RespondAsync("✅ Your changes have been saved.", ephemeral: true);
            return true;
        }
    }
}
