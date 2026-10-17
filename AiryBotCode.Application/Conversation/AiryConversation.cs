using System.Collections.Concurrent;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Services;
using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Conversation
{
    public sealed class AiryConversation(
        IConversationStore store,
        ChannelLog log,
        ChannelWatch watch,
        IHiveDecider decider,
        IHiveAgent agent,
        ConversationSettings settings)
    {
        private sealed record Go(CheckStep Step, bool InTalk);

        private readonly ConcurrentDictionary<(ulong Bot, ulong Channel), SemaphoreSlim> _gates = new();

        public async Task<bool> OnMessageAsync(ChatMessage message, IChatChannel channel)
        {
            var row = await store.ChannelAsync(message.BotId, message.ChannelId);
            if (row is null) return false;
            var hive = await store.HiveAsync(message.BotId);
            if (hive is null)
            {
                Console.WriteLine($"[AiryChannel] bot {message.BotId} has no Hive connection; channel {message.ChannelId} stays quiet.");
                return true;
            }

            var member = await store.IsMemberAsync(message.BotId, message.AuthorId);
            Hear(row, message, member);

            switch (ConversationRules.OnMessage(row, message.Pinged, member, !string.IsNullOrWhiteSpace(message.Text)))
            {
                case MessageStep.AskConsent:
                    watch.Cancel(message.BotId, message.ChannelId);
                    await channel.AskConsentAsync();
                    break;
                case MessageStep.CheckLater:
                    var quick = ConversationRules.QuickCheck(watch.InTalk(message.BotId, message.ChannelId), AiryNames.In(message.Text, channel.BotName));
                    watch.CheckAfterPause(message.BotId, message.ChannelId, () => CheckAsync(message, channel), quick);
                    break;
                case MessageStep.AnswerNow:
                    if (message.Pinged) watch.Cancel(message.BotId, message.ChannelId);
                    await OneAtATime(message, () => ReplyAsync(message, channel, hive));
                    break;
            }
            return true;
        }

        public void OnOwnMessage(ChatMessage message)
        {
            watch.Spoke(message.BotId, message.ChannelId);
            log.HearOwn(message.BotId, message.ChannelId, message.Line());
        }

        private void Hear(AiryChannel row, ChatMessage message, bool member)
        {
            if (ConversationRules.Hears(row.Listens, member)) log.Hear(message.BotId, message.ChannelId, message.Line());
            else if (row.Listens) log.Hear(message.BotId, message.ChannelId, LogLine.Placeholder(message.Id));
            else log.Forget(message.BotId, message.ChannelId);
        }

        private async Task ReplyAsync(ChatMessage message, IChatChannel channel, HiveConnection hive)
        {
            if (log.IsRead(message.BotId, message.ChannelId, message.Id)) return;
            AgentReply reply;
            using (channel.Typing())
            {
                message = await channel.WithPreviewsAsync(message);
                var pictures = message.Pictures();
                var since = log.SinceAiry(message.BotId, message.ChannelId, message.Id);
                var aboutYou = since.Select(l => l.Text).Append(message.Text).Any(t => AiryNames.In(t, channel.BotName));
                var said = AgentMessage.Reply(channel.BotName, message.ChannelName, Writer(message.AuthorName, message.AuthorId),
                    message.Text, since, Labels(pictures), aboutYou);
                log.MarkRead(message.BotId, message.ChannelId, message.Id);
                watch.Spoke(message.BotId, message.ChannelId);
                reply = await RunAsync(hive, message, message.AuthorId, message.AuthorAccount, said, pictures);
            }
            await SendAsync(message, channel, reply.Text ?? (reply.Failed ? WebhookChatService.OfflineReply() : null));
        }

        private async Task CheckAsync(ChatMessage newest, IChatChannel channel)
        {
            var row = await store.ChannelAsync(newest.BotId, newest.ChannelId);
            if (row is null || !row.Enabled || row.AnswerMode != AnswerModes.Decide) return;
            var hive = await store.HiveAsync(newest.BotId);
            if (hive is null) return;
            var go = await DecideAsync(newest, channel, row, hive);
            if (go is not null) await OneAtATime(newest, () => AnswerCheckAsync(newest, channel, go));
        }

        private async Task<Go?> DecideAsync(ChatMessage newest, IChatChannel channel, AiryChannel row, HiveConnection hive)
        {
            var (bot, id) = (newest.BotId, newest.ChannelId);
            var unread = Unread(newest);
            var inTalk = watch.InTalk(bot, id);
            var step = ConversationRules.OnCheck(unread.Count > 0, unread.Any(l => AiryNames.In(l.Text, channel.BotName)), inTalk, watch.CoolingDown(bot, id));
            if (step == CheckStep.Skip) return null;
            if (step == CheckStep.AnswerNamed)
            {
                await store.RecordAsync(row.Id, Texts.Section("conversation", "named"), null);
                return new Go(step, inTalk);
            }

            var lines = Stretch(newest, unread, inTalk).Select(l => new CheckLine(l.FromAiry ? channel.BotName : l.Author, l.Text)).ToList();
            var verdict = await decider.AskAsync(hive, new CheckRequest(channel.BotName, newest.ChannelName, lines[^1].Author,
                AuthorOf(unread[^1], newest), lines[^1].Text, lines.Take(lines.Count - 1).ToList(), step == CheckStep.AskFollowUp));
            if (verdict.Error is not null) Console.WriteLine($"[AiryChannel] channel {id}: {verdict.Error}");
            await store.RecordAsync(row.Id, verdict.Summary, verdict.Error);
            return verdict.SaysAnswer ? new Go(step, inTalk) : null;
        }

        private async Task AnswerCheckAsync(ChatMessage newest, IChatChannel channel, Go go)
        {
            var (bot, id) = (newest.BotId, newest.ChannelId);
            var row = await store.ChannelAsync(bot, id);
            var hive = await store.HiveAsync(bot);
            if (row is null || !row.Enabled || row.AnswerMode != AnswerModes.Decide || hive is null) return;
            var unread = Unread(newest);
            if (unread.Count == 0) return;
            if (go.Step == CheckStep.AskJoinIn)
            {
                if (watch.CoolingDown(bot, id)) return;
                watch.Joined(bot, id);
            }

            var stretch = Stretch(newest, unread, go.InTalk);
            var last = unread[^1];
            var lastId = AuthorOf(last, newest);
            log.MarkRead(bot, id, unread.Max(l => l.MessageId));
            watch.Spoke(bot, id);
            AgentReply reply;
            using (channel.Typing())
            {
                newest = await channel.WithPreviewsAsync(newest);
                var pictures = newest.Pictures();
                var aboutYou = stretch.Any(l => !l.FromAiry && AiryNames.In(l.Text, channel.BotName));
                var said = go.InTalk
                    ? AgentMessage.Reply(channel.BotName, newest.ChannelName, Writer(last.Author, lastId),
                        last.MessageId == newest.Id ? newest.Text : last.Text, unread.Take(unread.Count - 1).ToList(), Labels(pictures), aboutYou)
                    : AgentMessage.JoinIn(channel.BotName, newest.ChannelName, Writer(last.Author, lastId), stretch, Labels(pictures), aboutYou);
                reply = await RunAsync(hive, newest, lastId, last.MessageId == newest.Id ? newest.AuthorAccount : last.Author, said, pictures);
            }
            await SendAsync(newest, channel, reply.Text);
        }

        private IReadOnlyList<LogLine> Unread(ChatMessage newest) =>
            log.Knows(newest.BotId, newest.ChannelId) ? log.Unread(newest.BotId, newest.ChannelId) : [newest.Line()];

        private IReadOnlyList<LogLine> Stretch(ChatMessage newest, IReadOnlyList<LogLine> unread, bool inTalk)
        {
            var (bot, id) = (newest.BotId, newest.ChannelId);
            var last = unread[^1];
            var unreadIds = unread.Select(l => l.MessageId).ToHashSet();
            var lines = inTalk
                ? log.Recent(bot, id, settings.CheckLines * 2)
                    .Where(l => !unreadIds.Contains(l.MessageId) && (l.FromAiry || l.MessageId < last.MessageId))
                    .Concat(unread)
                    .TakeLast(settings.CheckLines)
                    .ToList()
                : log.SinceAiry(bot, id).Where(l => l.MessageId <= last.MessageId).ToList();
            return lines.Count == 0 ? unread : lines;
        }

        private Task<AgentReply> RunAsync(HiveConnection hive, ChatMessage message, ulong authorId, string author, string said,
            IReadOnlyList<ChatPicture> pictures) =>
            agent.RunAsync(hive.RunUrl, hive.AppKey, message.BotId, message.ChannelId, authorId, author, said,
                message.Attachments.Concat(pictures.Select(p => p.Image)).ToList());

        private async Task SendAsync(ChatMessage message, IChatChannel channel, string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            foreach (var sent in await channel.SendAsync(text))
                OnOwnMessage(message with { Id = sent.Id, AuthorId = message.BotId, AuthorName = channel.BotName, Text = sent.Text });
        }

        private async Task OneAtATime(ChatMessage message, Func<Task> work)
        {
            var gate = _gates.GetOrAdd((message.BotId, message.ChannelId), _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync();
            try
            {
                await work();
            }
            finally
            {
                gate.Release();
            }
        }

        private static ulong AuthorOf(LogLine line, ChatMessage newest) => line.AuthorId == 0 ? newest.AuthorId : line.AuthorId;

        private static string Writer(string name, ulong id) => $"{name} ({id})";

        private static IReadOnlyList<string> Labels(IReadOnlyList<ChatPicture> pictures) => pictures.Select(p => p.Label).ToList();
    }
}
