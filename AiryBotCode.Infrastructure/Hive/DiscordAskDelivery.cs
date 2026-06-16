using AiryBotCode.Application.Frontend;
using AiryBotCode.Application.Hive;
using AiryBotCode.Application.Services;
using Discord;
using Discord.WebSocket;

namespace AiryBotCode.Infrastructure.Hive
{
    /// <summary>
    /// Posts an await-mode <c>ask_user</c> effect to its Discord channel as a question
    /// with one button per option. Button context rides in the customId via the project's
    /// canonical <see cref="ButtonEncriptionService"/>: <c>c:ask_user</c> (routing),
    /// <c>a:&lt;effectId&gt;</c> (the WS correlation key), and <c>u:&lt;askerUserId&gt;</c>
    /// (so only the user the agent asked can answer). The chosen ANSWER is the button's
    /// label — it is NOT stuffed into the customId, so long options can't blow Discord's
    /// 100-char customId limit. The thin, untestable edge behind <see cref="IAskDelivery"/>.
    /// </summary>
    public sealed class DiscordAskDelivery : IAskDelivery
    {
        private readonly DiscordSocketClient _client;

        public DiscordAskDelivery(DiscordSocketClient client) => _client = client;

        public async Task SendAskAsync(ulong channelId, string effectId, string question, IReadOnlyList<string> options, string? askerUserId, CancellationToken ct = default)
        {
            if (_client.GetChannel(channelId) is not IMessageChannel channel)
            {
                Console.WriteLine($"[HiveEffects] ask channel {channelId} not found / not a message channel — dropped.");
                return;
            }

            ulong.TryParse(askerUserId, out var asker);

            // Discord allows up to 5 buttons per action row; AskRouter caps options at 5.
            // Each button needs a UNIQUE customId, so the index disambiguates; effectId is
            // the WS correlation key; UsersId restricts who may answer. The chosen ANSWER
            // is the button LABEL (read back at press time), NOT in the customId — so a
            // long option can't blow Discord's 100-char customId limit.
            var builder = new ComponentBuilder();
            for (var i = 0; i < options.Count; i++)
            {
                var ctx = new ButtonEncriptionService { CommandName = AskRouter.Command, Action = $"{effectId}.{i}" };
                if (asker != 0) ctx.UsersId.Add(asker);
                builder.WithButton(label: Truncate(options[i], 80), customId: ctx.Encript(), style: ButtonStyle.Primary);
            }

            await channel.SendMessageAsync(embed: AskFrontend.AskEmbed(question, askerUserId), components: builder.Build());
        }

        private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max];
    }
}
