using AiryBotCode.Application.Hive;

namespace AiryBotCode.Application.Conversation
{
    public sealed record ChatMessage(
        ulong Id,
        ulong BotId,
        ulong ChannelId,
        string ChannelName,
        ulong AuthorId,
        string AuthorName,
        string AuthorAccount,
        string Text,
        string RawText,
        bool Pinged,
        IReadOnlyList<ForwardedImage> Attachments,
        IReadOnlyList<StickerSeen> Stickers,
        IReadOnlyList<EmbedSeen> Embeds)
    {
        public bool WaitsForPreviews => Embeds.Count == 0 && ChatPictures.HasLink(RawText);

        public IReadOnlyList<ChatPicture> Pictures() => ChatPictures.Pick(RawText, Stickers, Embeds);

        public LogLine Line() => new(Id, AuthorName, Text, AuthorId: AuthorId);
    }

    public sealed record SentMessage(ulong Id, string Text);

    public interface IChatChannel
    {
        string BotName { get; }

        IDisposable Typing();

        Task<IReadOnlyList<SentMessage>> SendAsync(string text);

        Task AskConsentAsync();

        Task<ChatMessage> WithPreviewsAsync(ChatMessage message);
    }
}
