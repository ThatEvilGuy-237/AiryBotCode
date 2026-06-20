using AiryBotCode.Application.Features.Logging;
using AiryBotCode.Application.Services.User;
using AiryBotCode.Domain.Configuration.Attributes;
using AiryBotCode.Domain.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Features.SpamCatcher
{
    /// <summary>
    /// Message-driven anti-spam. A member who holds the configured role and posts a
    /// burst of messages across several channels in a short window is automatically
    /// timed out, the burst is deleted, and the action is logged like any other
    /// moderation log.
    ///
    /// This is NOT a slash command — it has no <see cref="GetCommand"/>. It exists as
    /// a <see cref="EvilCommand"/> only so its tunables flow through the standard
    /// [ConfigurableCommand] settings/seed/hot-reload pipeline, and so it can be
    /// enabled per-bot from the control panel.
    /// </summary>
    [ConfigurableCommand("SpamCatcherCommand")]
    public class SpamCatcherCommand : EvilCommand
    {
        // --- Settings Declaration for Seeder ---
        [ReloadableSetting("Role that is watched for cross-channel spam. None = catcher OFF. Only members WITH this role are monitored, each one independently.", Category = "SpamCatcher", UiHint = "role")]
        public ulong MonitoredRoleId { get; set; } = 0;            // base: empty (off)

        [ReloadableSetting("How many messages within the window count as spam.", Category = "SpamCatcher", UiHint = "number")]
        public int MessageThreshold { get; set; } = 3;             // base: 3

        [ReloadableSetting("How many DISTINCT channels the burst must span.", Category = "SpamCatcher", UiHint = "number")]
        public int DistinctChannels { get; set; } = 2;             // base: 2

        [ReloadableSetting("Sliding time window, in seconds.", Category = "SpamCatcher", UiHint = "number")]
        public int WindowSeconds { get; set; } = 6;                // base: 6

        [ReloadableSetting("Timeout applied to a caught spammer, in minutes.", Category = "SpamCatcher", UiHint = "number")]
        public int TimeoutMinutes { get; set; } = 1440;            // base: 24h

        [ReloadableSetting("Delete the offending messages when spam is caught.", Category = "SpamCatcher", UiHint = "boolean")]
        public bool DeleteMessages { get; set; } = true;           // base: true
        // --- End of Settings Declaration ---

        private readonly UserService _userService;
        private readonly UserlogsCommand _userlogs;
        private readonly SpamTracker _tracker;

        public SpamCatcherCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "spamcatcher";
            _userService = serviceProvider.GetRequiredService<UserService>();
            _userlogs = serviceProvider.GetRequiredService<UserlogsCommand>();
            _tracker = serviceProvider.GetRequiredService<SpamTracker>();
        }

        // Message-driven only: never registered as a slash command.
        public override SlashCommandBuilder GetCommand() =>
            throw new NotSupportedException("SpamCatcherCommand is message-driven and has no slash command.");

        public async Task HandleMessageAsync(SocketMessage message)
        {
            // Off until a role is configured.
            if (MonitoredRoleId == 0) return;

            // Guild text messages from real members only (bots are already filtered
            // upstream, but guard anyway).
            if (!MessageGuard.TryGuildMessage(message, out var member, out var guildChannel)) return;

            // Only members who hold the monitored role are watched.
            if (!member.Roles.Any(r => r.Id == MonitoredRoleId)) return;

            var offending = _tracker.Record(
                guildChannel.Guild.Id, member.Id, message.Channel.Id, message.Id,
                MessageThreshold, DistinctChannels, WindowSeconds);
            if (offending == null) return;   // not spam (yet)

            var distinct = offending.Select(o => o.ChannelId).Distinct().Count();
            Console.WriteLine($"[SpamCatcher] {member.Username} ({member.Id}) tripped — {offending.Count} msgs across {distinct} channels in {WindowSeconds}s.");

            // 1) Timeout the spammer.
            try { await _userService.TimeOutUser(member, TimeoutMinutes); }
            catch (Exception ex) { Console.WriteLine($"[SpamCatcher] timeout failed: {ex.Message}"); }

            // 2) Delete the offending burst.
            int deleted = 0;
            if (DeleteMessages)
            {
                foreach (var (chId, msgId) in offending)
                {
                    try
                    {
                        if (_client.GetChannel(chId) is IMessageChannel ch)
                        {
                            await ch.DeleteMessageAsync(msgId);
                            deleted++;
                        }
                    }
                    catch (Exception ex) { Console.WriteLine($"[SpamCatcher] delete failed for {msgId}: {ex.Message}"); }
                }
            }

            // 3) Log it to the log channel, like the other moderation logs (logged by
            //    the bot itself, type Timeout).
            try
            {
                var until = DateTimeOffset.UtcNow.AddMinutes(TimeoutMinutes);
                var info = new LogInfo
                {
                    Type = LogType.Timeout,
                    Target = member,
                    Reason = $"Automatic spam detection: {offending.Count} messages across {distinct} channels within {WindowSeconds}s.",
                    Action = $"Timed out for {TimeoutMinutes} minute(s); {deleted} message(s) deleted.",
                    Consequences = $"Muted until `{until:f}` UTC."
                };
                await _userlogs.SendUserLog(_client.CurrentUser, info);
            }
            catch (Exception ex) { Console.WriteLine($"[SpamCatcher] log failed: {ex.Message}"); }
        }
    }
}
