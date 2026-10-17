namespace AiryBotCode.Application.Hive
{
    public sealed record AgentLink(int Id, ulong ChannelId, string RunUrl, string? Key, bool Enabled);

    public sealed record ChannelPlan(ulong ChannelId, string Mode, bool Enabled);

    public sealed record LinkMove(HiveConnection? Connection, IReadOnlyList<ChannelPlan> Channels, IReadOnlyList<int> RemoveLinks)
    {
        public bool IsEmpty => Connection is null && Channels.Count == 0 && RemoveLinks.Count == 0;
    }

    public static class AgentLinkMove
    {
        public static LinkMove Plan(HiveConnection? current, IReadOnlyCollection<ulong> existingChannels, IReadOnlyList<AgentLink> links, bool requireMention)
        {
            var target = current ?? links
                .OrderByDescending(l => l.Enabled)
                .ThenBy(l => l.Id)
                .Select(l => HiveConnection.FromRunUrl(l.RunUrl, l.Key))
                .FirstOrDefault(c => c is not null);
            if (target is null) return new LinkMove(null, [], []);

            var channels = new List<ChannelPlan>();
            var removed = new List<int>();
            var taken = new HashSet<ulong>(existingChannels);
            foreach (var link in links.OrderBy(l => l.Id))
            {
                if (HiveConnection.FromRunUrl(link.RunUrl, link.Key) != target) continue;
                removed.Add(link.Id);
                if (taken.Add(link.ChannelId))
                    channels.Add(new ChannelPlan(link.ChannelId, requireMention ? AnswerModes.Mention : AnswerModes.Always, link.Enabled));
            }
            return removed.Count == 0
                ? new LinkMove(null, [], [])
                : new LinkMove(current is null ? target : null, channels, removed);
        }
    }
}
