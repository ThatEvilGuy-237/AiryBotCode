using System.Text.RegularExpressions;

namespace AiryBotCode.Application.Hive
{
    public sealed record ChatPicture(string Label, ForwardedImage Image);

    public sealed record StickerSeen(ulong Id, string Name, int Format);

    public static class ChatPictures
    {
        public const int Max = 4;
        private const int LottieFormat = 3;
        private const int GifFormat = 4;
        private static readonly Regex Custom = new(@"<a?:(\w{2,32}):(\d{16,20})>", RegexOptions.Compiled);

        public static string Readable(string content, IReadOnlyList<StickerSeen> stickers)
        {
            var text = string.IsNullOrEmpty(content) ? "" : Custom.Replace(content, m => $":{m.Groups[1].Value}:");
            var parts = new[] { text }.Concat(stickers.Select(s => $"[sticker {s.Name}]"));
            return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        public static IReadOnlyList<ChatPicture> Pick(string content, IReadOnlyList<StickerSeen> stickers)
        {
            var picked = new List<ChatPicture>();
            var seen = new HashSet<ulong>();
            foreach (Match m in Custom.Matches(content ?? ""))
            {
                if (!ulong.TryParse(m.Groups[2].Value, out var id) || !seen.Add(id)) continue;
                var name = m.Groups[1].Value;
                picked.Add(new ChatPicture($":{name}:",
                    new ForwardedImage($"https://cdn.discordapp.com/emojis/{id}.png?size=96", $"{name}.png", "image/png")));
            }
            foreach (var s in stickers)
            {
                if (s.Format == LottieFormat || !seen.Add(s.Id)) continue;
                var ext = s.Format == GifFormat ? "gif" : "png";
                picked.Add(new ChatPicture($"[sticker {s.Name}]",
                    new ForwardedImage($"https://media.discordapp.net/stickers/{s.Id}.{ext}?size=160", $"{s.Name}.{ext}", $"image/{ext}")));
            }
            return picked.Take(Max).ToList();
        }
    }
}
