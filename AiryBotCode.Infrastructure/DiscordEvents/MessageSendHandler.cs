using System.Text;
using AiryBotCode.Application.Consent;
using AiryBotCode.Application.Frontend;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Services;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Interfaces;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class MessageSendHandler : EvilEventHandler
    {
        private List<EvilAction> _messageAction;
        public void AssignActions(List<EvilAction> events)
        {
            _messageAction = events.OfType<IMessageAction>().Cast<EvilAction>().ToList();
            Console.WriteLine("MessageSendHandler");
        }
        public MessageSendHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // TODO: this should be reworked to handel diffrent type of messages check that we want.
        public async Task HandelMessageSend(SocketMessage message)
        {
            if (message.Author.IsBot)
            {
                HearOwnLine(message);
                return;
            }

            // Channel → webhook chat: if this channel is linked, forward the message
            // to its webhook and relay any reply back.
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var botId = scope.ServiceProvider.GetRequiredService<IConfigurationReader>().GetBotId();
                var forwarder = scope.ServiceProvider.GetRequiredService<WebhookChatService>();

                // Resolve raw mentions (<@id> / bare @id) to "DisplayName (id)" so the
                // flow + memory see a name, not a snowflake. Prefer the users Discord
                // already resolved on the message; fall back to a guild lookup.
                var content = Readable(message, botId);

                var mentionsBot = message.MentionedUsers.Any(u => u.Id == botId);
                var repliesToBot = message.Reference != null
                    && (message as SocketUserMessage)?.ReferencedMessage?.Author?.Id == botId;
                if (await AnswerInAiryChannelAsync(_serviceProvider, scope.ServiceProvider, message, botId, content, mentionsBot || repliesToBot))
                {
                    foreach (var Event in _messageAction)
                        if (Event is IMessageAction handled) await handled.HandleMessageReceivedAsync(message);
                    return;
                }

                var link = await scope.ServiceProvider.GetRequiredService<IChannelWebhookRepository>()
                    .GetForChannelAsync(botId, message.Channel.Id);
                if (link != null)
                {
                    var requireMention = false;
                    try
                    {
                        var hiveSettings = await scope.ServiceProvider.GetRequiredService<ICommandSettingsRepository>()
                            .GetByCommandAsync(botId, "HiveChatCommand");
                        var row = hiveSettings.FirstOrDefault(s => s.Key == "RequireMention");
                        requireMention = row != null && bool.TryParse(row.Value, out var rm) && rm;
                    }
                    catch (Exception sx) { Console.WriteLine($"[HiveChat] setting read failed, allowing through: {sx.Message}"); }

                    if (requireMention && !mentionsBot && !repliesToBot)
                        return;

                    if (!await HasConsentAsync(scope.ServiceProvider, botId, message.Author.Id))
                    {
                        await SendConsentAsync(scope.ServiceProvider, message, botId);
                        return;
                    }
                }

                // Forward any image attachments so the agent's vision intake can read them.
                var images = ImageAttachments.Pick(
                    message.Attachments.Select(a => ((string?)a.Url, (string?)a.Filename, (string?)a.ContentType)));

                var reply = link == null ? null : await forwarder.TryForwardAsync(
                    botId, message.Channel.Id, message.Author.Id, message.Author.Username, content,
                    images, () => message.Channel.EnterTypingState());
                await SendReplyAsync(scope.ServiceProvider, message, reply);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Webhook] message forward error: {ex.Message}");
            }

            foreach (var Event in _messageAction)
            {
                if (Event is IMessageAction messageEvent)
                {
                    await messageEvent.HandleMessageReceivedAsync(message);
                }
            }
        }

        private static async Task<bool> AnswerInAiryChannelAsync(IServiceProvider root, IServiceProvider services, SocketMessage message, ulong botId, string content, bool targeted)
        {
            var channels = services.GetRequiredService<IAiryChannelRepository>();
            AiryChannel? channel;
            try
            {
                channel = await channels.GetForChannelAsync(botId, message.Channel.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiryChannel] could not read the channels: {ex.Message}");
                return false;
            }
            if (channel is null) return false;

            var bot = await services.GetRequiredService<IBotSettingRepository>().GetBotSettingAsync(botId);
            var hive = bot is null ? null : HiveConnection.Of(bot);
            if (hive is null)
            {
                Console.WriteLine($"[AiryChannel] bot {botId} has no Hive connection; channel {message.Channel.Id} stays quiet.");
                return true;
            }

            var consented = await HasConsentAsync(services, botId, message.Author.Id);
            var transcript = services.GetRequiredService<ChannelTranscript>();
            var botName = BotNameOf(services, message, bot!);
            var author = NameOf(message.Author);
            if (AnswerRules.Hears(channel.Listens, consented)) transcript.Hear(botId, message.Channel.Id, new HeardLine(message.Id, author, content, false));
            else if (channel.Listens) transcript.Hear(botId, message.Channel.Id, HeardLine.NotShared(message.Id));
            else transcript.Forget(botId, message.Channel.Id);
            if (!channel.Enabled) return true;
            var watch = services.GetRequiredService<ChannelWatch>();
            if (targeted) watch.Cancel(botId, message.Channel.Id);
            else if (!string.IsNullOrWhiteSpace(content) && AnswerRules.Watches(channel.AnswerMode, targeted, consented))
            {
                watch.Note(botId, message.Channel.Id, () => JoinInAsync(root, message, botId), watch.InTalk(botId, message.Channel.Id) || AiryNames.In(content, botName));
                return true;
            }
            if (!AnswerRules.Answers(channel.AnswerMode, targeted, null)) return true;

            if (!consented)
            {
                await SendConsentAsync(services, message, botId);
                return true;
            }

            AgentReply reply;
            using (message.Channel.EnterTypingState())
            {
                var embeds = await EmbedsSoonAsync(message);
                if (embeds.Count > 0) content = Readable(message, botId, embeds);
                var pictures = ChatPictures.Pick(message.Content, StickersOf(message), embeds);
                var images = ImageAttachments.Pick(
                        message.Attachments.Select(a => ((string?)a.Url, (string?)a.Filename, (string?)a.ContentType)))
                    .Concat(pictures.Select(p => p.Image))
                    .ToList();
                var since = transcript.SinceBotSpoke(botId, message.Channel.Id, message.Id);
                var said = ChannelTalk.Message(botName, message.Channel.Name, $"{author} ({message.Author.Id})", content, since,
                    pictures.Select(p => p.Label).ToList(), since.Select(l => l.Text).Append(content).Any(t => AiryNames.In(t, botName)));
                transcript.Seen(botId, message.Channel.Id, message.Id);
                watch.Spoke(botId, message.Channel.Id);
                reply = await services.GetRequiredService<HiveAgentClient>().RunAsync(
                    hive.RunUrl, hive.AppKey, botId, message.Channel.Id, message.Author.Id, message.Author.Username, said, images);
            }
            await SendReplyAsync(services, message, reply.Text ?? (reply.Failed ? WebhookChatService.OfflineReply() : null));
            return true;
        }

        private static async Task JoinInAsync(IServiceProvider root, SocketMessage newest, ulong botId)
        {
            using var scope = root.CreateScope();
            var services = scope.ServiceProvider;
            var channelId = newest.Channel.Id;
            var channels = services.GetRequiredService<IAiryChannelRepository>();
            var channel = await channels.GetForChannelAsync(botId, channelId);
            if (channel is null || !channel.Enabled || channel.AnswerMode != AnswerModes.Decide) return;
            var bot = await services.GetRequiredService<IBotSettingRepository>().GetBotSettingAsync(botId);
            var hive = bot is null ? null : HiveConnection.Of(bot);
            if (hive is null) return;

            var watch = services.GetRequiredService<ChannelWatch>();
            var transcript = services.GetRequiredService<ChannelTranscript>();
            var botName = BotNameOf(services, newest, bot!);
            var author = NameOf(newest.Author);
            var unseen = transcript.Knows(botId, channelId)
                ? transcript.Unseen(botId, channelId)
                : [new HeardLine(newest.Id, author, Readable(newest, botId), false)];
            if (unseen.Count == 0) return;
            var last = unseen[^1];
            var followUp = watch.InTalk(botId, channelId);
            var addressed = unseen.Any(l => AiryNames.In(l.Text, botName));
            var invited = followUp || addressed;
            if (!invited && watch.CoolingDown(botId, channelId)) return;

            var unseenIds = unseen.Select(l => l.MessageId).ToHashSet();
            var stretch = followUp
                ? transcript.Recent(botId, channelId, 0, WorthQuestion.MaxRecent * 2)
                    .Where(l => !unseenIds.Contains(l.MessageId) && (l.FromBot || l.MessageId < last.MessageId))
                    .Concat(unseen)
                    .TakeLast(WorthQuestion.MaxRecent)
                    .ToList()
                : transcript.SinceBotSpoke(botId, channelId, 0).Where(l => l.MessageId <= last.MessageId).ToList();
            if (stretch.Count == 0) stretch = unseen.ToList();
            if (addressed)
            {
                try { await channels.RecordDecisionAsync(channel.Id, DateTime.UtcNow, HiveTexts.Say("named"), null); }
                catch (Exception ex) { Console.WriteLine($"[AiryChannel] could not record the decision: {ex.Message}"); }
            }
            else
            {
                var lines = stretch.Select(l => new RecentLine(l.FromBot ? botName : l.Author, l.Text)).ToList();
                var verdict = await services.GetRequiredService<HiveAnswerClient>().AskAsync(hive,
                    new WorthEvent(botName, newest.Channel.Name, lines[^1].Author, newest.Author.Id, lines[^1].Text, lines.Take(lines.Count - 1).ToList(), followUp));
                if (verdict.Error is not null) Console.WriteLine($"[AiryChannel] channel {channelId}: {verdict.Error}");
                try { await channels.RecordDecisionAsync(channel.Id, DateTime.UtcNow, verdict.Summary, verdict.Error); }
                catch (Exception ex) { Console.WriteLine($"[AiryChannel] could not record the decision: {ex.Message}"); }
                if (!AnswerRules.Answers(channel.AnswerMode, false, verdict)) return;
            }
            if (!invited)
            {
                if (watch.CoolingDown(botId, channelId)) return;
                watch.Joined(botId, channelId);
            }
            transcript.Seen(botId, channelId, unseen.Max(l => l.MessageId));
            watch.Spoke(botId, channelId);

            AgentReply reply;
            using (newest.Channel.EnterTypingState())
            {
                var embeds = await EmbedsSoonAsync(newest);
                var pictures = ChatPictures.Pick(newest.Content, StickersOf(newest), embeds);
                var images = ImageAttachments.Pick(
                        newest.Attachments.Select(a => ((string?)a.Url, (string?)a.Filename, (string?)a.ContentType)))
                    .Concat(pictures.Select(p => p.Image))
                    .ToList();
                var labels = pictures.Select(p => p.Label).ToList();
                var lastText = last.MessageId == newest.Id ? Readable(newest, botId, embeds) : last.Text;
                var aboutYou = stretch.Any(l => !l.FromBot && AiryNames.In(l.Text, botName));
                var said = followUp
                    ? ChannelTalk.Message(botName, newest.Channel.Name, $"{last.Author} ({newest.Author.Id})", lastText, unseen.Take(unseen.Count - 1).ToList(), labels, aboutYou)
                    : ChannelTalk.JoinIn(botName, newest.Channel.Name, $"{author} ({newest.Author.Id})", stretch, labels, aboutYou);
                reply = await services.GetRequiredService<HiveAgentClient>().RunAsync(
                    hive.RunUrl, hive.AppKey, botId, channelId, newest.Author.Id, newest.Author.Username, said, images);
            }
            if (reply.Text is not null) await SendReplyAsync(services, newest, reply.Text);
        }

        private static string BotNameOf(IServiceProvider services, SocketMessage message, BotSetting bot) =>
            (message.Channel as SocketGuildChannel)?.Guild.CurrentUser is { } me ? NameOf(me)
                : services.GetService<DiscordSocketClient>()?.CurrentUser?.Username ?? bot.BotName ?? "Airy";

        private static string NameOf(IUser user) => (user as SocketGuildUser)?.Nickname ?? user.GlobalName ?? user.Username;

        private static IReadOnlyList<StickerSeen> StickersOf(SocketMessage message) =>
            message.Stickers.Select(s => new StickerSeen(s.Id, s.Name, (int)s.Format)).ToList();

        private static IReadOnlyList<EmbedSeen> EmbedsOf(IMessage message) =>
            message.Embeds.Select(e => new EmbedSeen(e.Type.ToString(), e.Url, e.Thumbnail?.Url ?? e.Image?.Url)).ToList();

        private static async Task<IReadOnlyList<EmbedSeen>> EmbedsSoonAsync(SocketMessage message)
        {
            if (message.Embeds.Count > 0 || !ChatPictures.HasLink(message.Content)) return EmbedsOf(message);
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1.5));
                var fresh = await message.Channel.GetMessageAsync(message.Id, CacheMode.AllowDownload);
                return fresh is null ? EmbedsOf(message) : EmbedsOf(fresh);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiryChannel] could not read the link previews: {ex.Message}");
                return EmbedsOf(message);
            }
        }

        private static string Readable(SocketMessage message, ulong botId, IReadOnlyList<EmbedSeen>? embeds = null)
        {
            var mentioned = message.MentionedUsers
                .GroupBy(u => u.Id)
                .ToDictionary(g => g.Key, g => NameOf(g.First()));
            var guild = (message.Channel as SocketGuildChannel)?.Guild;
            var text = MentionResolver.Rewrite(message.Content, id =>
                mentioned.TryGetValue(id, out var n) ? n
                    : (guild?.GetUser(id) is { } gu ? NameOf(gu) : null), botId);
            return ChatPictures.Readable(text, StickersOf(message), embeds ?? EmbedsOf(message));
        }

        private void HearOwnLine(SocketMessage message)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var botId = scope.ServiceProvider.GetRequiredService<IConfigurationReader>().GetBotId();
                if (message.Author.Id != botId) return;
                scope.ServiceProvider.GetRequiredService<ChannelWatch>().Spoke(botId, message.Channel.Id);
                scope.ServiceProvider.GetRequiredService<ChannelTranscript>()
                    .HearIfKnown(botId, message.Channel.Id, new HeardLine(message.Id, NameOf(message.Author), Readable(message, botId), true));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AiryChannel] could not note the bot's own line: {ex.Message}");
            }
        }

        private static async Task<bool> HasConsentAsync(IServiceProvider services, ulong botId, ulong userId)
        {
            try { return await services.GetRequiredService<IUserConsentRepository>().HasConsentAsync(botId, userId); }
            catch (Exception cx)
            {
                Console.WriteLine($"[Consent] check failed, allowing through: {cx.Message}");
                return true;
            }
        }

        private static async Task SendConsentAsync(IServiceProvider services, SocketMessage message, ulong botId)
        {
            var evilId = services.GetRequiredService<IConfigurationReader>().GetEvilId();
            var embed = ConsentFrontend.ConsentEmbed(evilId);
            var components = new ComponentBuilder()
                .WithButton("Accept", ConsentInteraction.BuildAcceptId(botId, message.Author.Id), ButtonStyle.Success)
                .Build();
            try
            {
                var dm = await message.Author.CreateDMChannelAsync();
                await dm.SendMessageAsync(embed: embed, components: components);
            }
            catch
            {
                await message.Channel.SendMessageAsync(embed: embed, components: components);
            }
        }

        private static async Task SendReplyAsync(IServiceProvider services, SocketMessage message, string? reply)
        {
            if (string.IsNullOrWhiteSpace(reply)) return;
            var dedup = services.GetRequiredService<RecentMessageDedup>();
            foreach (var chunk in SplitForDiscord(reply))
            {
                if (!dedup.TryReserve(message.Channel.Id, chunk))
                {
                    Console.WriteLine($"[Webhook] reply chunk to {message.Channel.Id} suppressed — already said via effect.");
                    continue;
                }
                await message.Channel.SendMessageAsync(chunk);
            }
        }

        // Split text into <=max-char chunks for Discord, breaking on line then
        // space boundaries; only hard-cutting when a single run has no break.
        private static IEnumerable<string> SplitForDiscord(string text, int max = 2000)
        {
            if (text.Length <= max) { yield return text; yield break; }

            var sb = new StringBuilder();
            foreach (var rawLine in text.Split('\n'))
            {
                var line = rawLine;
                // A single line longer than the cap → break it (prefer a space).
                while (line.Length > max)
                {
                    if (sb.Length > 0) { yield return sb.ToString(); sb.Clear(); }
                    var cut = line.LastIndexOf(' ', Math.Min(max - 1, line.Length - 1));
                    if (cut <= 0) cut = max;
                    yield return line[..cut];
                    line = line[cut..].TrimStart();
                }
                // Flush the buffer if appending this line would overflow.
                if (sb.Length > 0 && sb.Length + line.Length + 1 > max)
                {
                    yield return sb.ToString(); sb.Clear();
                }
                if (sb.Length > 0) sb.Append('\n');
                sb.Append(line);
            }
            if (sb.Length > 0) yield return sb.ToString();
        }

    }
}
