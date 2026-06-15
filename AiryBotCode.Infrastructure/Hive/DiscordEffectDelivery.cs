using AiryBotCode.Application.Hive;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Hive
{
    /// <summary>
    /// Posts a Hive effect's message to its Discord channel via the bot's socket
    /// client. The thin, untestable edge behind <see cref="IEffectDelivery"/> — all
    /// the parse/route logic is covered by HiveEffectListener/EffectRouter tests.
    /// </summary>
    public sealed class DiscordEffectDelivery : IEffectDelivery
    {
        private readonly DiscordSocketClient _client;

        public DiscordEffectDelivery(DiscordSocketClient client) => _client = client;

        public async Task SendAsync(ulong channelId, string text, CancellationToken ct = default)
        {
            if (_client.GetChannel(channelId) is IMessageChannel channel)
                await channel.SendMessageAsync(text);
            else
                Console.WriteLine($"[HiveEffects] channel {channelId} not found / not a message channel — dropped.");
        }
    }
}
