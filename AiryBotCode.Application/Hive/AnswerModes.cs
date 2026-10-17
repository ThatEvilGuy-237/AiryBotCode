namespace AiryBotCode.Application.Hive
{
    public static class AnswerModes
    {
        public const string Always = "always";
        public const string Mention = "mention";
        public const string Decide = "decide";

        public static readonly IReadOnlyList<string> All = [Always, Mention, Decide];

        public static bool IsValid(string? mode) => mode is not null && All.Contains(mode);
    }
}
