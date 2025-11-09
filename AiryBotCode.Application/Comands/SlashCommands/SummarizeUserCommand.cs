using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using AiryBotCode.Application.Interfaces.Service; // Added for IConversationManagerService

namespace AiryBotCode.Application.Comands.SlashCommands
{
    public class SummarizeUserCommand : EvilCommand
    {
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
