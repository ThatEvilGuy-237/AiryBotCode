using System.Text;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.Configuration.Attributes;
using AiryBotCode.Domain.database;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Features.Leveling
{
    /// <summary>
    /// Message-based leveling, MEE6-style: every eligible message (rate-limited per
    /// user) grants random XP; crossing a level threshold announces it and hands out
    /// any role rewards. XP is GLOBAL across servers for now. Exposes a single
    /// <c>/level</c> slash command with <c>rank</c> and <c>leaderboard</c> subcommands.
    /// </summary>
    [ConfigurableCommand("LevelingCommand")]
    public class LevelingCommand : EvilCommand
    {
        // --- XP earning ---
        [ReloadableSetting("Minimum XP granted per eligible message.", Category = "Leveling", UiHint = "number")]
        public int XpMin { get; set; } = 15;

        [ReloadableSetting("Maximum XP granted per eligible message.", Category = "Leveling", UiHint = "number")]
        public int XpMax { get; set; } = 25;

        [ReloadableSetting("Seconds a user must wait between XP grants.", Category = "Leveling", UiHint = "number")]
        public int XpCooldownSeconds { get; set; } = 60;

        [ReloadableSetting("Global XP multiplier (events / boosts).", Category = "Leveling", UiHint = "number")]
        public double XpMultiplier { get; set; } = 1.0;

        // --- Level curve: quad·n² + linear·n + base (MEE6 = 5 / 50 / 100) ---
        [ReloadableSetting("Curve base XP (the +100 term).", Category = "Leveling", UiHint = "number")]
        public int BaseXp { get; set; } = 100;

        [ReloadableSetting("Curve linear factor (the 50·n term).", Category = "Leveling", UiHint = "number")]
        public int LinearFactor { get; set; } = 50;

        [ReloadableSetting("Curve quadratic factor (the 5·n² term).", Category = "Leveling", UiHint = "number")]
        public int QuadraticFactor { get; set; } = 5;

        // --- Level-up announcement (always on; 0 channel = announce in-channel) ---
        [ReloadableSetting("Level-up message. Placeholders: {user} {level} {xp}.", Category = "Leveling")]
        public string LevelUpMessage { get; set; } = "GG {user}, you reached level {level}! 🎉";

        [ReloadableSetting("Force level-up announcements to this channel id. 0 = announce where they leveled up.", Category = "Leveling")]
        public ulong LevelUpChannelId { get; set; } = 0;

        // --- Role rewards ---
        [ReloadableSetting("Level → role-id rewards, e.g. {\"5\":123,\"10\":456}.", Category = "Leveling", UiHint = "json")]
        public Dictionary<int, ulong> RoleRewards { get; set; } = new();

        [ReloadableSetting("Keep all earned reward roles (true) or replace the lower tier (false).", Category = "Leveling")]
        public bool StackRewardRoles { get; set; } = false;

        // --- Exclusions ---
        [ReloadableSetting("Channel ids that grant no XP.", Category = "Leveling", UiHint = "json")]
        public ulong[] NoXpChannelIds { get; set; } = Array.Empty<ulong>();

        [ReloadableSetting("Role ids that earn no XP.", Category = "Leveling", UiHint = "json")]
        public ulong[] NoXpRoleIds { get; set; } = Array.Empty<ulong>();

        // --- Leaderboard ---
        [ReloadableSetting("Entries shown per /level leaderboard page.", Category = "Leveling", UiHint = "number")]
        public int LeaderboardPageSize { get; set; } = 10;

        private readonly IServiceProvider _services;
        private readonly IConfigurationReader _config;
        private readonly XpCooldown _cooldown;

        public LevelingCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "level";
            _services = serviceProvider;
            _config = serviceProvider.GetRequiredService<IConfigurationReader>();
            _cooldown = serviceProvider.GetRequiredService<XpCooldown>();
        }

        public override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Leveling — check your rank or the leaderboard")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("rank")
                    .WithDescription("Show your (or someone's) level and XP")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("user", ApplicationCommandOptionType.User, "Whose rank to show", isRequired: false))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("leaderboard")
                    .WithDescription("Show the top members")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("page", ApplicationCommandOptionType.Integer, "Page number", isRequired: false));
        }

        // ---------------------------------------------------------------- accrual

        public async Task HandleMessageAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return;
            if (message is not SocketUserMessage) return;
            if (message.Channel is not SocketGuildChannel) return;
            if (message.Author is not SocketGuildUser member) return;

            // No-XP zones.
            if (NoXpChannelIds.Contains(message.Channel.Id)) return;
            if (member.Roles.Any(r => NoXpRoleIds.Contains(r.Id))) return;

            // One grant per user per cooldown window.
            if (!_cooldown.TryConsume(member.Id, XpCooldownSeconds)) return;

            var low = Math.Min(XpMin, XpMax);
            var high = Math.Max(XpMin, XpMax);
            var gain = (long)Math.Round(Random.Shared.Next(low, high + 1) * Math.Max(0, XpMultiplier));
            if (gain <= 0) return;

            var botId = _config.GetBotId();
            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ILevelUserRepository>();

            var row = await repo.GetAsync(botId, member.Id)
                      ?? new LevelUser { BotId = botId, UserId = member.Id };

            var oldLevel = LevelMath.Resolve(row.Xp, BaseXp, LinearFactor, QuadraticFactor).Level;
            row.Xp += gain;
            row.UserName = member.Username;
            row.LastXpAt = DateTime.UtcNow;
            var newLevel = LevelMath.Resolve(row.Xp, BaseXp, LinearFactor, QuadraticFactor).Level;
            row.Level = newLevel;

            await repo.SaveAsync(row);

            if (newLevel > oldLevel)
            {
                await AnnounceLevelUpAsync(member, message.Channel, newLevel, row.Xp);
                await ApplyRoleRewardsAsync(member, newLevel);
            }
        }

        private async Task AnnounceLevelUpAsync(SocketGuildUser member, ISocketMessageChannel origin, int level, long xp)
        {
            try
            {
                var text = LevelUpMessage
                    .Replace("{user}", member.Mention)
                    .Replace("{level}", level.ToString())
                    .Replace("{xp}", xp.ToString("N0"));

                IMessageChannel? target = LevelUpChannelId != 0
                    ? _client.GetChannel(LevelUpChannelId) as IMessageChannel
                    : origin;

                if (target != null)
                    await target.SendMessageAsync(text);
            }
            catch (Exception ex) { Console.WriteLine($"[Leveling] announce failed: {ex.Message}"); }
        }

        private async Task ApplyRoleRewardsAsync(SocketGuildUser member, int level)
        {
            if (RoleRewards == null || RoleRewards.Count == 0) return;

            var earned = RoleRewards.Where(kv => kv.Key <= level).OrderBy(kv => kv.Key).ToList();
            if (earned.Count == 0) return;

            var guild = member.Guild;
            try
            {
                if (StackRewardRoles)
                {
                    foreach (var kv in earned)
                    {
                        var role = guild.GetRole(kv.Value);
                        if (role != null && !member.Roles.Any(r => r.Id == role.Id))
                            await member.AddRoleAsync(role);
                    }
                }
                else
                {
                    // Grant the top tier reached; strip the lower reward roles.
                    var top = earned[^1];
                    var topRole = guild.GetRole(top.Value);
                    if (topRole != null && !member.Roles.Any(r => r.Id == topRole.Id))
                        await member.AddRoleAsync(topRole);

                    foreach (var kv in earned.Take(earned.Count - 1))
                    {
                        var role = guild.GetRole(kv.Value);
                        if (role != null && member.Roles.Any(r => r.Id == role.Id))
                            await member.RemoveRoleAsync(role);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"[Leveling] role reward failed: {ex.Message}"); }
        }

        // ------------------------------------------------------------- slash side

        public async Task HandleSlashAsync(SocketSlashCommand command)
        {
            var sub = command.Data.Options.FirstOrDefault();
            switch (sub?.Name)
            {
                case "rank": await HandleRankAsync(command, sub); break;
                case "leaderboard": await HandleLeaderboardAsync(command, sub); break;
                default: await command.RespondAsync("Unknown subcommand.", ephemeral: true); break;
            }
        }

        private async Task HandleRankAsync(SocketSlashCommand command, SocketSlashCommandDataOption sub)
        {
            var target = sub.Options.FirstOrDefault(o => o.Name == "user")?.Value as SocketUser ?? command.User;
            var botId = _config.GetBotId();

            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ILevelUserRepository>();

            var row = await repo.GetAsync(botId, target.Id);
            if (row == null)
            {
                await command.RespondAsync($"{target.Username} hasn't earned any XP yet.", ephemeral: true);
                return;
            }

            var res = LevelMath.Resolve(row.Xp, BaseXp, LinearFactor, QuadraticFactor);
            var rank = await repo.GetRankAsync(botId, target.Id);
            var total = await repo.CountAsync(botId);

            var embed = new EmbedBuilder()
                .WithTitle($"Rank — {target.Username}")
                .WithThumbnailUrl(target.GetDisplayAvatarUrl())
                .AddField("Level", res.Level, true)
                .AddField("XP", row.Xp.ToString("N0"), true)
                .AddField("Rank", $"#{rank} / {total}", true)
                .AddField("Progress to next", ProgressBar(res.IntoLevel, res.NeededForNext), false)
                .WithColor(Color.Gold)
                .WithCurrentTimestamp()
                .Build();

            await command.RespondAsync(embed: embed);
        }

        private async Task HandleLeaderboardAsync(SocketSlashCommand command, SocketSlashCommandDataOption sub)
        {
            var pageRaw = sub.Options.FirstOrDefault(o => o.Name == "page")?.Value is long l ? l : 1L;
            var pageSize = Math.Max(1, LeaderboardPageSize);
            var botId = _config.GetBotId();

            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ILevelUserRepository>();

            var total = await repo.CountAsync(botId);
            if (total == 0)
            {
                await command.RespondAsync("No one has earned any XP yet.", ephemeral: true);
                return;
            }

            var pages = (int)Math.Ceiling(total / (double)pageSize);
            var page = (int)Math.Clamp(pageRaw, 1, pages);
            var skip = (page - 1) * pageSize;
            var top = await repo.TopAsync(botId, skip, pageSize);

            var sb = new StringBuilder();
            for (int i = 0; i < top.Count; i++)
            {
                var u = top[i];
                var res = LevelMath.Resolve(u.Xp, BaseXp, LinearFactor, QuadraticFactor);
                var rank = skip + i + 1;
                var medal = rank switch { 1 => "🥇", 2 => "🥈", 3 => "🥉", _ => $"`#{rank}`" };
                sb.AppendLine($"{medal} <@{u.UserId}> — **Lvl {res.Level}** ({u.Xp:N0} XP)");
            }

            var embed = new EmbedBuilder()
                .WithTitle("🏆 Leaderboard")
                .WithDescription(sb.ToString())
                .WithFooter($"Page {page}/{pages} • {total} ranked")
                .WithColor(Color.Gold)
                .Build();

            await command.RespondAsync(embed: embed);
        }

        private static string ProgressBar(long into, long needed, int slots = 12)
        {
            if (needed <= 0) needed = 1;
            var filled = (int)Math.Round((double)into / needed * slots);
            filled = Math.Clamp(filled, 0, slots);
            return $"{new string('▰', filled)}{new string('▱', slots - filled)}  {into:N0}/{needed:N0} XP";
        }
    }
}
