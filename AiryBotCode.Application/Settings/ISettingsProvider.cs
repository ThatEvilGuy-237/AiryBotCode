namespace AiryBotCode.Application.Settings
{
    /// <summary>
    /// Holds the preloaded <see cref="AirySettings"/> tree for the whole app.
    /// Registered as a singleton; loaded once at startup before the bot handles
    /// any events, so handlers never block on a DB read mid-interaction.
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// The current in-memory settings. Lazy-loads on first access if
        /// <see cref="LoadAsync"/> was not called at startup (keeps handlers safe
        /// even on bots that haven't wired the explicit preload yet).
        /// </summary>
        AirySettings Current { get; }

        /// <summary>
        /// Load (or reload) settings from the persisted <c>BotSetting</c>. Call
        /// once at startup, after seeding and before the bot begins handling events.
        /// </summary>
        Task<AirySettings> LoadAsync();

        /// <summary>Re-read settings (e.g. after a UI save) so changes go live.</summary>
        Task<AirySettings> RefreshAsync();
    }
}
