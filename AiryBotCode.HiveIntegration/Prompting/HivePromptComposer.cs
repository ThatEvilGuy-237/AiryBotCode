using AiryBotCode.HiveIntegration.Contracts;

namespace AiryBotCode.HiveIntegration.Prompting;

// Assembles the per-turn system prompt for Mind.
//
// The base prompt is the operator's default (set in appsettings under
// Hive.SystemPrompt). We append a small per-turn block telling the agent
// who is currently speaking — Discord display names drift, but the speaker
// hint always reflects the *current* nickname. The user's canonical name
// lives in Chronos's [USER PROFILE] block which Mind injects automatically.
public sealed class HivePromptComposer
{
    public string Compose(string baseSystemPrompt, ulong discordUserId, string displayName)
    {
        var safeName = string.IsNullOrWhiteSpace(displayName) ? "(unknown)" : displayName.Trim();
        var speakerLine =
            $"The current speaker on Discord is \"{safeName}\" (Discord user id: {discordUserId}). " +
            "This is their *current* server display name — it can change. " +
            "Their canonical name (if they have stated one) lives in [USER PROFILE] and is the truth; " +
            "the display name above is only the handle to address them by right now.";

        if (string.IsNullOrWhiteSpace(baseSystemPrompt))
            return speakerLine;

        return baseSystemPrompt.TrimEnd() + "\n\n" + speakerLine;
    }
}
