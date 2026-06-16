using Discord;

namespace AiryBotCode.Application.Frontend
{
    // Card-style first-contact consent prompt. DM'd to the user so ONLY they see it
    // (a plain channel message can't be ephemeral — that's interaction-only). Richer
    // context than the old one-liner: what's stored, why, the guarantees, their control.
    public static class ConsentFrontend
    {
        public static Embed ConsentEmbed(ulong evilId)
        {
            var eb = new EmbedBuilder()
                .WithColor(new Color(0xF6, 0xB7, 0x3C)) // foxfire amber
                .WithTitle("👋 Before Airy replies — a quick heads-up")
                .WithDescription(
                    "This channel is connected to **Airy**, your AI companion. So Airy can remember "
                    + "your conversations and reply helpfully, the messages you send here are stored.")
                .AddField("📥 What's stored",
                    "The messages you send in this channel and Airy's replies, tied to your account.", false)
                .AddField("🎯 Why",
                    "Only to give **you** a more personal experience — memory, context, and continuity across chats.", false)
                .AddField("🚫 Never",
                    "Your data is **never** sold and **never** used to train AI models.", false)
                .AddField("✅ Your choice",
                    "Press **Accept** below to continue. You can ask the team to delete your data at any time.", false)
                .WithFooter("Airy · you only see this once")
                .WithCurrentTimestamp();

            if (evilId != 0)
                eb.AddField("❓ Questions", $"Ping <@{evilId}>.", false);

            return eb.Build();
        }
    }
}
