using AiryBotCode.Application.Services.User;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using AiryBotCode.Application.Interfaces; // Added

namespace AiryBotCode.Application.Services.Loging
{
    public class LogService
    {
        private DiscordSocketClient _client;
        private readonly IConfigurationReader _configurationReader; // Added

        public LogService(IServiceProvider serviceProvider, IConfigurationReader configurationReader) // Modified constructor
        {
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            _configurationReader = configurationReader; // Assigned
        }

        public async Task<ulong> GetLogChannelId()
        {
            var logChannelId = _configurationReader.GetLogChannelId(); // Using IConfigurationReader
            if (logChannelId == 0)
            {
                await ContactEvil(SimpleLog("Config", "There is no LOGCHANNELID in appsettings.json"), false);
                return 0;
            }
            return logChannelId;
        }

        public async Task<ulong> GetErrorChannelEvilId()
        {
            var errorChannelEvilId = ulong.TryParse(_configurationReader.GetSection("Bots:EvilLogChannelId"), out var channelId) ? channelId : 0; // Using IConfigurationReader
            if (errorChannelEvilId == 0)
            {
                Console.WriteLine($"[LogService] Error: Bots:EvilLogChannelId not found in appsettings.json.");
                return 0;
            }
            return errorChannelEvilId;
        }

        public async Task LogToMainChannel(SocketSlashCommand command, Embed message)
        {
            var logChannel = _client.GetGuild(command.GuildId!.Value)?.GetTextChannel(await GetLogChannelId());
            if (logChannel == null)
            {
                await ContactEvil(SimpleLog("LogChannel", "Log channel not found 'LogChannelId'"), false); // Changed ping to false
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
            var evilId = ulong.TryParse(_configurationReader.GetSection("Bots:EvilId"), out var id) ? id : 0; // Using IConfigurationReader
            if (evilId == 0)
            {
                await ContactEvil(SimpleLog("Config", "There is no Bots:EvilId in appsettings.json"), false);
                return 0;
            }

            return evilId;
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
