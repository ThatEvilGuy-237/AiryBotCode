using AiryBotCode.Application.Services.User;
using AiryBotCode.Domain.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Features.Moderation
{
    public class UntimeoutCommand : EvilCommand
    {
        protected readonly UserService userService;

        public UntimeoutCommand(IServiceProvider serviceProvider)
            :base(serviceProvider)
        {
            userService = serviceProvider.GetRequiredService<UserService>();
            Name = "untimeout";
        }

        public override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Remove a user's timeout")
                .AddOption("target", ApplicationCommandOptionType.User, "Select the user to untimeout", isRequired: true);
        }

        public async Task<UntimeoutInfo> UntimeoutUser(SocketSlashCommand command)
        {
            // Get user option
            UntimeoutInfo info = new UntimeoutInfo()
            {
                Target = command.Data.Options.FirstOrDefault(o => o.Name == "target")?.Value as SocketGuildUser
            };

            // Validate options
            if (info.Target == null)
            {
                await command.RespondAsync("Something went wrong", ephemeral: true);
                return info;
            }

            // Check if user is currently timed out
            if (info.Target.TimedOutUntil == null || info.Target.TimedOutUntil <= DateTimeOffset.UtcNow)
            {
                await command.RespondAsync($"{info.Target.Mention} is not currently timed out.", ephemeral: true);
                return info;
            }

            // Check permissions
            //if (!await userService.UserIsAdmin(command))
            //{
            //    return info;
            //}

            // Remove timeout
            await userService.UntimeOut(command, info.Target);
            await command.RespondAsync(
                $"✅ Timeout removed from **{info.Target.Mention}**.\n" +
                $"They were previously timed out until: `{info.Target.TimedOutUntil?.ToString("f") ?? "unknown"}`\n" +
                $"⏱ remaining Timeout was: **{(info.Target.TimedOutUntil.HasValue ? (info.Target.TimedOutUntil.Value - DateTimeOffset.UtcNow).ToString("hh\\:mm\\:ss") : "N/A")}**",
                ephemeral: true
            );

            return info;
        }
    }
}
