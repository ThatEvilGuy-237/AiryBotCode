using AiryBotCode.Application.Features.Logging;
using AiryBotCode.Application.Services;
using AiryBotCode.Application.Services.Loging;
using AiryBotCode.Domain.Entities;
using AiryBotCode.Infrastructure.Interfaces;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AiryBotCode.Application.Interfaces;

namespace AiryBotCode.Infrastructure.Activitys
{
    public class UserlogsAction : EvilAction, ISlashAction, IButtonAction, IFormAction, IBanAction, ILeftAction
    {
        protected IConfigurationReader _config;
        protected LogService _logService;

        public UserlogsAction(IServiceProvider serviceProvider, IConfigurationReader configuration) : 
            base(serviceProvider.GetRequiredService<UserlogsCommand>(), serviceProvider)
        {
            _config = configuration;
            _logService = serviceProvider.GetRequiredService<LogService>();
        }
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            UserlogsCommand userlogs = (UserlogsCommand)Command;
            bool worked = await userlogs.HandleSlashCommand(command);
            //if (!worked) return;
        }

        public async Task HandleButtonPressAsync(SocketMessageComponent component, ButtonEncriptionService buttonEncription)
        {
            UserlogsCommand userlogs = (UserlogsCommand)Command;
            await userlogs.HandelEditButton(component, buttonEncription);
        }

        public async Task HandleFormAsync(SocketModal modal, ButtonEncriptionService buttonEncription)
        {
            UserlogsCommand userlogs = (UserlogsCommand)Command;
            await userlogs.HandleForm(modal, buttonEncription);
        }
        public async Task HandleBanAsync(SocketUser user, SocketGuild guild)
        {
            // Everything here is best-effort: a banned member is usually ALREADY
            // gone from the guild cache (GetUser → null) and GetBanAsync/audit can
            // race right after the ban — none of that may lose the log. The old
            // version NRE'd on those races (and then threw NotImplementedException
            // even on success), which is why ban logs intermittently "fell out".
            try
            {
                UserlogsCommand userlogs = (UserlogsCommand)Command;

                RestBan? ban = null;
                try { ban = await guild.GetBanAsync(user); } catch { /* race / missing perm */ }

                RestAuditLogEntry? auditEntry = null;
                try { auditEntry = await GetLatestEntryAsync(guild, user.Id, ActionType.Ban); } catch { }
                var moderator = auditEntry?.User;

                var reason = ban?.Reason ?? auditEntry?.Reason ?? "(no reason recorded)";
                var log = new LogInfo(LogType.Ban, guild.GetUser(user.Id), reason, "HAMMER!", "Got Banned")
                {
                    TargetUser = user
                };

                await userlogs.SendUserLog(moderator ?? guild.CurrentUser, log);
            }
            catch (Exception ex)
            {
                await _logService.ContactEvil(
                    _logService.SimpleLog("Ban log failed", $"{user} in {guild.Name}: {ex.Message}", Color.Red), true);
            }
        }

        /// <summary>UserLeft → was it a KICK? Discord has no kick event, so check
        /// the audit log for a fresh Kick entry targeting this user. A voluntary
        /// leave (no entry) is ignored. Same never-lose-the-log rules as bans.</summary>
        public async Task HandleLeftAsync(SocketGuild guild, SocketUser user)
        {
            try
            {
                RestAuditLogEntry? kick = null;
                try { kick = await GetLatestEntryAsync(guild, user.Id, ActionType.Kick); } catch { }
                if (kick is null) return;                                         // normal leave
                if (DateTimeOffset.UtcNow - kick.CreatedAt > TimeSpan.FromMinutes(2)) return; // stale entry

                UserlogsCommand userlogs = (UserlogsCommand)Command;
                var reason = kick.Reason ?? "(no reason recorded)";
                var log = new LogInfo(LogType.Kick, guild.GetUser(user.Id), reason, "BOOT!", "Got Kicked")
                {
                    TargetUser = user
                };
                await userlogs.SendUserLog(kick.User ?? guild.CurrentUser, log);
            }
            catch (Exception ex)
            {
                await _logService.ContactEvil(
                    _logService.SimpleLog("Kick log failed", $"{user} in {guild.Name}: {ex.Message}", Color.Red), true);
            }
        }

        public async Task<RestAuditLogEntry?> GetLatestEntryAsync(SocketGuild guild, ulong targetUserId, ActionType actionType)
        {
            if (!guild.CurrentUser.GuildPermissions.ViewAuditLog)
                return null;

            await foreach (var page in guild.GetAuditLogsAsync(5, actionType: actionType))
            {
                foreach (var entry in page)
                {
                    var matches = entry.Data switch
                    {
                        BanAuditLogData ban   => ban.Target.Id == targetUserId,
                        KickAuditLogData kick => kick.Target.Id == targetUserId,
                        _ => false,
                    };
                    if (matches) return entry; // latest matching entry
                }
            }

            return null; // Not found
        }

    }
}
