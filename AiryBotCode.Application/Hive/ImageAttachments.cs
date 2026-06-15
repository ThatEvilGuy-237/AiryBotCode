namespace AiryBotCode.Application.Hive
{
    /// <summary>An image attachment forwarded to the Hive flow alongside the text.</summary>
    public record ForwardedImage(string Url, string Name, string Mime);

    /// <summary>
    /// Picks the image attachments off a Discord message so they can be forwarded
    /// into the Hive flow (where the agent's vision/image intake reads them). Pure +
    /// Discord-free (takes plain tuples) so it's unit-testable.
    /// </summary>
    public static class ImageAttachments
    {
        public static IReadOnlyList<ForwardedImage> Pick(
            IEnumerable<(string? Url, string? Filename, string? ContentType)> attachments)
        {
            var images = new List<ForwardedImage>();
            foreach (var a in attachments ?? Array.Empty<(string?, string?, string?)>())
            {
                if (string.IsNullOrWhiteSpace(a.Url)) continue;
                var mime = !string.IsNullOrWhiteSpace(a.ContentType) ? a.ContentType! : GuessMime(a.Filename);
                if (mime is null || !mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) continue;
                images.Add(new ForwardedImage(a.Url!, string.IsNullOrWhiteSpace(a.Filename) ? "image" : a.Filename!, mime));
            }
            return images;
        }

        // Discord usually sets ContentType; fall back to the file extension when it doesn't.
        private static string? GuessMime(string? filename)
        {
            var ext = filename is null ? "" : System.IO.Path.GetExtension(filename).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => null,
            };
        }
    }
}
