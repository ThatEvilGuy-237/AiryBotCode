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
        private readonly RecentMessageDedup _dedup;

        public DiscordEffectDelivery(DiscordSocketClient client, RecentMessageDedup dedup)
        {
            _client = client;
            _dedup = dedup;
        }

        public async Task SendAsync(ulong channelId, string text, CancellationToken ct = default)
        {
            // Skip if the webhook-reply path already posted this exact text to the channel
            // (or post it and let the reply path skip) — see RecentMessageDedup.
            if (!_dedup.TryReserve(channelId, text))
            {
                Console.WriteLine($"[HiveEffects] say to {channelId} suppressed — duplicate of a just-posted message.");
                return;
            }

            if (_client.GetChannel(channelId) is IMessageChannel channel)
                await channel.SendMessageAsync(text);
            else
                Console.WriteLine($"[HiveEffects] channel {channelId} not found / not a message channel — dropped.");
        }
    }
}
