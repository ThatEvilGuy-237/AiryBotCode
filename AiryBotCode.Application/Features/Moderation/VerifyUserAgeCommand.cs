using AiryBotCode.Application.Frontend;
using AiryBotCode.Application.Services.User;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


namespace AiryBotCode.Application.Features.Moderation
{
    public class VerifyUserAgeCommand : EvilCommand
    {
        public const string ActionEdit = "edit";
        protected UserService _userService;

        public VerifyUserAgeCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "verif";
            _userService = serviceProvider.GetRequiredService<UserService>();
        }

        public override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Age Verify a User")
                .AddOption("target", ApplicationCommandOptionType.User, "Select user to Age Verify", isRequired: true);
        }

        public async Task<bool> AgeVerifyUser(SocketSlashCommand command)
        {
            try
            {
                ulong verifiedRoleId = 1283099014476075151;
                ulong unverifiedRoleId = 1283101142255144991;
                ulong logChannelId = 1283102267129724958;

                // The staff/admin who ran the command
                var executedBy = command.User as SocketGuildUser;

                //if (!await _userService.UserIsAdmin(command))
                //{
                //    throw new Exception("not valdig input");
                //}
                // The target user passed as option
                var targetOption = command.Data.Options.FirstOrDefault(opt => opt.Name == "target")?.Value as SocketGuildUser;
                if (executedBy == null || targetOption == null)
                {
                    await command.RespondAsync(embed: VerifyFrontend.VerificationErrorEmbed("Invalid user or target."));
                    return false;
                }

                var verifiedRole = targetOption.Guild.GetRole(verifiedRoleId);
                var unverifiedRole = targetOption.Guild.GetRole(unverifiedRoleId);

                // Add verified role
                if (verifiedRole != null && !targetOption.Roles.Contains(verifiedRole))
                    await targetOption.AddRoleAsync(verifiedRole);

                // Remove unverified role
                if (unverifiedRole != null && targetOption.Roles.Contains(unverifiedRole))
                    await targetOption.RemoveRoleAsync(unverifiedRole);

                // Reply to command (let executor know)
                await command.RespondAsync(embed: VerifyFrontend.VerifiedUserEmbed(targetOption, verifiedRole));

                // Log action
                var logChannel = targetOption.Guild.GetTextChannel(logChannelId);
                if (logChannel != null)
                    await logChannel.SendMessageAsync(
                        embed: VerifyFrontend.VerificationLogEmbed(targetOption, executedBy, verifiedRole, unverifiedRole));

                return true;
            }
            catch (Exception ex)
            {
                await command.RespondAsync(embed: VerifyFrontend.VerificationErrorEmbed());
                Console.WriteLine($"Error in AgeVerifyUser: {ex}");
                return false;
            }
        }

    }
}
