using System.Collections.Concurrent;

namespace AiryBotCode.Application.Hive
{
    public sealed record HeardLine(ulong MessageId, string Author, string Text, bool FromBot, bool Unheard = false)
    {
        public static HeardLine NotShared(ulong messageId) =>
            new(messageId, HiveTexts.Say("unheard author"), HiveTexts.Say("unheard text"), false, true);
    }

    public class ChannelTranscript
    {
        public const int Keep = 30;
        public const int MaxContextLines = 20;
        public const int MaxContextChars = 4000;

        private readonly ConcurrentDictionary<(ulong Bot, ulong Channel), List<HeardLine>> _channels = new();

        public void Hear(ulong botId, ulong channelId, HeardLine line)
        {
            if (string.IsNullOrWhiteSpace(line.Text)) return;
            var lines = _channels.GetOrAdd((botId, channelId), _ => []);
            lock (lines)
            {
                if (lines.Any(l => l.MessageId == line.MessageId)) return;
                if (line.Unheard && lines.Count > 0 && lines[^1].Unheard) return;
                lines.Add(line);
                if (lines.Count > Keep) lines.RemoveRange(0, lines.Count - Keep);
            }
        }

        public void Forget(ulong botId, ulong channelId) => _channels.TryRemove((botId, channelId), out _);

        public void HearIfKnown(ulong botId, ulong channelId, HeardLine line)
        {
            if (_channels.ContainsKey((botId, channelId))) Hear(botId, channelId, line);
        }

        public IReadOnlyList<HeardLine> Recent(ulong botId, ulong channelId, ulong beforeMessageId, int count)
        {
            if (!_channels.TryGetValue((botId, channelId), out var lines)) return [];
            lock (lines)
            {
                return lines.Where(l => l.MessageId != beforeMessageId).TakeLast(count).ToList();
            }
        }

        public IReadOnlyList<HeardLine> SinceBotSpoke(ulong botId, ulong channelId, ulong beforeMessageId)
        {
            if (!_channels.TryGetValue((botId, channelId), out var lines)) return [];
            List<HeardLine> since;
            lock (lines)
            {
                var others = lines.Where(l => l.MessageId != beforeMessageId).ToList();
                var lastBot = others.FindLastIndex(l => l.FromBot);
                since = others.Skip(lastBot + 1).TakeLast(MaxContextLines).ToList();
            }
            var total = 0;
            var kept = new List<HeardLine>();
            foreach (var line in Enumerable.Reverse(since))
            {
                total += line.Author.Length + line.Text.Length + 2;
                if (total > MaxContextChars) break;
                kept.Add(line);
            }
            kept.Reverse();
            return kept;
        }
    }

    public static class ChannelTalk
    {
        private static readonly Lazy<IReadOnlyDictionary<string, string>> Sections = new(() => WorthQuestion.ReadSections("channel-talk"));

        public static string Message(string bot, string channel, string author, string message, IReadOnlyList<HeardLine> since, IReadOnlyList<string>? pictures = null)
        {
            var lines = string.Join("\n", since.Select(l => $"{l.Author}: {Flat(l.Text)}"));
            var said = WorthQuestion.Fill(Sections.Value[since.Count == 0 ? "alone" : "with context"], new Dictionary<string, string>
            {
                ["bot"] = bot,
                ["channel"] = channel,
                ["author"] = author,
                ["lines"] = lines,
                ["message"] = message,
            });
            return WithPictures(said, pictures);
        }

        public static string JoinIn(string bot, string channel, string author, IReadOnlyList<HeardLine> stretch, IReadOnlyList<string>? pictures = null)
        {
            var said = WorthQuestion.Fill(Sections.Value["joining in"], new Dictionary<string, string>
            {
                ["bot"] = bot,
                ["channel"] = channel,
                ["author"] = author,
                ["lines"] = string.Join("\n", stretch.Select(l => $"{l.Author}: {Flat(l.Text)}")),
            });
            return WithPictures(said, pictures);
        }

        private static string WithPictures(string said, IReadOnlyList<string>? pictures)
        {
            if (pictures is not { Count: > 0 }) return said;
            return said + "\n\n" + WorthQuestion.Fill(Sections.Value["pictures"], new Dictionary<string, string>
            {
                ["pictures"] = string.Join(", ", pictures),
            });
        }

        private static string Flat(string text)
        {
            var flat = text.Replace('\n', ' ').Trim();
            return flat.Length <= WorthQuestion.MaxLine ? flat : flat[..WorthQuestion.MaxLine] + "…";
        }
    }
}
