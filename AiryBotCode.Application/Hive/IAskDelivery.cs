namespace AiryBotCode.Application.Hive
{
    /// <summary>
    /// Delivers an await-mode <c>ask_user</c> effect to Discord: posts the question
    /// with one tappable button per option. Each button's customId encodes the Hive
    /// effect id (see <see cref="AskInteraction"/>) so the user's tap can be routed
    /// back over the WS as the answer. Infrastructure-implemented (Discord.Net).
    /// </summary>
    public interface IAskDelivery
    {
        Task SendAskAsync(ulong channelId, string effectId, string question, IReadOnlyList<string> options, CancellationToken ct = default);
    }
}
