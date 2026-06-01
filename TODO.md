# AiryBotCode — TODO

Audit of "good setups that still need work." Ordered by priority.
Last reviewed: 2026-06-01.

## 🔴 Do first — security / production-hardening

- [ ] **Re-enable + centralize permission guards** (currently commented out, anyone can run mod commands)
  - `AiryBotCode.Application/Comands/SlashCommands/ContactUserCommand.cs:31-34`
  - `AiryBotCode.Application/Comands/SlashCommands/TimeoutCommand.cs:58-62`
  - `AiryBotCode.Application/Comands/SlashCommands/UntimeoutCommand.cs:51-54`
  - `AiryBotCode.Application/Comands/SlashCommands/VerifyUserAgeCommand.cs:41-44`
- [ ] **Switch from `EnsureCreated()` to EF Core migrations** (`AIDbContext.cs:70`) — no schema history/versioning today
- [ ] **Add a test project** — zero tests exist; Application services are the most testable, start there

## 🟡 Good setup, blocked by a clear gap

- [ ] **Consolidate the 4 bots onto a shared base in `Bot.Core`** — `Bot.Core` only has `IBot.cs`; each bot has a copy-pasted `Bot.cs` with `StartAsync`/`StopAsync` throwing `NotImplementedException`
  - `Bots/AiryBotCode.AiryBot/Bots/Bot.cs:59,65`
  - `Bots/AiryBotCode.AiryDevBot/Bots/Bot.cs:60,66`
  - `Bots/AiryBotCode.AiryDevSecondBot/Bots/Bot.cs:63,69`
  - `Bots/AiryBotCode.AiryGuardian/Bots/Bot.cs:61,67`
- [ ] **Connect Frontend to a real backend** — Svelte UI is built but runs 100% on `AiryWebpage/.../mock.ts`; no API calls. BotSettings page is a stub.
- [ ] **Add data/CRUD endpoints to the Api** — only the Discord OAuth2 + JWT flow exists (`AuthController.cs`); Frontend has nothing to call. (Pairs with the Frontend item above.)

## 🟢 Finish-the-last-mile loose ends

- [ ] **Finish `UserlogsAction`** — throws `NotImplementedException` mid-method (`Infrastructure/Activitys/UserlogsAction.cs:70`)
- [ ] **Rework `MessageSendHandler`** for different message types (`Infrastructure/DiscordEvents/MessageSendHandler.cs:19`)
- [ ] **Re-enable `JoinServerHandler`** — entire file (~46 lines) is commented out
- [ ] **Wire up Discord response/defer handling** — `//TODO: [READ ABOUT RESPONSES TO USER]`
  - `Infrastructure/DiscordEvents/SlashCommandHandler.cs:79`
  - `Infrastructure/DiscordEvents/BanHandler.cs:30`
- [ ] **Validate the OpenAI token** — `// Validate the token :TODO` (`Application/ConfigurationReader.cs:130`)
- [ ] **Pull hardcoded channel IDs into config** (e.g. `LogService.cs:49`)
- [ ] **Make `EvilCommand.GetCommand()` abstract** instead of a virtual that throws (`Application/Comands/EvilCommand.cs:18`)
- [ ] **Re-enable bot presence** — `SetStatusAsync(UserStatus.Online)` commented out in all 4 bots

## 📄 Docs

- [ ] **Update README** — doesn't mention `AiryBotCode.Api`, `AiryBotCode.Frontend` (Svelte), or `Bot.Core`; only describes the 4-layer bot
