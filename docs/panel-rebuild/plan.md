---
title: Airy panel, rebuilt from zero on @airy/ui
status: R0 to R5 built 2026-10-15; R6 prepared, waits on the owner (tunnel route, Discord redirect, go)
---

# Airy panel, rebuilt from zero on @airy/ui

## The ask (owner, 2026-10-15)

"Improve the Airy page much more, make it one of the packages, but make it from zero." Then, on
decisions: use the same Hive connection for every channel, decide per channel whether a message is
worth answering, and pick channels by name instead of typing ids.

Answers to the two questions asked: the whole panel, with Airy's own look.

## What is there today (inventory 2026-10-15)

| Fact | Consequence |
|---|---|
| Since a1a913c (2026-10-06) Caddy :8088 serves only the Hive; `/`, `/login/` and `/api/*` redirect to `/hive/` | the panel, the login app and the API are reachable nowhere |
| Discord `RedirectUri` / `FrontendUri` still point at thehive.thatevilguy.com | Discord sign-in is broken even if the app comes back |
| The API registers its own services and misses the decision watch, counting and level repositories | Decision watches and Status answer 500 |
| The vendored `@hive/ui` is from June | newer props (password fields, field errors, busy buttons) are silently dropped |
| Editing a webhook without retyping its secret erases the secret | silent data loss |
| `/suggest/:code` is public on the server but the router guard sends visitors to `/login/` | the public board does not work |
| Each Hive agent link stores its own address and app key; "only when mentioned" is one switch per bot | no way to give channels different answer rules on one connection |
| Prod: one Hive key "Airy on Discord" (Cookie & Cream, scope `agents.run`), one channel linked to the Airy agent, three old links to the retired ts.net Funnel host | the key cannot make decisions; the old links are dead |
| Dead code: BotList, the old two-colour theme, mock data, `/hive-ui` demo, unused API calls | not carried over |

## Decisions

| # | Decision |
|---|---|
| D1 | Identity: company `airy` in evil-ui, exported as `@airy/ui` and installed from `vendor/airy-ui-<v>.tgz`. Sky `#38bdf8` on navy `#0f1722`, DM Sans for words, JetBrains Mono for Discord ids, radius 10, the cloud mark from `docs/brand/logo.svg`. Done (evil-ui 0d26c22). |
| D2 | One new app `AiryBotCode.Frontend/AiryPanel` replaces both `AiryWebApp` and `AiryLogin`: sign-in is a public route of the panel. The old apps are deleted when the new one serves prod. |
| D3 | One Hive connection per bot: the Hive address, the agent Airy speaks as, and one app key (write-only). The same key runs the agent and asks for decisions, so it needs `agents.run` and `decisions.make`. The panel checks the key and says which right is missing. |
| D4 | Channels are picked from the bot's servers by name (`/api/settings/{id}/discord/channels`), never typed as ids; every list shows names, ids only as a quiet second line. |
| D5 | Per channel, when Airy answers: **Always**, **When mentioned** (a mention or a reply to Airy), or **When it's worth it** (the Hive decides per message). A mention or reply is always answered. Airy answers in the same voice everywhere: the agent does not change per channel. |
| D6 | "Worth it" asks the Hive `POST /v1/decisions` with one yes/no question, text from a markdown template. Unsure counts as no. If the Hive cannot decide (no decider, key refused, down) Airy falls back to "When mentioned" for that message and the channel shows the reason. |
| D7 | Outside webhooks (send a channel's messages to another URL) stay, on the same Channels page as a second kind of link; a blank secret keeps the stored one. |
| D8 | The listener-based decision watches (react, alert, listener URL per watch) are removed from Airy: none exist on prod, and D5/D6 cover the ask. The Hive's listeners stay. |
| D9 | Reachable again on its own host, proposed `airy.thatevilguy.com` through the prod Caddy; the owner adds the tunnel route and the Discord redirect URL. |
| D10 | Installable, with the Update / Install pill (stack rule). |

## Pages

| Page | Job | Changes from today |
|---|---|---|
| Sign in | password gate, then Discord | moved into the panel |
| Home | how this bot is doing: online, channels Airy talks in, today's answers, what needs a look | was a session card and a bot list |
| Channels | the Hive connection, the channels Airy talks in with their answer rule, outside webhooks | replaces Webhooks and Decision watches |
| Commands | turn commands on and off, edit their settings with the right control per setting | errors no longer swallowed |
| Settings | name, token, log channels and admin roles picked by name, database | pickers instead of raw ids |
| Look | the bot's image, the colours taken from it, set as avatar | was Theme |
| Activity | counting game and level board | was Status; fixed 500 |
| Suggestions | the ideas board and its public link | public page fixed |
| Databases | Postgres databases and a read-only table explorer | kept; stays the last item |

## Rounds

| Round | Content | Done when |
|---|---|---|
| R0 ✅ | inventory, identity, `@airy/ui` 0.1.0, app scaffold, icons, this plan | |
| R1 ✅ | backend: bot Hive connection, Airy channels with answer rule, the worth-it decision, migration of the prod agent link, API DI fixed with a test that resolves every controller, webhook secret kept, decision watches removed | Airy tests green; on prod data the migration is a no-op twice |
| R2 ✅ | panel shell: sign-in, bot switcher, sidebar, top bar with Update/Install, PWA, Home | evidence at 1440/375/320 |
| R3 ✅ | Channels page | evidence, a live decision on dev |
| R4 ✅ | Commands, Settings, Look | evidence |
| R5 ✅ | Activity, Suggestions (+ public page), Databases | evidence |
| R6 | serving: Caddy host block, Discord redirect, CORS, old apps deleted, deploy | owner opens it on the phone |
| R7 | design review of the whole panel, owner marks it | |

## Deploy checklist (R6, one switch, owner chose 2026-10-16)

1. Owner: tunnel route `airy.thatevilguy.com` to `http://localhost:8088`; Discord redirect
   `https://airy.thatevilguy.com/api/auth/discord/redirect` on app 1318870826862379018; the go.
2. Back up `airy_db`; set `Discord:RedirectUri` and `Discord:FrontendUri` (`https://airy.thatevilguy.com/login`) in the
   mounted `AiryBotCode.Api/appsettings.json`.
3. `docker compose -f docker-compose.yml -p airybotcode up -d --build api airydevbot caddy` (Caddy restart: a few seconds
   for thehive.thatevilguy.com).
4. Check the bot log: tables ensured, "Moved 1 Hive agent link" (#airy-talk-🚧 becomes When mentioned, listens).
5. Add for Cookie&Creams (1437132076066406510), Listens and Answers on, rule When it's worth it:
   `1295105016675303424` #adult-general and `1341725917642227803` #nsfw-general (Cookies&Cream Audios).
6. Open the panel on the phone, sign in, check Home and Channels; drop the scratch database `airy_panel_dev`, stop the
   dev API and Vite, revoke the dev Hive key "Airy panel test (dev)".

## Waiting on the owner

- The tunnel route `airy.thatevilguy.com → http://localhost:8088` and the Discord redirect URL (R6).
- A key with `agents.run` and `decisions.make`, or the right to add `decisions.make` to "Airy on Discord" (R1/R3).
- A decider on prod Cookie & Cream: a Jev key or OpenAI credits (or a Claude decider on the plan, not built).
