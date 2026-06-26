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

        [ReloadableSetting("Exempt role: members WITH this role are never caught by the spam filter, even if they also hold the monitored role. None = no exemption.", Category = "SpamCatcher", UiHint = "role")]
        public ulong IgnoredRoleId { get; set; } = 0;             // base: empty (no exemption)

        [ReloadableSetting("How many messages within the window count as spam.", Category = "SpamCatcher", UiHint = "slider:2,15")]
        public int MessageThreshold { get; set; } = 3;             // base: 3

        [ReloadableSetting("How many DISTINCT channels the burst must span.", Category = "SpamCatcher", UiHint = "slider:1,10")]
        public int DistinctChannels { get; set; } = 2;             // base: 2

        [ReloadableSetting("Sliding time window, in seconds.", Category = "SpamCatcher", UiHint = "duration:seconds")]
        public int WindowSeconds { get; set; } = 6;                // base: 6

        [ReloadableSetting("Timeout applied to a caught spammer, in minutes.", Category = "SpamCatcher", UiHint = "duration:minutes")]
        public int TimeoutMinutes { get; set; } = 1440;            // base: 24h

        [ReloadableSetting("Delete the offending messages when spam is caught.", Category = "SpamCatcher", UiHint = "boolean")]
        public bool DeleteMessages { get; set; } = true;           // base: true

        [ReloadableSetting("Role pinged in the log channel when spam is caught, so a moderator can review. None = no ping.", Category = "SpamCatcher", UiHint = "role")]
        public ulong AlertRoleId { get; set; } = 0;                // base: empty (no ping)
        // --- End of Settings Declaration ---

        private readonly UserService _userService;
        private readonly UserlogsCommand _userlogs;
        private readonly SpamTracker _tracker;
        private readonly AiryBotCode.Application.Services.Loging.LogService _logService;

        public SpamCatcherCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "spamcatcher";
            _userService = serviceProvider.GetRequiredService<UserService>();
            _userlogs = serviceProvider.GetRequiredService<UserlogsCommand>();
            _tracker = serviceProvider.GetRequiredService<SpamTracker>();
            _logService = serviceProvider.GetRequiredService<AiryBotCode.Application.Services.Loging.LogService>();
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

            // Exempt: members holding the ignore role are never caught, even if they
            // also hold the monitored role (e.g. trusted/staff role overrides the watch).
            if (IgnoredRoleId != 0 && member.Roles.Any(r => r.Id == IgnoredRoleId)) return;

            var result = _tracker.Record(
                guildChannel.Guild.Id, member.Id, message.Channel.Id, message.Id,
                MessageThreshold, DistinctChannels, WindowSeconds);

            if (result.Outcome == SpamOutcome.None) return;   // not spam (yet)

            // Tail of an already-caught burst: the timeout + log already fired, but these
            // messages were sent before it landed and are still spam — just delete them.
            if (result.Outcome == SpamOutcome.Tail)
            {
                if (DeleteMessages) await DeleteMessagesAsync(result.Messages);
                return;
            }

            var offending = result.Messages;
            var distinct = offending.Select(o => o.ChannelId).Distinct().Count();
            Console.WriteLine($"[SpamCatcher] {member.Username} ({member.Id}) tripped — {offending.Count} msgs across {distinct} channels in {WindowSeconds}s.");

            // 1) Timeout the spammer.
            try { await _userService.TimeOutUser(member, TimeoutMinutes); }
            catch (Exception ex) { Console.WriteLine($"[SpamCatcher] timeout failed: {ex.Message}"); }

            // 2) Wipe the whole burst. Don't just delete the few ids that tripped the
            //    threshold — gather the spammer's own recent messages across the guild and
            //    delete them (the same way /timeout's clear works), so the entire burst
            //    goes, not just the messages we happened to track.
            int deleted = DeleteMessages
                ? await _userService.ClearUserMessages(member, Math.Max(WindowSeconds, 30))
                : 0;

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

            // 4) Ping a moderator role in the log channel so a human can review.
            //    Mentions only notify from message CONTENT (not embeds), and the role
            //    is allow-listed explicitly so it pings even if it isn't "mentionable".
            if (AlertRoleId != 0)
            {
                try
                {
                    var logChannelId = await _logService.GetLogChannelId();
                    if (await _client.GetChannelAsync(logChannelId) is IMessageChannel logChannel)
                    {
                        await logChannel.SendMessageAsync(
                            $"<@&{AlertRoleId}> ⚠️ Spam caught — **{member.Username}** was timed out across {distinct} channel(s). Please review.",
                            allowedMentions: new AllowedMentions { RoleIds = new List<ulong> { AlertRoleId } });
                    }
                }
                catch (Exception ex) { Console.WriteLine($"[SpamCatcher] alert ping failed: {ex.Message}"); }
            }
        }

        // Delete a set of (channel, message) pairs, skipping any that error (already
        // gone, etc.). Returns how many were actually removed.
        private async Task<int> DeleteMessagesAsync(IReadOnlyList<(ulong ChannelId, ulong MessageId)> messages)
        {
            int deleted = 0;
            foreach (var (chId, msgId) in messages)
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
            return deleted;
        }
    }
}
