using AiryBotCode.Application.Frontend;
using AiryBotCode.Application.Services.User;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

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
                .AddOption("user", ApplicationCommandOptionType.User, "Select the user to timeout", isRequired: true)
                .AddOption("duration", ApplicationCommandOptionType.Integer, "Timeout duration in minutes", isRequired: true)
                .AddOption("reason", ApplicationCommandOptionType.String, "Reason for the timeout", isRequired: true)
                .AddOption("clear", ApplicationCommandOptionType.Integer, "clear messages of the past hours", 
                isRequired: false, 
                choices: clearMessagesChoices);
        }
       

        public async Task TimeoutUser(SocketSlashCommand command, DiscordSocketClient client)
        {
            // Get user option
            var userOption = command.Data.Options.FirstOrDefault(o => o.Name == "user")?.Value as SocketGuildUser;
            var durationOption = command.Data.Options.FirstOrDefault(o => o.Name == "duration")?.Value;
            var reason = command.Data.Options.FirstOrDefault(o => o.Name == "reason")?.Value?.ToString();
            var clearMessagesOption = command.Data.Options.FirstOrDefault(o => o.Name == "clear_messages")?.Value;
            int clearMessagesTime = clearMessagesOption != null ? Convert.ToInt32(clearMessagesOption) : 0;

            // Validate options
            if (userOption == null || durationOption == null || reason == null || !(durationOption is int durationMinutes))
            {
                await command.RespondAsync("Something went wrong", ephemeral: true);
                return;
            }

            // Is admin?
            if (!await userService.UserIsAdmin(command))
            {
                await command.RespondAsync("Admin role ID not found in environment variables. CONTACT evil", ephemeral: true);
                return;
            }

            // Log action
            var logChannel = client.GetGuild(command.GuildId!.Value)?.GetTextChannel(1364679724269305967);
            if (logChannel != null)
            {

                TimeoutFrontend.RespondToCommand(command, logChannel, userOption, durationMinutes, reason);
            }
            // clear messages
            if (clearMessagesTime > 0)
            {
                await userService.ClearMessage(clearMessagesTime, userOption);
            }
            // Set timeout duration
            await userService.TimeOutUser(command, durationMinutes);
        }
    
    }
}
