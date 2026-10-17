using AiryBotCode.Application.Conversation;
using AiryBotCode.Application.Hive;
using AiryBotCode.Domain.database;
using Xunit;

namespace AiryBotCode.Tests
{
    public class AiryConversationTests
    {
        private const ulong Bot = 1;
        private const ulong Room = 10;
        private const ulong Evil = 405;
        private const ulong Kira = 501;
        private const ulong Stranger = 777;

        private static readonly ConversationSettings Fast = new()
        {
            Quiet = TimeSpan.FromMilliseconds(80),
            QuickQuiet = TimeSpan.FromMilliseconds(20),
            MaxMessages = 8,
            MaxWait = TimeSpan.FromMilliseconds(600),
            Cooldown = TimeSpan.FromMilliseconds(1500),
            InTalkFor = TimeSpan.FromMilliseconds(300),
        };

        private sealed class Store : IConversationStore
        {
            public AiryChannel? Row = new() { Id = 7, BotId = Bot, ChannelId = Room, AnswerMode = AnswerModes.Decide, Enabled = true, Listens = true };
            public HiveConnection? Hive = new("http://spine:7000", "agent", "key");
            public readonly HashSet<ulong> Members = [Evil, Kira];
            public readonly List<string> Records = [];

            public Task<AiryChannel?> ChannelAsync(ulong botId, ulong channelId) => Task.FromResult(channelId == Room ? Row : null);
            public Task<HiveConnection?> HiveAsync(ulong botId) => Task.FromResult(Hive);
            public Task<bool> IsMemberAsync(ulong botId, ulong userId) => Task.FromResult(Members.Contains(userId));

            public Task RecordAsync(int channelRowId, string summary, string? error)
            {
                lock (Records) Records.Add(summary);
                return Task.CompletedTask;
            }
        }

        private sealed class Decider : IHiveDecider
        {
            public Func<CheckRequest, CheckVerdict> Answer = _ => new CheckVerdict(true, true, 0.9, "d", null);
            public readonly List<CheckRequest> Asked = [];

            public Task<CheckVerdict> AskAsync(HiveConnection hive, CheckRequest check, CancellationToken ct = default)
            {
                lock (Asked) Asked.Add(check);
                return Task.FromResult(Answer(check));
            }
        }

        private sealed class Agent : IHiveAgent
        {
            public readonly List<string> Said = [];
            public TaskCompletionSource? Hold;
            public int Running;
            public int MostAtOnce;

            public async Task<AgentReply> RunAsync(string runUrl, string appKey, ulong botId, ulong channelId, ulong authorId,
                string author, string content, IReadOnlyList<ForwardedImage>? images, CancellationToken cancellationToken = default)
            {
                var now = Interlocked.Increment(ref Running);
                lock (Said)
                {
                    MostAtOnce = Math.Max(MostAtOnce, now);
                    Said.Add(content);
                }
                if (Hold is { } hold) await hold.Task;
                Interlocked.Decrement(ref Running);
                return new AgentReply($"reply {Said.Count}", false);
            }
        }

        private sealed class Channel(Func<AiryConversation> conversation) : IChatChannel
        {
            private static ulong _nextId = 900_000;
            public readonly List<string> Sent = [];
            public int ConsentAsks;

            public string BotName => "Airy";

            public IDisposable Typing() => new Nothing();

            public Task SendAsync(string text)
            {
                lock (Sent) Sent.Add(text);
                var id = Interlocked.Increment(ref _nextId);
                conversation().OnOwnMessage(new ChatMessage(id, Bot, Room, "airy-talk", Bot, "Airy", "airy", text, text, false, [], [], []));
                return Task.CompletedTask;
            }

            public Task AskConsentAsync()
            {
                Interlocked.Increment(ref ConsentAsks);
                return Task.CompletedTask;
            }

            public Task<ChatMessage> WithPreviewsAsync(ChatMessage message) => Task.FromResult(message);

            private sealed class Nothing : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }

        private readonly Store _store = new();
        private readonly Decider _decider = new();
        private readonly Agent _agent = new();
        private readonly AiryConversation _airy;
        private readonly Channel _channel;
        private ulong _id = 100;

        public AiryConversationTests()
        {
            _airy = new AiryConversation(_store, new ChannelLog(Fast), new ChannelWatch(Fast, TimeProvider.System), _decider, _agent);
            _channel = new Channel(() => _airy);
        }

        private Task<bool> Say(ulong author, string text, bool pinged = false) =>
            _airy.OnMessageAsync(new ChatMessage(++_id, Bot, Room, "airy-talk", author, Name(author), Name(author).ToLowerInvariant(),
                text, text, pinged, [], [], []), _channel);

        private static string Name(ulong id) => id switch { Evil => "Evil", Kira => "KiraZ", _ => "Pell" };

        private static async Task Until(Func<bool> done, int ms = 2000)
        {
            var end = DateTime.UtcNow.AddMilliseconds(ms);
            while (!done() && DateTime.UtcNow < end) await Task.Delay(10);
            Assert.True(done());
        }

        [Fact]
        public async Task A_ping_is_answered_at_once_with_what_was_said_since_she_spoke()
        {
            await Say(Kira, "anyone up for a quiz?");
            await Say(Evil, "@Airy what do you think", pinged: true);

            Assert.Single(_agent.Said);
            Assert.Contains("KiraZ: anyone up for a quiz?", _agent.Said[0]);
            Assert.Contains("Now Evil (405) writes:\n@Airy what do you think", _agent.Said[0]);
            Assert.Equal(["reply 1"], _channel.Sent);
            Assert.Empty(_decider.Asked);
        }

        [Fact]
        public async Task Someone_who_did_not_opt_in_gets_the_consent_message_and_no_answer()
        {
            await Say(Stranger, "@Airy hi", pinged: true);

            Assert.Equal(1, _channel.ConsentAsks);
            Assert.Empty(_agent.Said);
        }

        [Fact]
        public async Task Talk_without_a_ping_waits_for_a_pause_then_one_join_in_check_on_the_whole_stretch()
        {
            await Say(Kira, "anyone listen to trance audios before bed?");
            await Say(Stranger, "lol");
            await Say(Evil, "yeah they knock me out");
            Assert.Empty(_decider.Asked);

            await Until(() => _channel.Sent.Count == 1);
            var check = Assert.Single(_decider.Asked);
            Assert.False(check.FollowUp);
            Assert.Equal(["KiraZ", "someone (not opted in)"], check.Recent.Select(l => l.Author));
            Assert.Equal("yeah they knock me out", check.Message);
            Assert.Contains("Nobody pinged you; you chose to join in.", _agent.Said[0]);
            Assert.Equal(["yes 90%"], _store.Records);
        }

        [Fact]
        public async Task A_no_from_the_decider_keeps_her_quiet_and_is_recorded()
        {
            _decider.Answer = _ => new CheckVerdict(false, true, 0.97, "d", null);
            await Say(Kira, "lol same");

            await Until(() => _store.Records.Count == 1);
            await Task.Delay(100);
            Assert.Empty(_agent.Said);
            Assert.Equal(["no 97%"], _store.Records);
        }

        [Fact]
        public async Task Her_name_is_answered_after_a_short_pause_without_asking_the_decider()
        {
            await Say(Evil, "isn't that right foxy fox airy");

            await Until(() => _channel.Sent.Count == 1, 400);
            Assert.Empty(_decider.Asked);
            Assert.Equal(["named, answered"], _store.Records);
            Assert.Contains("Here Airy, the fox and the bot mean you", _agent.Said[0]);
        }

        [Fact]
        public async Task Right_after_she_spoke_a_message_gets_a_quick_follow_up_check_and_a_normal_reply()
        {
            await Say(Evil, "@Airy hey", pinged: true);
            await Say(Evil, "Good bot");

            await Until(() => _channel.Sent.Count == 2, 400);
            var check = Assert.Single(_decider.Asked);
            Assert.True(check.FollowUp);
            Assert.Contains(check.Recent, l => l.Author == "Airy" && l.Text == "reply 1");
            Assert.StartsWith("Evil (405) writes in #airy-talk:\nGood bot", _agent.Said[1]);
        }

        [Fact]
        public async Task After_joining_in_uninvited_she_stays_out_of_new_talk_until_the_cooldown_ends()
        {
            await Say(Kira, "how do you even get into trance?");
            await Until(() => _channel.Sent.Count == 1);
            await Task.Delay(Fast.InTalkFor + TimeSpan.FromMilliseconds(50));

            await Say(Kira, "which audio is best for that?");
            await Task.Delay(Fast.Quiet + TimeSpan.FromMilliseconds(150));
            Assert.Single(_decider.Asked);
            Assert.Single(_channel.Sent);
        }

        [Fact]
        public async Task Lines_sent_while_she_writes_get_their_own_check_after_and_she_never_writes_twice_at_once()
        {
            _agent.Hold = new TaskCompletionSource();
            var ping = Say(Kira, "@Airy i wish i was more suggestible", pinged: true);
            await Until(() => _agent.Said.Count == 1);
            await Say(Kira, "like... just turn my brain off please");
            await Task.Delay(Fast.QuickQuiet + TimeSpan.FromMilliseconds(60));
            Assert.Single(_agent.Said);

            _agent.Hold.SetResult();
            await ping;
            await Until(() => _channel.Sent.Count == 2);
            var check = Assert.Single(_decider.Asked);
            Assert.True(check.FollowUp);
            Assert.Equal("like... just turn my brain off please", check.Message);
            Assert.Equal(1, _agent.MostAtOnce);
        }

        [Fact]
        public async Task Nothing_unread_means_no_check()
        {
            await Say(Evil, "@Airy one", pinged: true);
            await Say(Evil, "two");
            await Say(Evil, "@Airy three", pinged: true);
            await Task.Delay(Fast.QuickQuiet + TimeSpan.FromMilliseconds(100));

            Assert.Empty(_decider.Asked);
            Assert.Equal(2, _agent.Said.Count);
            Assert.Contains("Evil: two", _agent.Said[1]);
        }

        [Fact]
        public async Task A_channel_that_does_not_listen_checks_the_message_on_its_own()
        {
            _store.Row!.Listens = false;
            await Say(Kira, "first line");
            await Say(Evil, "how do you get into trance?");

            await Until(() => _channel.Sent.Count == 1);
            var check = Assert.Single(_decider.Asked);
            Assert.Empty(check.Recent);
            Assert.Equal("how do you get into trance?", check.Message);
        }

        [Fact]
        public async Task With_answers_off_she_still_listens_but_never_speaks()
        {
            _store.Row!.Enabled = false;
            await Say(Evil, "@Airy hello?", pinged: true);
            await Say(Evil, "airy?");
            await Task.Delay(Fast.Quiet + TimeSpan.FromMilliseconds(100));

            Assert.Empty(_agent.Said);
            _store.Row.Enabled = true;
            await Say(Evil, "@Airy now?", pinged: true);
            Assert.Contains("Evil: @Airy hello?", _agent.Said[0]);
        }

        [Fact]
        public async Task Other_channels_and_a_missing_Hive_connection_are_left_alone()
        {
            Assert.False(await _airy.OnMessageAsync(new ChatMessage(1, Bot, 99, "other", Evil, "Evil", "evil", "hi", "hi", true, [], [], []), _channel));
            _store.Hive = null;
            Assert.True(await Say(Evil, "@Airy hi", pinged: true));
            Assert.Empty(_agent.Said);
        }
    }
}
