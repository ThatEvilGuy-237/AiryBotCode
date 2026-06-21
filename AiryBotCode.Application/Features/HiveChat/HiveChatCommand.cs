using AiryBotCode.Application.Features;
using AiryBotCode.Domain.Configuration.Attributes;
using Discord;

namespace AiryBotCode.Application.Features.HiveChat
{
    /// <summary>
    /// Per-bot settings for the channel→Hive AI chat. This is NOT a slash command —
    /// it only carries configuration that the webhook-chat message handler reads
    /// (by BotId) to decide whether/when to engage the Hive.
    /// </summary>
    [ConfigurableCommand("HiveChatCommand")]
    public class HiveChatCommand : EvilCommand
    {
        [ReloadableSetting(
            "Only respond when the bot is @mentioned or replied-to (a reply to the bot counts as a ping). " +
            "Off = respond to every message in linked channels.",
            Category = "HiveChat", UiHint = "boolean")]
        public bool RequireMention { get; set; } = false;

        public HiveChatCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "hivechat";
        }

        // Message-driven only: never registered as a slash command.
        public override SlashCommandBuilder GetCommand() =>
            throw new NotSupportedException("HiveChatCommand is message-driven and has no slash command.");
    }
}
