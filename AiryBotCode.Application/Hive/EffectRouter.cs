namespace AiryBotCode.Application.Hive
{
    /// <summary>A resolved instruction to deliver one message to a Discord channel.</summary>
    public sealed record DeliveryIntent(ulong ChannelId, string Text, int DelaySeconds);

    /// <summary>
    /// Pure decision layer for Hive "effect" frames that arrive over the tools WS.
    /// Turns a fire-and-forget effect (e.g. the agent's <c>say</c> tool) into a
    /// concrete "post this text to this channel" intent — or null when the effect
    /// isn't a deliverable message or can't be routed.
    ///
    /// The channel is the run's <c>sessionId</c>: for an Airy webhook flow the Hive
    /// sets <c>sessionId = channelId</c> (Neural-Spine FlowRunner), so it parses
    /// straight back to the originating Discord channel.
    /// </summary>
    public static class EffectRouter
    {
        private const int DefaultDelay = 2;
        private const int MaxDelay = 60;

        public static DeliveryIntent? Route(string? name, string? message, double? delaySeconds, string? sessionId)
        {
            // Only message-bearing effects are deliverable. `schedule_message` is a
            // deferred send and is handled elsewhere, not posted immediately.
            if (name is not ("say" or "send_message")) return null;
            if (string.IsNullOrWhiteSpace(message)) return null;
            if (!ulong.TryParse(sessionId, out var channelId)) return null;

            var delay = delaySeconds is { } d && double.IsFinite(d)
                ? Math.Clamp((int)d, 0, MaxDelay)
                : DefaultDelay;

            return new DeliveryIntent(channelId, message.Trim(), delay);
        }
    }
}
