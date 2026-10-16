# Airy panel rebuild log

Newest first. Plan: `plan.md`.

## 2026-10-16 · Worth it = the conversation, not one line

Owner: the decision should look at several messages. A "When it's worth it" channel no longer asks per message:
the bot waits until the room is quiet for 20 s (or 8 messages, or 60 s at most), then asks once whether Airy
should join the talk since she last spoke (up to 12 lines). A yes lets her join in with the stretch as context
("Nobody pinged you; you chose to join in."), then she stays out uninvited for 10 minutes per channel. A ping or
reply cancels the wait and is answered at once. Question text in `Texts/worth-answering.md`, tuned on six
stretches (fox teasing, chatter, a question to the room, trance talk, named without a ping, private talk): all
as intended on GPT-6 Luna. Also in this round: emotes, stickers, GIF and image links reach her as pictures, the
writer's display name, her own name from the server ("Airy", was "Airy bot").

Open, owner's call: only consented people are heard, so other people's lines (a laugh, a question) are missing
from her context.

## 2026-10-16 · One rule for every channel

Owner: a ping always gets an answer, in every channel; anything else only when it is worth it. That is the
"When it's worth it" rule (a mention or reply skips the decision and answers), so #airy-talk-🚧 moved from When
mentioned to When it's worth it on prod; all four Cookie&Creams channels now share it.

## 2026-10-16 · Panel deployed; waits only on the tunnel route

Owner: deploy now. Backup `~/backups/airy-2026-10-16-panel` (airy_db and the API settings), `Discord:FrontendUri` set
to `https://airy.thatevilguy.com/login` (RedirectUri stays on thehive, already registered), Caddy image rebuilt from
main fa31686 and recreated (thehive answered again within 20 s), API restarted. Checked through the live Caddy: the
airy host serves the panel (deep links, `/login`, `version.json`, `sw.js`) and `/api` (401 unsigned); the sign-in
start returns the Discord address with the thehive callback; thehive keeps `/hive/` (200), Spine ping (200) and `/`
to `/hive/`; from outside, thehive's `/hive/` is 200 and the callback reaches the API. `airy.thatevilguy.com` has no
DNS until the owner adds the tunnel's public hostname.

## 2026-10-16 · Bot and API on prod with the owner's channels (panel exposure later)

Owner: the port does not matter now; make the setup and data work, expose the panel later. Backed up airy_db
(`~/backups/airy-2026-10-16-predeploy`), rebuilt only `api` and `airydevbot` (main 97bc6ae; Caddy untouched). The bot
made the tables and moved the agent link: #airy-talk-🚧 (When mentioned, listens) on one Hive connection
(http://spine:7000, agent Airy); the API's simultaneous move hit the unique index and rolled back, no duplicate.
Added through the prod API for Cookie&Creams, listening and answering When it's worth it: #adult-general
1295105016675303424, #nsfw-general 1341725917642227803, #adult-memes 1327630989547012137. The API container was
not on the Hive network, so its check could not reach Spine; it joins `hive` now (20caa75). Prod check: reachable,
runs the agent Airy, cannot decide (the key lacks decisions.make), so the worth-it channels answer when mentioned.
Sign-in now takes the Discord address from the API's settings, and Caddy will route the already registered callback
on thehive.thatevilguy.com to the API at its next restart, so only the tunnel route is needed later.

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
