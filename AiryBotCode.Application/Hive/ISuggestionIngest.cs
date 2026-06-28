namespace AiryBotCode.Application.Hive
{
    /// <summary>
    /// Writes an agent-filed suggestion (the Hive <c>submit_suggestion</c> effect) to the
    /// /suggestions board. Kept as an interface so HiveEffectListener's parse/route path
    /// stays testable without a database; the Infrastructure impl owns the repository + the
    /// cross-listener dedup (every bot's listener receives the same broadcast effect, so the
    /// row must be written exactly once — see the effectId guard).
    /// </summary>
    public interface ISuggestionIngest
    {
        Task SubmitAsync(SuggestionIntent intent, CancellationToken ct = default);
    }
}
