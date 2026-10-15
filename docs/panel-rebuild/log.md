# Airy panel rebuild log

Newest first. Plan: `plan.md`.

## 2026-10-15 · Two switches per channel: Listens and Answers

Owner: while listening is still being worked on, each channel gets a switch to stop listening, and one for
answering. `AiryChannels.Listens` (default on, added at start like the other columns). Listens off: the bot stops
logging that channel and forgets what it had; answers then carry only the message itself. Answers off: Airy stays
quiet there (no decisions, no runs) and, while Listens is on, keeps reading along. Both off: the channel is off.
Panel rows show both switches with Undo; Home shows "Listening only" or "Off". Tests 151 of 151; Channels axe 0,
phone probe 0.

## 2026-10-15 · Airy listens in its channels

Owner: the decision only decides whether to answer; Airy should keep listening so that when it answers it has the
messages from above. The bot keeps a log per Airy channel (`ChannelTranscript`, last 30 lines from people who
accepted the data notice plus Airy's own lines, memory only, empty after a restart). The worth-it decision reads its
last 6 lines (before this the decision's context was always empty: the bot keeps no Discord message cache). When
Airy answers, the agent gets what was said since Airy last spoke (at most 20 lines, 4000 characters) above the new
message, from the template `Application/Hive/Texts/channel-talk.md`; that also lands in the Hive's conversation
history. The Channels page says so in one line. Tests 150 of 150.

## 2026-10-15 · R4 Commands, Settings, Look; R5 Activity, Suggestions, Databases; R6 serving prepared

- Pages: Commands (one-tap on/off with Undo, a drawer editor with the right control per setting kind, restart),
  Settings (pickers by name, token write-only with reveal behind the access password, add and remove a bot, a save
  bar that appears only with changes), Look (picture, message colours from it, use as Discord avatar), Activity
  (counting per channel, level board), Suggestions (waiting first, approve or not now, public link), the public
  ideas board `/suggest/:code` (no sign-in), Databases (list, make one, read-only table view).
- A searchable channel picker by name with the server beside it, used everywhere one channel is chosen.
- Kit (evil-ui, airy 0.1.4 to 0.1.11): Drawer named and focused, Autocomplete `badgeShows` and first results on
  focus, scrim token, EmptyState padding on the scale, Airy phone sizes (switch hit area, search, picker, copy,
  breadcrumbs, 48 px text fields), field hints in the text face.
- API: theme request fields optional (a look without the old panel's theme data was refused), readable error texts.
- Reviews: `docs/review/r3.md`, `r4.md` (R4 Blocker was a misread: the test uploaded a new picture by script;
  High sticky bar and Mediums fixed).
- R6 prepared: Caddy builds `AiryPanel` into `/srv`; host `airy.thatevilguy.com` serves it and proxies `/api`
  (tested in a throwaway container on both networks; the Hive host unchanged); the old `AiryWebApp` and
  `AiryLogin` are removed. Not deployed: waits for the tunnel route, the Discord redirect and the owner's go.
- Evidence of member data (Activity, Suggestions, public board, tables) stays in the git-ignored
  `docs/screenshots/private/`.

## 2026-10-15 · R2 shell and Home, R3 Channels

- Panel `AiryBotCode.Frontend/AiryPanel` on `@airy/ui` 0.1.3: sign-in (password, then Discord) as a public route,
  sidebar with the bot switcher, phone menu sliding the same sidebar in, Update/Install pill, PWA files, Home (bot,
  needs a look grouped per page, stats, where Airy talks), Channels (Hive connection with a check and Try a message,
  channels picked by name with one-tap rules and Undo, webhooks). Briefs `briefs/r2-shell-home.md`,
  `briefs/r3-channels.md` carry the measurements; R2 review in `docs/review/r2.md` (all findings closed).
- Kit: Airy ghost text lifted, phone sizes in Airy overrides; base ConfirmDialog uses Button, Modal has no landmarks.
- API: Discord lookups shared between callers and retried once on a rate limit, names reported unknown instead of
  gone; `hive/try`; the Hive connection save now stores (148 tests).
- Dev loop: `scripts/panel-dev-api.sh` (127.0.0.1:8202, scratch database `airy_panel_dev`), Vite on 172.17.0.1:5173.
  Dev Hive key "Airy panel test (dev)" (agents.run, decisions.make, Evil helper) for live checks; revoke when the
  rebuild ships.

## 2026-10-15 · R1 backend: one Hive connection, channels with an answer rule

- Bot settings carry the Hive connection (`HiveUrl`, `HiveAgentId`, `HiveAppKey`); table `AiryChannels`
  holds the channels Airy talks in with their rule (`always`, `mention`, `decide`) and the last decision.
- A "When it's worth it" channel asks the Hive `POST /v1/decisions` (question text in
  `Application/Hive/Texts/worth-answering.md`, last 6 cached messages as context) only for messages that
  do not mention Airy, from people who accepted the data notice; a sure yes answers, anything else stays
  quiet; a mention or reply is always answered.
- At start, agent-mode links move into the connection and `AiryChannels` (rule from the old
  RequireMention switch), then the link rows go. Rehearsed on a copy of prod's airy_db: first start moved
  1 link of Cookie&Creams (channel becomes `mention`, as RequireMention was on), second start moved
  nothing; the 3 sync webhooks stayed; `ChannelDecisionWatches` dropped.
- API: `api/channels`, `api/settings/{id}/hive` (get, put, delete, `agents`, `check`); service setup in
  `PanelServices` with the counting, level and channel repositories the API was missing (Status and
  Decision watches answered 500); a test builds every controller from it and fails on the old gap.
- Webhook edits keep the stored secret when it is left blank; webhooks are sync or async only.
- Decision watches (listener per watch) removed.
- Tests: 145 of 145.
- Not deployed yet: ships with the panel (R6).

## 2026-10-15 · R0 inventory and identity

Inventory of the old panel (findings in `plan.md`). Company `airy` in evil-ui (0d26c22): `@airy/ui` 0.1.0,
sky on navy, DM Sans and JetBrains Mono, cloud BrandMark. New app `AiryBotCode.Frontend/AiryPanel` with the
package and icons from `docs/brand/logo.svg`.
