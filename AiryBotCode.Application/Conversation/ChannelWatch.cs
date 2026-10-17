using System.Collections.Concurrent;

namespace AiryBotCode.Application.Conversation
{
    public class ChannelWatch
    {
        private sealed class Stretch
        {
            public int Count;
            public DateTimeOffset First;
            public CancellationTokenSource? Timer;
            public Func<Task>? Check;
            public DateTimeOffset? Joined;
            public DateTimeOffset? Spoke;
        }

        private readonly ConcurrentDictionary<(ulong Bot, ulong Channel), Stretch> _channels = new();
        private readonly ConversationSettings _settings;
        private readonly TimeProvider _clock;

        public ChannelWatch() : this(ConversationSettings.Default, TimeProvider.System)
        {
        }

        public ChannelWatch(ConversationSettings settings, TimeProvider clock)
        {
            _settings = settings;
            _clock = clock;
        }

        public void CheckAfterPause(ulong botId, ulong channelId, Func<Task> check, bool quick = false)
        {
            var stretch = _channels.GetOrAdd((botId, channelId), _ => new Stretch());
            CancellationTokenSource timer;
            TimeSpan due;
            lock (stretch)
            {
                var now = _clock.GetUtcNow();
                if (stretch.Count == 0) stretch.First = now;
                stretch.Count++;
                stretch.Check = check;
                stretch.Timer?.Cancel();
                var quiet = quick ? _settings.QuickQuiet : _settings.Quiet;
                var left = _settings.MaxWait - (now - stretch.First);
                due = stretch.Count >= _settings.MaxMessages ? TimeSpan.Zero
                    : left < quiet ? (left < TimeSpan.Zero ? TimeSpan.Zero : left)
                    : quiet;
                timer = new CancellationTokenSource();
                stretch.Timer = timer;
            }
            _ = FireAsync(stretch, timer, due);
        }

        public void Cancel(ulong botId, ulong channelId)
        {
            if (!_channels.TryGetValue((botId, channelId), out var stretch)) return;
            lock (stretch)
            {
                stretch.Timer?.Cancel();
                stretch.Timer = null;
                stretch.Check = null;
                stretch.Count = 0;
            }
        }

        public bool CoolingDown(ulong botId, ulong channelId)
        {
            if (!_channels.TryGetValue((botId, channelId), out var stretch)) return false;
            lock (stretch)
            {
                return stretch.Joined is { } at && _clock.GetUtcNow() - at < _settings.Cooldown;
            }
        }

        public void Spoke(ulong botId, ulong channelId)
        {
            var stretch = _channels.GetOrAdd((botId, channelId), _ => new Stretch());
            lock (stretch)
            {
                stretch.Spoke = _clock.GetUtcNow();
            }
        }

        public bool InTalk(ulong botId, ulong channelId)
        {
            if (!_channels.TryGetValue((botId, channelId), out var stretch)) return false;
            lock (stretch)
            {
                return stretch.Spoke is { } at && _clock.GetUtcNow() - at < _settings.InTalkFor;
            }
        }

        public void Joined(ulong botId, ulong channelId)
        {
            var stretch = _channels.GetOrAdd((botId, channelId), _ => new Stretch());
            lock (stretch)
            {
                stretch.Joined = _clock.GetUtcNow();
            }
        }

        private async Task FireAsync(Stretch stretch, CancellationTokenSource timer, TimeSpan due)
        {
            try
            {
                if (due > TimeSpan.Zero) await Task.Delay(due, _clock, timer.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            Func<Task>? check;
            lock (stretch)
            {
                if (stretch.Timer != timer) return;
                check = stretch.Check;
                stretch.Timer = null;
                stretch.Check = null;
                stretch.Count = 0;
            }
            if (check is null) return;
            try
            {
                await check();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiryChannel] joining in failed: {ex.Message}");
            }
        }
    }
}
