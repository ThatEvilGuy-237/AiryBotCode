# Vinnie — Slack bot on the Airy/Hive stack

A separate Slack identity ("Vinnie") that chats backed by the same Hive brain as the Discord bot.
v1 is conversational: Slack message → the **Vinnie** Spine agent (`a869d850-2f85-4d57-8fbb-fd20774d90dd`,
37 tools, gpt-4o-mini) → reply. Memory is automatic (Chronos, keyed by Slack thread/channel + user).

Connection: **Socket Mode** (outbound WebSocket — no public inbound endpoint, nothing to expose through
Cloudflare/Tailscale).

## 1. Create the Slack app (one-time, you)

At <https://api.slack.com/apps> → **Create New App → From an app manifest**, paste this:

```yaml
display_information:
  name: Vinnie
  description: AI assistant backed by the Hive
features:
  bot_user:
    display_name: Vinnie
    always_online: true
oauth_config:
  scopes:
    bot:
      - app_mentions:read
      - chat:write
      - channels:history
      - groups:history
      - im:history
      - im:read
      - mpim:history
      - users:read
      - files:read
settings:
  socket_mode_enabled: true
  event_subscriptions:
    bot_events:
      - app_mention
      - message.im
      # add message.channels if Vinnie should also read non-mention channel messages
```

Then:
- **Socket Mode → Enable**, and generate an **App-Level Token** with scope `connections:write` → `xapp-…`
- **Install App** to the workspace → copy the **Bot User OAuth Token** → `xoxb-…`
- Invite Vinnie to a channel (`/invite @Vinnie`) or just DM it.

## 2. Run it

Locally:
```bash
cp appsettings.example.json appsettings.json   # fill BotToken / AppToken / Jwt.Secret
dotnet run
```

In the Hive stack (reaches `spine:7000`):
```bash
SLACK_BOT_TOKEN=xoxb-… SLACK_APP_TOKEN=xapp-… AIRY_JWT_SECRET=<shared secret> \
  docker compose -f docker-compose.slack.yml -p vinnie up -d --build
```

## Config
| key | env | meaning |
|---|---|---|
| `Slack:BotToken` | `Slack__BotToken` / `SLACK_BOT_TOKEN`* | `xoxb-…` |
| `Slack:AppToken` | `Slack__AppToken` / `SLACK_APP_TOKEN`* | `xapp-…` (Socket Mode) |
| `Spine:Url` | `Spine__Url` | default `http://spine:7000` |
| `Vinnie:AgentId` | `Vinnie__AgentId` | the Vinnie agent id |
| `Jwt:Secret` | `Jwt__Secret` | shared AiryBotCode JWT secret |

\* the compose file maps `SLACK_BOT_TOKEN`→`Slack__BotToken` etc.

## Status / roadmap
- **Phase 1 (this):** conversational — DMs + @mentions, in-thread replies, chunked. Built & compiles.
- **Phase 2 (later):** subscribe to the Wraith effects WS for proactive `say` / `ask_user` (Block Kit
  buttons), reusing `AiryBotCode.Application.Hive.HiveEffectListener` with a Slack-ID filter; images.
- **Tools:** Vinnie has 37 tools (browser, chronos, memory, math). The 9 `sandbox_*` tools (arbitrary
  shell) were intentionally held back — add them to the agent only if the boss is fully trusted.
