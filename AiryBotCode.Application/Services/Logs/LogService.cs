using AiryBotCode.Application.Services.User;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Services.Loging
{
    public class LogService
    {
        private DiscordSocketClient _client;
        public LogService(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        }

        public async Task<ulong> GetLogChannelId()
        {
            var logChannelId = ulong.TryParse(Environment.GetEnvironmentVariable("LOGCHANNELID"), out var channelId) ? channelId : 0;
            if (logChannelId == 0)
            {
                await ContactEvil(SimpleLog("ENV", "There is no LOGCHANNELID"), false);
                return 0;
            }
            return logChannelId;
        }

        public async Task<ulong> GetErrorChannelEvilId()
        {
            var errorChannelEvilId = ulong.TryParse(Environment.GetEnvironmentVariable("EVILLOGCHANNELID"), out var channelId) ? channelId : 0;
            if (errorChannelEvilId == 0)
            {
                Console.WriteLine($"[LogService] Error: EVILLOGCHANNELID not found in environment variables.");
                return 0;
            }
            return errorChannelEvilId;
        }

        public async Task LogToMainChannel(SocketSlashCommand command, Embed message)
        {
            var logChannel = _client.GetGuild(command.GuildId!.Value)?.GetTextChannel(await GetLogChannelId());
            if (logChannel == null)
            {
                await ContactEvil(SimpleLog("LogChannel", "Log channel not found 'LogChannelId'"));
            }
        }
        // const ulong errorChannelEvilId = 1364679746356514936;
        public async Task ContactEvil(Embed embed, bool ping = true)
        {
            string owner = ping ? (await GetEvilId()).ToString() : "";

            var channel = (ITextChannel)await _client.GetChannelAsync(await GetErrorChannelEvilId());

            await channel.SendMessageAsync(
                text: $"<@{owner}>",
                embed: embed
            );
        }
        private async Task<ulong> GetEvilId()
        {
            var adminRoleId = ulong.TryParse(Environment.GetEnvironmentVariable("EVILID"), out var roleId) ? roleId : 0;
            if (adminRoleId == 0)
            {
                await ContactEvil(SimpleLog("ENV", "There is no EVILID"), false);
                return 0;
            }

            return adminRoleId;
        }
        public Embed SimpleLog(string title, string description, Color? color = null)
        {
            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color ?? Color.Red)
                .WithCurrentTimestamp()
                .Build();

            return embed;
        }

    }
}
