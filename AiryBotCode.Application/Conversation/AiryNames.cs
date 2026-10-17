using System.Text.RegularExpressions;

namespace AiryBotCode.Application.Conversation
{
    public static class AiryNames
    {
        private static readonly Lazy<IReadOnlyList<string>> Nicknames = new(() =>
            Texts.Section("conversation", "nicknames").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        public static bool In(string? text, string botName)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            var names = Nicknames.Value.Append(botName).Where(n => !string.IsNullOrWhiteSpace(n)).Select(Regex.Escape);
            return Regex.IsMatch(text, $@"(?<![\w@#/-])({string.Join("|", names)})(?![\w-])", RegexOptions.IgnoreCase);
        }
    }
}
