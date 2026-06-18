using System.Globalization;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.Configuration.Attributes;
using AiryBotCode.Domain.database;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Features.Counting
{
    /// <summary>
    /// The counting game. Members count up in a channel; numbers may be plain or
    /// math expressions (<see cref="CountingMath"/>). Success is a silent ✅ react;
    /// failure and mini-boss spawns are voiced by Airy over the existing Hive socket
    /// (with a static fallback). Message-driven, plus a /counting status command.
    /// </summary>
    [ConfigurableCommand("CountingCommand")]
    public class CountingCommand : EvilCommand
    {
        // --- Channel / rules ---
        [ReloadableSetting("Counting channel id. 0 = game OFF.", Category = "Counting")]
        public ulong CountingChannelId { get; set; } = 0;

        [ReloadableSetting("Allow math expressions as answers (6*2, sqrt(144)).", Category = "Counting")]
        public bool AllowMath { get; set; } = true;

        [ReloadableSetting("Require a formula — a bare number doesn't count.", Category = "Counting")]
        public bool MathOnly { get; set; } = false;

        [ReloadableSetting("Allow the same user to count twice in a row.", Category = "Counting")]
        public bool AllowSameUserTwice { get; set; } = false;

        [ReloadableSetting("The first number of a run.", Category = "Counting", UiHint = "number")]
        public int StartNumber { get; set; } = 1;

        [ReloadableSetting("Reset to the start on a wrong count.", Category = "Counting")]
        public bool ResetOnWrong { get; set; } = true;

        [ReloadableSetting("React ✅ on a correct count.", Category = "Counting")]
        public bool ReactOnSuccess { get; set; } = true;

        [ReloadableSetting("How close a (decimal) answer must be to count.", Category = "Counting", UiHint = "number")]
        public double DecimalTolerance { get; set; } = 0.01;

        [ReloadableSetting("Track the all-time high score.", Category = "Counting")]
        public bool TrackHighScore { get; set; } = true;

        [ReloadableSetting("Fallback fail message when the Hive is unreachable. {user} {count} {next} {start}.", Category = "Counting")]
        public string FailMessage { get; set; } = "💥 {user} broke the chain at **{count}**! The next number was **{next}**. Starting over from {start}.";

        // --- Mini-bosses ---
        [ReloadableSetting("Enable Airy-generated mini-boss puzzles at milestones.", Category = "Counting")]
        public bool BossesEnabled { get; set; } = true;

        [ReloadableSetting("Milestone counts that spawn a boss; past the last, the last gap repeats.", Category = "Counting", UiHint = "json")]
        public int[] BossMilestones { get; set; } = new[] { 10, 20, 50, 100, 200, 300, 400 };

        [ReloadableSetting("A wrong boss answer resets the count (otherwise it's ignored).", Category = "Counting")]
        public bool BossWrongResets { get; set; } = false;

        [ReloadableSetting("Abandon an unsolved/pending boss after this many seconds (anti-softlock).", Category = "Counting", UiHint = "number")]
        public int BossTimeoutSeconds { get; set; } = 300;

        private readonly IServiceProvider _services;
        private readonly IConfigurationReader _config;
        private readonly IHiveResponseSender _hive;

        public CountingCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "counting";
            _services = serviceProvider;
            _config = serviceProvider.GetRequiredService<IConfigurationReader>();
            _hive = serviceProvider.GetRequiredService<IHiveResponseSender>();
        }

        public override SlashCommandBuilder GetCommand() =>
            new SlashCommandBuilder().WithName(Name).WithDescription("Show the counting game status");

        // ---------------------------------------------------------------- counting

        public async Task HandleMessageAsync(SocketMessage message)
        {
            if (CountingChannelId == 0) return;
            if (message.Channel.Id != CountingChannelId) return;
            if (message.Author.IsBot) return;
            if (message is not SocketUserMessage) return;
            if (message.Author is not SocketGuildUser member) return;

            var content = message.Content?.Trim() ?? "";
            if (content.Length == 0) return;

            var (parsed, value) = TryParse(content);
            if (!parsed) return;   // not a number/expression → chatter, ignore

            var botId = _config.GetBotId();
            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICountingStateRepository>();
            var state = await repo.GetAsync(botId, CountingChannelId)
                        ?? new CountingState { BotId = botId, ChannelId = CountingChannelId, CurrentCount = StartNumber - 1 };

            if (state.BossActive)
            {
                await HandleBossAttemptAsync(message, member, value, state, repo);
                return;
            }

            // Same user twice in a row (once a run is going).
            if (!AllowSameUserTwice && state.LastUserId == member.Id && state.CurrentCount >= StartNumber)
            {
                await FailAsync(message, member, state, repo, "counted twice in a row");
                return;
            }

            var expected = state.CurrentCount + 1;
            if (Matches(value, expected))
            {
                state.CurrentCount = expected;
                state.LastUserId = member.Id;
                state.LastMessageId = message.Id;
                if (TrackHighScore && expected > state.HighScore) state.HighScore = expected;

                // No Hive connection → no boss; counting just continues.
                var spawningBoss = BossesEnabled && IsMilestone(expected) && _hive.IsConnected;
                if (spawningBoss)
                {
                    state.BossActive = true;
                    state.BossAnswer = null;            // pending until the Hive sends it
                    state.BossSpawnedAt = DateTime.UtcNow;
                }

                // Persist BEFORE notifying the Hive: the counting_boss_answer comes
                // straight back, and the sink (a separate scope) must find BossActive
                // already committed or it would drop the answer and soft-lock the boss.
                await repo.SaveAsync(state);

                if (spawningBoss)
                    await _hive.SendEventAsync(
                        "counting_boss",
                        new { channelId = message.Channel.Id.ToString(), milestone = expected },
                        message.Channel.Id.ToString());

                if (ReactOnSuccess) await TryReactAsync(message);
            }
            else
            {
                await FailAsync(message, member, state, repo, "wrong number");
            }
        }

        private (bool ok, double value) TryParse(string content)
        {
            bool isPlainNumber = double.TryParse(content, NumberStyles.Float, CultureInfo.InvariantCulture, out var n);
            if (isPlainNumber)
                return MathOnly ? (false, 0) : (true, n);   // bare number disallowed in math-only mode

            if (!AllowMath) return (false, 0);
            var r = CountingMath.Evaluate(content);
            return r.Ok ? (true, r.Value) : (false, 0);
        }

        private bool Matches(double value, double expected) =>
            Math.Abs(value - expected) <= Math.Max(0, DecimalTolerance);

        private bool IsMilestone(long count)
        {
            var ms = BossMilestones;
            if (ms == null || ms.Length == 0) return false;
            foreach (var m in ms) if (m == count) return true;

            long last = ms[^1];
            if (count <= last) return false;
            long gap = ms.Length >= 2 ? ms[^1] - ms[^2] : last;
            return gap > 0 && (count - last) % gap == 0;
        }

        // ---------------------------------------------------------------- bosses

        private async Task HandleBossAttemptAsync(
            SocketMessage message, SocketGuildUser member, double value, CountingState state, ICountingStateRepository repo)
        {
            // Pending: waiting on Airy. Abandon if it's been too long (anti-softlock).
            if (state.BossAnswer is null)
            {
                if (state.BossSpawnedAt is { } at &&
                    (DateTime.UtcNow - at).TotalSeconds > Math.Max(30, BossTimeoutSeconds))
                {
                    state.BossActive = false;
                    state.BossSpawnedAt = null;
                    await repo.SaveAsync(state);
                }
                return;
            }

            if (Math.Abs(value - state.BossAnswer.Value) <= Math.Max(0, DecimalTolerance))
            {
                state.BossActive = false;
                state.BossAnswer = null;
                state.BossSpawnedAt = null;
                await repo.SaveAsync(state);
                if (ReactOnSuccess) await TryReactAsync(message);

                _ = _hive.SendEventAsync(
                    "counting_boss_defeated",
                    new { channelId = message.Channel.Id.ToString(), userId = member.Id.ToString() },
                    message.Channel.Id.ToString());
            }
            else if (BossWrongResets)
            {
                await FailAsync(message, member, state, repo, "failed the boss");
            }
            // otherwise: wrong boss answer → ignored, boss stays up
        }

        // ---------------------------------------------------------------- fail

        private async Task FailAsync(
            SocketMessage message, SocketGuildUser member, CountingState state, ICountingStateRepository repo, string reason)
        {
            var brokeAt = state.CurrentCount;
            var next = state.CurrentCount + 1;
            if (TrackHighScore && state.CurrentCount > state.HighScore) state.HighScore = state.CurrentCount;

            if (ResetOnWrong)
            {
                state.CurrentCount = StartNumber - 1;
                state.LastUserId = 0;
                state.LastMessageId = 0;
            }
            state.BossActive = false;
            state.BossAnswer = null;
            state.BossSpawnedAt = null;
            await repo.SaveAsync(state);

            // Voice it through Airy; fall back to a static message if the Hive is down.
            var sent = await _hive.SendEventAsync(
                "counting_fail",
                new
                {
                    channelId = message.Channel.Id.ToString(),
                    userId = member.Id.ToString(),
                    userName = member.Username,
                    brokeAt,
                    next,
                    highScore = state.HighScore,
                    reason,
                },
                message.Channel.Id.ToString());

            if (!sent)
            {
                var text = FailMessage
                    .Replace("{user}", member.Mention)
                    .Replace("{count}", brokeAt.ToString())
                    .Replace("{next}", next.ToString())
                    .Replace("{start}", StartNumber.ToString());
                try { await message.Channel.SendMessageAsync(text); } catch { }
            }
        }

        private static async Task TryReactAsync(SocketMessage message)
        {
            try { if (message is IUserMessage um) await um.AddReactionAsync(new Emoji("✅")); }
            catch { /* missing perms / deleted message — ignore */ }
        }

        // ---------------------------------------------------------------- /counting

        public async Task HandleStatusAsync(SocketSlashCommand command)
        {
            if (CountingChannelId == 0)
            {
                await command.RespondAsync("Counting isn't set up here.", ephemeral: true);
                return;
            }

            var botId = _config.GetBotId();
            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICountingStateRepository>();
            var state = await repo.GetAsync(botId, CountingChannelId);

            long current = state?.CurrentCount ?? (StartNumber - 1);
            long record = state?.HighScore ?? 0;

            var embed = new EmbedBuilder()
                .WithTitle("🔢 Counting")
                .AddField("Next number", current + 1, true)
                .AddField("Record", record, true)
                .AddField("Channel", $"<#{CountingChannelId}>", true)
                .WithColor(Color.Teal);

            if (state?.BossActive == true)
                embed.AddField("⚔️ Boss", state.BossAnswer is null ? "incoming…" : "active — solve it!", false);

            await command.RespondAsync(embed: embed.Build());
        }
    }
}
