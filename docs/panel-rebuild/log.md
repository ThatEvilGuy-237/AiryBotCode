# Airy panel rebuild log

Newest first. Plan: `plan.md`.

## 2026-10-30 · G13: Evil SDK 0.1.4 (Discord into a browser session, visitor address by network)

- `Evil.Sdk` and `Evil.Sdk.Telemetry` 0.1.4. The API trusts `CF-Connecting-IP` from any peer on this project's default
  network (`Proxies:Networks`, the subnet `airy-deploy.sh` reads from `docker network inspect airybotcode_default`), so a
  recreated Caddy at a new address keeps every visitor in their own throttle bucket; the app-level
  `ForwardedHeadersOptions` patch and the two Caddy addresses are gone. Measured on dev: Caddy recreated from 172.25.0.4
  to .5, the per-address lockout check still passed.
- Discord sign-in, still off in prod (`Discord__SignIn=false`) until the owner adds
  `https://airy.thatevilguy.com/api/auth/external/discord/callback` in the Discord developer portal: the button now
  starts `auth/external/discord/start?mode=browser&next=/login?next=<page>`; the SDK's callback opens the browser session
  and comes back to `/login`, which goes on to the page; a refusal comes back as `/login?...&error=<reason>` and the
  sign-in page says why in words (an Alert above the form). Going on: set `Discord__SignIn=true` in compose and deploy.
- The bot calls `EvilTelemetry.Start` with an `EvilTelemetrySettings` record (no lambda). Its image built once under a
  throwaway tag (removed); the bot was not started.
- Checks: panel tests 36 / 36 (Discord start link, refusal messages), vue-tsc clean, `dotnet test` 242 / 242;
  dev `scripts/auth-check.mjs` 46 / 46 (twice, the second after recreating the dev Caddy);
  `scripts/discord-browser-check.mjs` 11 / 11 on dev with Discord on and a dummy client (302 to Discord with scope
  identify, PKCE and the callback under the public URL; refusals as redirects with `error`; bad `next` refused);
  shots `docs/screenshots/g13/` at 375, 1440 and 320: probe 0 at 375 and 320, at 1440 the kit's desktop field and
  button heights and 15 px field text (touch rules, not desktop defects, as in L6), axe 0 at all three.
- Prod: `./scripts/airy-deploy.sh prod` 16:33 to 16:35 UTC, api recreated alone, Caddy not restarted (up since
  2026-10-16), the bot untouched (exited since 2026-10-22). `scripts/prod-check.mjs` 18 / 18 (synthetic person made and
  deleted, the forwarded client address seen by the API, not Caddy's).

## 2026-10-30 · L6: sign-in on the Evil SDK

Rollout loop L6 of `evil-sdk-research/07-rollout-design.md`. The access-password gate, the Discord-only sign-in, the
24 h JWT and the device pairing are gone. The panel API runs on `Evil.Sdk` 0.1.2 from the feed
(`AddEvil("airybotcode", "api")`: users, sessions as browser cookies, audit, proxies, telemetry) under the path base
`/api`, so Caddy's `/api/*` route stays as it is. Sign-in is the Evil account (user name or email, password); only
people with an `airybotcode` grant get in; the database explorer keeps its A5 limits and is the owner's only;
showing a stored bot token needs a sign-in in the last 5 minutes (the panel asks for the password then). Discord
sign-in is wired (`Discord:SignIn`) and off. Everything moved to .NET 10 (EF Core 10, Npgsql 10); the vendored
`Evil.Log.Telemetry` left for `Evil.Sdk.Telemetry` (the bot too). The API connects to `airy_db` with its own role
`airybotcode_app` (data rights on `public` only, refused by pg_hba in every other database), made by
`scripts/airy-db-role.py`.

- Tests: 242 of 242 (C#, net10; one removed with the old Discord controller), panel 34 of 34, vue-tsc clean.
- Dev stack (`./scripts/airy-deploy.sh dev`, a scrubbed copy of prod's `airy_db` public schema, `evil_users_dev`):
  `scripts/auth-check.mjs` 46 of 46; route walk `scripts/api-walk.mjs`, the pre-L6 image against the new one on the
  same copy, 55 calls: 55 same status, 52 same body (3 differ only in trace ids).
- Import dry run on dev: 1 row (the owner by `BotSettings.EvilId`), grant and Discord login added to the existing dev
  owner, rerun "already imported".
- Browser: sign-in through the form (wrong, then right, lands on `next`), the token prompt after a stale sign-in, sign
  out; a member has no Databases entry. Shots in `docs/screenshots/l6-evil-sdk/`; sign-in page 0 / 0 / 5 probe
  findings at 320 / 375 / 1440 (all five the kit's desktop field size, older than L6), axe 0. Design review: 0
  Blocker, 0 High, 1 Medium (enabled and focus states missing from the shots, added), 1 Low (desktop field size).
- Prod (a22aadb, 12:50 to 12:53 UTC, `./scripts/airy-deploy.sh prod`): backups first (`airy_db`, `evil_users`, the API
  settings file, the running panel files, pg_hba, in `AiryBotCode-data/backups/pre-l6`; images `:pre-l6`); role
  `airybotcode_app` made (reads the 11 public tables, refused on `spine`, DDL, `evil_users`, `dnd_db`); import 1 row
  (the owner's grant with the owner flag and the Discord login; every `BotSettings.EvilId`, 3 of 3, resolves to the
  owner); only the api container was recreated, the panel files copied into the running Caddy (up 13 days, not
  restarted), the bot and Postgres untouched. `scripts/prod-check.mjs` through the local Caddy entry
  (`airy.thatevilguy.com` has no public DNS record yet: the tunnel route is still the owner's): 18 of 18. Sign-in
  page on prod 0 / 0 / 5 at 320 / 375 / 1440 (desktop field size), axe 0, CLS 0. The API's settings file keeps
  only the Discord client and log levels; the gate password, JWT secret and superuser connection left it.

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
