using AiryBotCode.Application.Hive;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Hive
{
    /// <summary>
    /// Posts an await-mode <c>ask_user</c> effect to its Discord channel as a question
    /// with one button per option. Each button's customId carries the Hive effect id
    /// (<see cref="AskInteraction.BuildAnswerId"/>) so the user's tap can be routed back
    /// as the answer. The thin, untestable edge behind <see cref="IAskDelivery"/>.
    /// </summary>
    public sealed class DiscordAskDelivery : IAskDelivery
    {
        private readonly DiscordSocketClient _client;

        public DiscordAskDelivery(DiscordSocketClient client) => _client = client;

        public async Task SendAskAsync(ulong channelId, string effectId, string question, IReadOnlyList<string> options, CancellationToken ct = default)
        {
            if (_client.GetChannel(channelId) is not IMessageChannel channel)
            {
                Console.WriteLine($"[HiveEffects] ask channel {channelId} not found / not a message channel — dropped.");
                return;
            }

            // Discord allows up to 5 buttons per action row; AskRouter already caps at 5.
            var builder = new ComponentBuilder();
            foreach (var opt in options)
                builder.WithButton(label: Truncate(opt, 80), customId: AskInteraction.BuildAnswerId(effectId, opt), style: ButtonStyle.Primary);

            await channel.SendMessageAsync(text: question, components: builder.Build());
        }

        private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max];
    }
}
