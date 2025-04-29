using AiryBotCode.Application.Services.User;
using AiryBotCode.Domain.Entities;
using AiryBotCode.Tool.Frontend;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Comands
{
    public class TimeoutCommand : EvilCommand
    {
        protected readonly UserService userService;

        public TimeoutCommand(IServiceProvider serviceProvider)
            : base()
        {
            Name = "timeout";
            userService = serviceProvider.GetRequiredService<UserService>();
        }

        public override SlashCommandBuilder GetCommand()
        {
            var clearMessagesChoices = new[]
            {
                new ApplicationCommandOptionChoiceProperties { Name = "1 hour ago", Value = 60 },
                new ApplicationCommandOptionChoiceProperties { Name = "5 hours", Value = 5 * 60 },
                new ApplicationCommandOptionChoiceProperties { Name = "10 hours", Value = 10 * 60 },
                new ApplicationCommandOptionChoiceProperties { Name = "24 hours", Value = 24 * 60 }
            };

            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Timeout a user and log the action for admins")
                .AddOption("target", ApplicationCommandOptionType.User, "Select the user to timeout", isRequired: true)
                .AddOption("duration", ApplicationCommandOptionType.Integer, "Timeout duration in minutes", isRequired: true)
                .AddOption("reason", ApplicationCommandOptionType.String, "Reason for the timeout", isRequired: true)
                .AddOption("clear", ApplicationCommandOptionType.Integer, "Clear messages from past hours", isRequired: false, choices: clearMessagesChoices);
        }

        public async Task<TimeoutInfo> TimeoutUser(SocketSlashCommand command, DiscordSocketClient client)
        {
            TimeoutInfo info = new TimeoutInfo(
                command.Data.Options.FirstOrDefault(o => o.Name == "target")?.Value as SocketGuildUser,
                command.Data.Options.FirstOrDefault(o => o.Name == "duration")?.Value,
                command.Data.Options.FirstOrDefault(o => o.Name == "reason")?.Value?.ToString(),
                command.Data.Options.FirstOrDefault(o => o.Name == "clear") is { Value: var val } ? Convert.ToInt32(val) : 0
            );

            // Validate inputs
            if (info.Target == null || info.DurationOption == null || info.Reason == null ||
                !int.TryParse(info.DurationOption.ToString(), out int durationMinutes))
            {
                await command.RespondAsync("Invalid parameters provided.", ephemeral: true);
                return info;
            }

            // Check permissions
            if (!await userService.UserIsAdmin(command))
            {
                await command.RespondAsync("Admin role not found or insufficient permissions.", ephemeral: true);
                return info;
            }
            // responde before calculations
            await command.RespondAsync($"{info.Target.Mention} has been timed out for {durationMinutes} minutes. Reason: {info.Reason}", ephemeral: true);

            // Apply timeout
            await userService.TimeOutUser(command, info.Target, durationMinutes);
            // Clear messages if needed
            if (info.ClearMessagesTime > 0)
            {
                await userService.ClearMessage(info.ClearMessagesTime, info.Target);
            }

            return info;
        }
    }
}
