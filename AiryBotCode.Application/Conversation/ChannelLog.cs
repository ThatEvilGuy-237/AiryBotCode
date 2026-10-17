using System.Collections.Concurrent;

namespace AiryBotCode.Application.Conversation
{
    public sealed record LogLine(ulong MessageId, string Author, string Text, bool FromAiry = false, bool NotShared = false, ulong AuthorId = 0)
    {
        public static LogLine Placeholder(ulong messageId) =>
            new(messageId, Texts.Section("conversation", "not shared author"), Texts.Section("conversation", "not shared text"), NotShared: true);
    }

    public class ChannelLog
    {
        private sealed class Channel
        {
            public readonly List<LogLine> Lines = [];
            public ulong ReadUpTo;
        }

        private readonly ConcurrentDictionary<(ulong Bot, ulong Channel), Channel> _channels = new();
        private readonly ConversationSettings _settings;

        public ChannelLog() : this(ConversationSettings.Default)
        {
        }

        public ChannelLog(ConversationSettings settings)
        {
            _settings = settings;
        }

        public bool Knows(ulong botId, ulong channelId) => _channels.ContainsKey((botId, channelId));

        public void Forget(ulong botId, ulong channelId) => _channels.TryRemove((botId, channelId), out _);

        public void Hear(ulong botId, ulong channelId, LogLine line)
        {
            if (string.IsNullOrWhiteSpace(line.Text)) return;
            var channel = _channels.GetOrAdd((botId, channelId), _ => new Channel());
            lock (channel)
            {
                var lines = channel.Lines;
                if (lines.Any(l => l.MessageId == line.MessageId)) return;
                if (line.NotShared && lines.Count > 0 && lines[^1].NotShared) return;
                lines.Add(line);
                if (lines.Count > _settings.LogLines) lines.RemoveRange(0, lines.Count - _settings.LogLines);
            }
        }

        public void HearOwn(ulong botId, ulong channelId, LogLine line)
        {
            if (Knows(botId, channelId)) Hear(botId, channelId, line with { FromAiry = true });
        }

        public void MarkRead(ulong botId, ulong channelId, ulong messageId)
        {
            if (!_channels.TryGetValue((botId, channelId), out var channel)) return;
            lock (channel)
            {
                channel.ReadUpTo = Math.Max(channel.ReadUpTo, messageId);
            }
        }

        public bool IsRead(ulong botId, ulong channelId, ulong messageId)
        {
            if (!_channels.TryGetValue((botId, channelId), out var channel)) return false;
            lock (channel)
            {
                return messageId <= channel.ReadUpTo;
            }
        }

        public IReadOnlyList<LogLine> Unread(ulong botId, ulong channelId) =>
            With(botId, channelId, c => c.Lines.Where(l => !l.FromAiry && !l.NotShared && l.MessageId > c.ReadUpTo).ToList());

        public IReadOnlyList<LogLine> Recent(ulong botId, ulong channelId, int count) =>
            With(botId, channelId, c => c.Lines.TakeLast(count).ToList());

        public IReadOnlyList<LogLine> SinceAiry(ulong botId, ulong channelId, ulong exceptMessageId = 0)
        {
            var since = With(botId, channelId, c =>
            {
                var others = c.Lines.Where(l => l.MessageId != exceptMessageId).ToList();
                return others.Skip(others.FindLastIndex(l => l.FromAiry) + 1).TakeLast(_settings.ContextLines).ToList();
            });
            var total = 0;
            var kept = new List<LogLine>();
            foreach (var line in since.Reverse())
            {
                total += line.Author.Length + line.Text.Length + 2;
                if (total > _settings.ContextChars) break;
                kept.Add(line);
            }
            kept.Reverse();
            return kept;
        }

        private IReadOnlyList<LogLine> With(ulong botId, ulong channelId, Func<Channel, IReadOnlyList<LogLine>> read)
        {
            if (!_channels.TryGetValue((botId, channelId), out var channel)) return [];
            lock (channel)
            {
                return read(channel);
            }
        }
    }
}
