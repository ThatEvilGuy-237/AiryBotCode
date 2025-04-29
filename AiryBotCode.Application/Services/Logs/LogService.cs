using AiryBotCode.Application.Services.User;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Application.Services.Loging
{
    public class LogService : ClientService
    {
        public readonly ulong LogChannelId  = 1364679724269305967;
        public readonly ulong ErrorChannelEvilId = 1364679746356514936;

        public readonly UserService _userService;
        public LogService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task LogToMainChannel(SocketSlashCommand command, Embed message)
        {
            var logChannel = _client.GetGuild(command.GuildId!.Value)?.GetTextChannel(LogChannelId);
            if (logChannel == null)
            {
                await ContactEvil(SimpleLog("LogChannel", "Log channel not found 'LogChannelId'"));
            }
        }
        // const ulong errorChannelEvilId = 1364679746356514936;
        public async Task ContactEvil(Embed embed, bool ping = true)
        {
            string owner = ping ? (await GetEvilId()).ToString() : "";

            var channel = (ITextChannel)await _client.GetChannelAsync(ErrorChannelEvilId);

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
