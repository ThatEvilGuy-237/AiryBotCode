namespace AiryBotCode.Application.Conversation
{
    public sealed record ConversationSettings
    {
        public static ConversationSettings Default { get; } = new();

        public int LogLines { get; init; } = 30;
        public int ContextLines { get; init; } = 20;
        public int ContextChars { get; init; } = 4000;
        public int CheckLines { get; init; } = 12;
        public int LineChars { get; init; } = 300;
        public int MessageChars { get; init; } = 2000;
        public int MaxPictures { get; init; } = 4;

        public TimeSpan Quiet { get; init; } = TimeSpan.FromSeconds(20);
        public TimeSpan QuickQuiet { get; init; } = TimeSpan.FromSeconds(6);
        public int MaxMessages { get; init; } = 8;
        public TimeSpan MaxWait { get; init; } = TimeSpan.FromSeconds(60);
        public TimeSpan Cooldown { get; init; } = TimeSpan.FromMinutes(10);
        public TimeSpan InTalkFor { get; init; } = TimeSpan.FromMinutes(5);
        public TimeSpan PreviewWait { get; init; } = TimeSpan.FromSeconds(1.5);
    }
}
