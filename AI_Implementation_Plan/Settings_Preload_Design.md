# Settings Preload Provider — Design Note

Goal: make `BotSetting` the single source of truth that the UI edits and the
bot consumes, **preloaded once at startup** before any interaction logic runs,
so commands stop hardcoding Discord IDs / env values.

The round-trip we want:

```
BotSettings.svelte  →  Api  →  DB (BotSetting)  →  preloaded cache  →  interactions
        ▲                                                                     │
        └─────────────────────────  reads back  ◄────────────────────────────┘
```

## What already exists (don't rebuild)

- `Domain/database/BotSetting.cs` — entity (BotId, OpenAIModel, OpenAIPrompt,
  Token, AdminRoleIds, EvilId, LogChannelId, EvilLogChannelId)
- `Infrastructure/.../BotSettingRepository.cs` + `BotSettingsSeeder.cs` — load/seed
- `Frontend/.../routes/BotSettings.svelte` + `types/database.ts` — UI form bound
  to those exact fields
- `Application/ConfigurationReader.cs` — startup config load

## What's missing / to build

1. **Extend the settings model** to cover values currently hardcoded in handlers.
   Today `BotSetting` has 3 channel IDs; interactions reference more:
   - verified / unverified role IDs (`VerifyUserAgeCommand.cs:34-36`)
   - log channel (`VerifyUserAgeCommand.cs:36`)
   - contact category ID (`ContactUserCommand.cs:45`)
   - giveaway scoreboard channel (`GiveawayCommand.cs:21`)
   - Airy listen channels — 5 IDs (`TalkToAiry.cs:26`)
   - rules channel (`VerifyFrontend.cs:12`)
   - owner/admin user id `405431299323461634` (`GiveawayCommand.cs:161,183`)
   - AI defaults: model, max_tokens, retries (`OpenAIClient.cs`, `OllamaClient.cs`)

   Note: many of these are per-guild. Consider a `GuildConfiguration` table
   keyed by guild id rather than overloading `BotSetting`.

2. **A preloaded settings provider** — singleton, populated at startup (after
   seeding, before bots start handling events). Holds the resolved values in
   memory; exposes typed accessors. One refresh path so a UI save can invalidate.

3. **Swap literals for lookups** — replace hardcoded `ulong` literals in the
   handlers above with provider accessors.

4. **Standardize loading** — remove the lazy `Environment.GetEnvironmentVariable
   ("ADMINROLEID")` in `UserService.cs:46`; route it through the provider.

## Sync risk to address

Frontend field metadata is hand-maintained in `schema.ts` + `database.ts` and
must stay aligned with the C# `BotSetting`. No shared source today — they can
drift. Out of scope for first pass, but worth a follow-up.

## Order of work

1. Decide model shape (extend `BotSetting` vs new `GuildConfiguration`).
2. Build the preload provider + DI registration + startup hook.
3. Migrate handlers one at a time off literals.
4. Wire the Api endpoint so the Svelte page actually persists (ties into the
   Api/Frontend gap from the main review).
