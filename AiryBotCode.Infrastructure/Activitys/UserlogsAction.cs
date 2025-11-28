using AiryBotCode.Application.Comands.SlashCommands;
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
    public class UserlogsAction : EvilAction, ISlashAction, IButtonAction, IFormAction, IBanAction
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
            UserlogsCommand userlogs = (UserlogsCommand)Command;
            //await userlogs.HandleForm(modal, buttonEncription);
            RestBan? ban = await guild.GetBanAsync(user);
            SocketGuildUser target = guild.GetUser(user.Id);

            var auditEnumerator = guild.GetAuditLogsAsync(5, actionType: ActionType.Ban);

            RestAuditLogEntry? auditEntry = await GetLatestBanEntryAsync(guild, user.Id);
            var moderator = auditEntry?.User;
            var reason = (auditEntry?.Data as BanAuditLogData);

            var log = new LogInfo(LogType.Ban, target, ban.Reason, "HAMMER!", "Got Baned");

            if(target == null)
            {
                await _logService.ContactEvil(
                  _logService.SimpleLog("Ban ERROR", "Not been able to get the TARGET that got baned", Color.Red), true);
            } 
            if (moderator == null)
            {
               await _logService.ContactEvil(
                   _logService.SimpleLog("Ban ERROR", "Not been able to get the user that did the ban Action", Color.Red),true);
            }
          
            await userlogs.SendUserLog(moderator, log);
            throw new NotImplementedException();
        }

        public async Task<RestAuditLogEntry?> GetLatestBanEntryAsync(SocketGuild guild, ulong bannedUserId)
        {
            if (!guild.CurrentUser.GuildPermissions.ViewAuditLog)
                return null;

            await foreach (var page in guild.GetAuditLogsAsync(5, actionType: ActionType.Ban))
            {
                foreach (var entry in page)
                {
                    if (entry.Data is BanAuditLogData banData && banData.Target.Id == bannedUserId)
                    {
                        return entry; // This is the latest matching ban
                    }
                }
            }

            return null; // Not found
        }

    }
}
