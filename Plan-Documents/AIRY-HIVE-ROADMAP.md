# Airy ⇄ Hive — feature roadmap

> **Goal:** make Airy (the Discord bot + control panel) work much richer with The
> Hive, expand the websocket/effect interaction model, sharpen the UX, and unify
> the panel on the Hive's shared UI library.

Status: ☐ todo · ◐ in progress · ☑ done (dev) · ✅ live (prod)

> **Workflow:** build on a branch off `channel-webhooks` (the current Airy⇄Hive
> integration branch), validate, commit. **AiryBotCode is a PUBLIC repo — never
> commit tokens / gate password / appsettings secrets** (they're gitignored; keep
> them so). One feature at a time; small validated commits.

---

## How Airy talks to the Hive today (context)

- **Discord → Hive:** the Airy bot forwards a Discord message to a **Hive flow
  webhook** — a signed HTTP POST, then **async + poll** for the reply (it shows a
  typing indicator while the flow runs, splits replies into ≤2000-char Discord
  messages, and forwards the Discord user id for per-user memory). This is the
  `/chat` page's path in the Hive portal, and the production Airy path.
- **Effect passthrough (already live in Hive prod):** an Obsidian in-process
  "dispatch" tool emits `{type:"tool_effect", call, context}` over the tools WS;
  Wraith fans it out to every `{type:"subscribe_effects"}` listener as
  `{type:"effect", call, context, at}` and acks the sender. First tool:
  `schedule_message`. **Nothing consumes these effects yet — Airy becoming a
  subscriber is the missing half (see A).**

---

## ◐ A. Multi-message / follow-up replies   *(extends the effect passthrough)*
Stop the rigid one-in-one-out. Airy gives the user an answer fast, then the agent
can **keep thinking and push follow-up messages**.
- ☑ **Hive producer (dev):** `say(message, delaySeconds?)` dispatch tool in
  Obsidian — emits a fire-and-forget `say` effect over the tools WS and the agent
  loop **continues** (so: quick first answer, then follow-ups, vs one reply at
  `finish`). Goes to the current conversation — **no channel arg needed**: for an
  Airy webhook flow the run's `sessionId` **is** the Discord channel id
  (`FlowRunner.cs:51` `sessionId = channelId ?? …`), and the effect context already
  carries `sessionId`+`userId`. `delaySeconds` default 2, clamped 0..60. 6/6 bun
  tests; committed to Hive `dev`.
- ☐ **Airy consumer:** subscribe to Hive effects (`subscribe_effects` on the tools
  WS) and post each `say`/`schedule_message` effect to the channel = `context
  .sessionId`. Pure effect→delivery router (testable) + a hosted listener +
  reconnect.
- ☐ **Pacing:** **2s** default before a follow-up, **measured from when the
  previous message was actually sent** (Airy-side, per-channel queue), AI can
  extend via `delaySeconds`.
- ☐ **Run lifecycle / reply contract (the fork):** with `say`, the agent emits
  user messages mid-run; decide whether the webhook HTTP reply becomes an ack
  (Airy posts only effects) or stays the first message — and avoid double-posting.

## ☐ B. Images over the socket (read images)
Let Airy forward Discord image attachments into the flow so the agent can read
them. **Mostly transport, not a new tool** — the Hive side largely exists:
Chronos has the 24h image store; Obsidian has `view_image` + `recall_image`.
- Missing: Airy forwards attachment urls/bytes on the webhook; the flow/agent
  ingests them (the existing image intake path) and can `view_image`.

## ☑ C. Discord mention resolution (dev — committed locally)
A raw `@124654654` in a message means nothing to the AI or memory today.
- **In:** ✅ `MentionResolver.Rewrite` (pure, Discord-free) turns `<@id>` / `<@!id>`
  / bare `@<snowflake>` → `DisplayName (id)`; `MessageSendHandler` supplies the
  name via `message.MentionedUsers` + a `guild.GetUser(id)` fallback, applied
  before `WebhookChatService.TryForwardAsync`. Role mentions (`<@&id>`),
  `@everyone`/`@here`, and short `@5` tokens are left alone.
- **Out:** ✅ no change needed — the reply path sends plain content with default
  AllowedMentions, so an agent reply containing `<@id>` pings naturally.
- Validation: new `AiryBotCode.Tests` xunit project — **8/8 green**. (Live Discord
  round-trip pending a real mention; dev bot not yet rebuilt.)
- Small, high-value, **no Hive changes** — lives in the Airy webhook forwarder.

## ☐ D. First-message consent gate
The first time a user talks to Airy, prompt them to accept data collection
**before** anything is stored.
- Copy: data is collected **only to improve their experience with Airy**, **not
  used for any AI training**; for questions, ping `<@405431299323461634>` (evil).
- Consent state in Airy's DB; **gate storage until accepted**.
- **Design fork:** a Discord **ephemeral message + Accept button** (lean toward
  this — first contact happens in Discord) vs a panel popup.

## ☐ E. Shared UI + per-instance theming   *(stated priority)*
Replace Airy's "vibe-coded" panel UI with the Hive's shared component library.
- **Share `@hive/ui`** (the Hive's `Aether-Sight/src` lib) with Airy's panel
  (`AiryBotCode.Frontend/AiryWebApp`, Vue 3). Same machine → a workspace / path
  link or a built local package. Rebuild the panel's pages on the shared
  components + theme tokens.
- **Image-driven theme:** upload an image → derive the theme from its dominant
  colour into the lib's existing oklch tokens (`--accent-hue` / `--base-hue`, …),
  so each Airy instance can look distinct.
- That **same uploaded image becomes the panel profile image** (panel only — NOT
  the Discord avatar).
- Biggest lift; **own track**, largely independent of A–D.

---

## Suggested sequence
C (fast, independent) → A (the deep one, reuses the most existing infra) → B
(small, on top of A's transport) → D (privacy gate) → E (own track; start in
parallel whenever, it's the priority but the largest). Re-order freely.

## Open decisions to settle when each is picked up
- **A:** run lifecycle for multi-message; HTTP-reply vs effect-only delivery.
- **D:** Discord ephemeral-button vs panel popup for the consent prompt.
- **E:** how to consume `@hive/ui` (path-link the source vs build a local package);
  where the uploaded image + derived theme are stored (Airy DB / per-bot row).
