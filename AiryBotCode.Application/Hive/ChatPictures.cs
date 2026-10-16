using System.Text.RegularExpressions;

namespace AiryBotCode.Application.Hive
{
    public sealed record ChatPicture(string Label, ForwardedImage Image);

    public sealed record StickerSeen(ulong Id, string Name, int Format);

    public sealed record EmbedSeen(string Type, string? Url, string? Picture);

    public static class ChatPictures
    {
        public const int Max = 4;
        private const int LottieFormat = 3;
        private const int GifFormat = 4;
        private static readonly Regex Custom = new(@"<a?:(\w{2,32}):(\d{16,20})>", RegexOptions.Compiled);
        private static readonly Regex Link = new(@"https?://\S+", RegexOptions.Compiled);
        private static readonly Regex TenorSlug = new(@"^https?://(?:www\.)?tenor\.com/(?:[\w-]+/)?view/([\w-]+?)-gif-\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex GiphySlug = new(@"^https?://(?:www\.)?giphy\.com/gifs/([\w-]+)-\w+/?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex GifHost = new(@"^https?://(?:[\w-]+\.)*(?:tenor\.com|giphy\.com)/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string Readable(string content, IReadOnlyList<StickerSeen> stickers, IReadOnlyList<EmbedSeen>? embeds = null)
        {
            var text = string.IsNullOrEmpty(content) ? "" : Custom.Replace(content, m => $":{m.Groups[1].Value}:");
            text = Link.Replace(text, m => LinkLabel(m.Value, embeds ?? []) ?? m.Value);
            var parts = new[] { text }.Concat(stickers.Select(s => $"[sticker {s.Name}]"));
            return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        public static bool HasLink(string content) => !string.IsNullOrEmpty(content) && Link.IsMatch(content);

        public static IReadOnlyList<ChatPicture> Pick(string content, IReadOnlyList<StickerSeen> stickers, IReadOnlyList<EmbedSeen>? embeds = null)
        {
            var picked = new List<ChatPicture>();
            var urls = new HashSet<string>();
            foreach (Match m in Link.Matches(content ?? ""))
            {
                var embed = EmbedFor(m.Value, embeds ?? []);
                if (embed?.Picture is null || !urls.Add(embed.Picture)) continue;
                picked.Add(new ChatPicture(LinkLabel(m.Value, embeds ?? [])!, new ForwardedImage(embed.Picture, FileOf(embed.Picture), MimeOf(embed.Picture))));
            }
            var seen = new HashSet<ulong>();
            foreach (var s in stickers)
            {
                if (s.Format == LottieFormat || !seen.Add(s.Id)) continue;
                var ext = s.Format == GifFormat ? "gif" : "png";
                picked.Add(new ChatPicture($"[sticker {s.Name}]",
                    new ForwardedImage($"https://media.discordapp.net/stickers/{s.Id}.{ext}?size=160", $"{s.Name}.{ext}", $"image/{ext}")));
            }
            foreach (Match m in Custom.Matches(content ?? ""))
            {
                if (!ulong.TryParse(m.Groups[2].Value, out var id) || !seen.Add(id)) continue;
                var name = m.Groups[1].Value;
                picked.Add(new ChatPicture($":{name}:",
                    new ForwardedImage($"https://cdn.discordapp.com/emojis/{id}.png?size=96", $"{name}.png", "image/png")));
            }
            return picked.Take(Max).ToList();
        }

        private static string? LinkLabel(string url, IReadOnlyList<EmbedSeen> embeds)
        {
            var embed = EmbedFor(url, embeds);
            var gif = IsGif(url, embed);
            if (!gif && embed?.Type != "Image") return null;
            if (!gif) return "[image]";
            var words = Words(url);
            return words is null ? "[gif]" : $"[gif: {words}]";
        }

        private static EmbedSeen? EmbedFor(string url, IReadOnlyList<EmbedSeen> embeds) =>
            embeds.FirstOrDefault(e => e.Url is not null && Same(e.Url, url) && (e.Type is "Gifv" or "Image"));

        private static bool IsGif(string url, EmbedSeen? embed) =>
            embed?.Type == "Gifv" || GifHost.IsMatch(url) || PathOf(url).EndsWith(".gif", StringComparison.OrdinalIgnoreCase);

        private static string? Words(string url)
        {
            var m = TenorSlug.Match(url);
            if (!m.Success) m = GiphySlug.Match(url);
            return m.Success ? m.Groups[1].Value.Replace('-', ' ') : null;
        }

        private static bool Same(string a, string b) => string.Equals(a.TrimEnd('/'), b.TrimEnd('/'), StringComparison.OrdinalIgnoreCase);

        private static string PathOf(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? u.AbsolutePath : url;

        private static string FileOf(string url)
        {
            var name = Path.GetFileName(PathOf(url));
            return string.IsNullOrWhiteSpace(name) ? "gif" : name;
        }

        private static string MimeOf(string url) => Path.GetExtension(PathOf(url)).ToLowerInvariant() switch
        {
            ".gif" => "image/gif",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp",
            _ => "image/png",
        };
    }
}
