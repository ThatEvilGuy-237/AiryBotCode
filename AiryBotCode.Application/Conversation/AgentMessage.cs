namespace AiryBotCode.Application.Conversation
{
    public static class AgentMessage
    {
        private const string File = "agent-message";

        public static string Reply(string bot, string channel, string author, string message, IReadOnlyList<LogLine> since,
            IReadOnlyList<string>? pictures = null, bool aboutYou = false)
        {
            var said = Texts.Fill(Texts.Section(File, since.Count == 0 ? "alone" : "with context"), new Dictionary<string, string>
            {
                ["bot"] = bot,
                ["channel"] = channel,
                ["author"] = author,
                ["lines"] = Lines(since),
                ["message"] = message,
            });
            return Finish(said, pictures, aboutYou);
        }

        public static string JoinIn(string bot, string channel, string author, IReadOnlyList<LogLine> stretch,
            IReadOnlyList<string>? pictures = null, bool aboutYou = false)
        {
            var said = Texts.Fill(Texts.Section(File, "joining in"), new Dictionary<string, string>
            {
                ["bot"] = bot,
                ["channel"] = channel,
                ["author"] = author,
                ["lines"] = Lines(stretch),
            });
            return Finish(said, pictures, aboutYou);
        }

        private static string Lines(IReadOnlyList<LogLine> lines) =>
            string.Join("\n", lines.Select(l => $"{l.Author}: {Texts.Flat(l.Text, ConversationSettings.Default.LineChars)}"));

        private static string Finish(string said, IReadOnlyList<string>? pictures, bool aboutYou)
        {
            if (aboutYou) said += "\n\n" + Texts.Section(File, "about you");
            if (pictures is { Count: > 0 })
                said += "\n\n" + Texts.Fill(Texts.Section(File, "pictures"), new Dictionary<string, string> { ["pictures"] = string.Join(", ", pictures) });
            return said;
        }
    }
}
