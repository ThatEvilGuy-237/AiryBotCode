using AiryBotCode.Application.Services;
using AiryBotCode.Application.Services.Loging;
using AiryBotCode.Application.Services.User;
using AiryBotCode.Domain.Entities;
using AiryBotCode.Tool.Frontend;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Comands.SlashCommands
{
    public class UserlogsCommand : EvilCommand
    {
        public const string ActionEdit = "edit";
        protected LogService _logservice;
        protected UserService _userService;
        private static readonly ApplicationCommandOptionChoiceProperties[] LogTypeChoices =
        {
            new() { Name = "Warning", Value = "Warning" },
            new() { Name = "Ban", Value = "Ban" },
            new() { Name = "Kick", Value = "Kick" },
            new() { Name = "Mute", Value = "Mute" }
        };

        public UserlogsCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "userlog";
            _logservice = serviceProvider.GetRequiredService<LogService>();
            _userService = serviceProvider.GetRequiredService<UserService>();
        }

        public override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Manage user logs")
                .AddOption("type", ApplicationCommandOptionType.String, "Type of log", true, choices: LogTypeChoices)
                .AddOption("target", ApplicationCommandOptionType.User, "Target user", true);
        }

        public async Task<bool> HandleSlashCommand(SocketSlashCommand command)
        {
            var typeString = command.Data.Options.FirstOrDefault(o => o.Name == "type")?.Value?.ToString();
            LogInfo logInfo = new LogInfo()
            {
                Target = command.Data.Options.FirstOrDefault(o => o.Name == "target")?.Value as SocketGuildUser,
                Type = Enum.TryParse(typeString, ignoreCase: true, out LogType parsedType) ? parsedType : LogType.Other
            };

            if (string.IsNullOrEmpty(typeString) || logInfo.Target == null)
            {
                await command.RespondAsync("Invalid inputs.", ephemeral: true);
                return false;
            }
            await command.RespondAsync($"Prossessing..", ephemeral: true);
            await SendUserLog(command, logInfo);
            return true;
        }

        public async Task SendUserLog(SocketSlashCommand command, LogInfo logInfo)
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

            var channel = (SocketTextChannel)await _client.GetChannelAsync(await _logservice.GetLogChannelId());
            await channel.SendMessageAsync(embed: embed, components: button);
            await command.FollowupAsync($"✅ Log has been created! {channel.Mention}", ephemeral: true);
        }
        public async Task SendUserLog(IUser user, LogInfo logInfo)
        {
            var buttonEncripter = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = ActionEdit,
                UsersId = new List<ulong> { user.Id },
                TargetsId = new List<ulong> { logInfo.Target.Id }
            };

            var logData = new UserLogData();
            logData.SetUser(user);
            logData.Type = logInfo.Type;
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

            var channel = (SocketTextChannel)await _client.GetChannelAsync(await _logservice.GetLogChannelId());
            await channel.SendMessageAsync(embed: embed, components: button);
        }


        public async Task<bool> HandelEditButton(SocketMessageComponent component, ButtonEncriptionService buttonEncription)
        {
            if (buttonEncription.Action != ActionEdit) return false;
            ulong makerId = buttonEncription.UsersId.FirstOrDefault();
            IUser makerUser = await _client.GetUserAsync(makerId);
            bool hasAccessPass = false;
            if (makerUser != null && makerUser.IsBot)
            {
                bool isAdmin = await _userService.UserIsAdmin(component);
                if (!isAdmin)
                {
                    await component.RespondAsync("Only admins can edit logs created by bots.", ephemeral: true);
                    return false;
                }
                hasAccessPass = true;
            }

            if (!hasAccessPass && !buttonEncription.UsersId.Contains(component.User.Id))
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

        public async Task<UserlogFormInfo> HandleForm(SocketModal modal, ButtonEncriptionService buttonEncription)
        {
            var channel = _client.GetChannel(buttonEncription.ChannelsId[0]) as IMessageChannel;
            var message = await channel?.GetMessageAsync(buttonEncription.MessagesId[0]) as IUserMessage;
            UserlogFormInfo info = new UserlogFormInfo()
            {
                Channel = channel,
                Message = message,
                Embed = message?.Embeds.FirstOrDefault()
            };

            if (info.Embed == null || message == null)
            {
                await modal.RespondAsync("❌ No embed found in the message." + message, ephemeral: true);
                return info;
            }

            var logData = LogFrontend.ExtractLogData(info.Embed);
            await LogFrontend.EditLogEmbedAsync(modal, logData, info.Message);
            await modal.RespondAsync("✅ Your changes have been saved.", ephemeral: true);
            return info;
        }
    }
}
