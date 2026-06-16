using Discord;

namespace AiryBotCode.Application.Frontend
{
    // Card for the await-mode ask_user prompt — the agent asks the user a question
    // mid-conversation and the option buttons sit under this embed. Restricted to the
    // user the agent asked (see DiscordAskDelivery / ButtonPressHandler).
    public static class AskFrontend
    {
        public static Embed AskEmbed(string question, string? askerUserId)
        {
            var eb = new EmbedBuilder()
                .WithColor(new Color(0x7C, 0x6A, 0xF7)) // periwinkle accent
                .WithTitle("🦊 Airy needs your input")
                .WithDescription(question)
                .WithFooter("Tap an option below")
                .WithCurrentTimestamp();

            if (ulong.TryParse(askerUserId, out var uid) && uid != 0)
                eb.AddField("For", $"<@{uid}>", true);

            return eb.Build();
        }
    }
}
