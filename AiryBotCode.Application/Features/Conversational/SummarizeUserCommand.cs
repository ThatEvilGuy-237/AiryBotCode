using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Domain.Configuration.Attributes;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Application.Features.Conversational
{
    [ConfigurableCommand("SummarizeUserCommand")]
    public class SummarizeUserCommand : EvilCommand
    {
        // --- Settings Declaration for Seeder ---
        [ReloadableSetting("The primary description for the command.")]
        public string Description { get; set; } = "Generates an AI summary of a user's chat history.";

        [LiveSetting("Acknowledgement sent while the summary is generated. {0}=User", Category = "AI")]
        public string GeneratingMessage { get; set; } = "Generating summary for {0}...";

        [LiveSetting("Message shown when no summary could be generated. {0}=User", Category = "AI")]
        public string NoSummaryMessage { get; set; } = "Could not generate a summary for {0}.";
        // --- End of Settings Declaration ---

        private readonly IConversationManagerService _conversationManagerService;

        public SummarizeUserCommand(IServiceProvider serviceProvider, IConversationManagerService conversationManagerService) : base(serviceProvider)
        {
            Name = "summarize-user";
            _conversationManagerService = conversationManagerService;
        }

        public override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Generates an AI summary of a user's chat history.")
                .AddOption("target", ApplicationCommandOptionType.User, "The user to summarize", true);
        }

        public async Task<bool> HandleSlashCommand(SocketSlashCommand command)
        {
            var targetUser = command.Data.Options.FirstOrDefault(o => o.Name == "target")?.Value as SocketGuildUser;

            if (targetUser == null)
            {
                await command.RespondAsync("Invalid target user.", ephemeral: true);
                return false;
            }

            await command.RespondAsync($"Generating summary for {targetUser.Mention}...", ephemeral: true);

            // Call the service to generate and save the summary
            string summary = await _conversationManagerService.GenerateAndSaveUserSummaryAsync(targetUser.Id, command.GuildId ?? 0); // GuildId might be null in DMs

            if (string.IsNullOrEmpty(summary))
            {
                await command.FollowupAsync($"Could not generate a summary for {targetUser.Mention}.", ephemeral: true);
            }
            else
            {
                await command.FollowupAsync($"Here is a summary for {targetUser.Mention}:\n\n> {summary}", ephemeral: true);
            }

            return true;
        }
    }
}
