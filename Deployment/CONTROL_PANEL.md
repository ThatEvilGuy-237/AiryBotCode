# Airy Control Panel — secure web access

Public, password-gated, Discord-login control panel served over HTTPS at
**https://thatevilguy.duckdns.org**.

## Architecture

```
Internet → router :443 → Caddy (docker)
                          ├─ HTTPS auto-cert via DuckDNS DNS-01 (token in .env)
                          ├─ basic-auth password gate (SPA routes only)
                          ├─  /        → Svelte SPA (built into the image, /srv)
                          └─  /api/*    → api:8080 (ASP.NET) → db:5432 (Postgres)
                                          └─ Discord OAuth → JWT → [Authorize]
```

Two security layers:
1. **Password gate** (HTTP basic-auth) — required to load the panel at all.
2. **Discord login** — OAuth issues a JWT; the API requires it and checks an
   allowlist (`Discord:AllowedUserIds` in `AiryBotCode.Api/appsettings.json`).

`/api/*` is intentionally NOT behind basic-auth (it carries the JWT in the
`Authorization` header, which would clash with basic-auth). It is protected by
the JWT requirement + allowlist instead.

## One-time setup you must do

1. **Discord Developer Portal** → your app (`1318870826862379018`) → OAuth2:
   - Copy the **Client Secret** into `AiryBotCode.Api/appsettings.json`
     (`Discord:ClientSecret`).
   - Add this **Redirect URL**: `https://thatevilguy.duckdns.org/api/auth/discord/redirect`
2. **Router**: forward TCP **80** and **443** to this host (`192.168.2.100`).
3. Confirm the allowlist (`Discord:AllowedUserIds`) holds the Discord user IDs
   allowed to log in (defaults to `405431299323461634`). Empty array = allow any.

## Credentials

- Panel password gate — user `airy`, password in `.env` (`PANEL_PASSWORD_HASH`
  is the bcrypt hash; plaintext noted in the `.env` comment).
  Change it: `docker run --rm caddy:2.8-alpine caddy hash-password --plaintext 'NEW'`
  then update `.env` and `docker compose up -d caddy`.
- JWT signing secret — `Jwt:Secret` (shared by API + bot configs).
- DuckDNS token — `.env` `DUCKDNS_TOKEN`.

Secrets live in gitignored files: `.env`, `AiryBotCode.Api/appsettings.json`,
`Bots/AiryBotCode.AiryDevBot/appsettings.json`.

## Run / operate

```sh
# Build + start the web stack (db is shared with the bot)
docker compose up -d --build api caddy

# Logs
docker compose logs -f caddy     # TLS issuance / requests
docker compose logs -f api       # API

# After editing AiryBotCode.Api/appsettings.json (e.g. Discord secret)
docker compose up -d api         # appsettings.json is mounted; restart picks it up

# Stop
docker compose stop api caddy
```

## Notes

- Caddy obtains its certificate via the DuckDNS DNS-01 challenge, so issuance
  works even before the router ports are forwarded — but the site is only
  reachable from the internet once 80/443 are forwarded.
- The frontend's Discord client id / redirect URI are baked at build time
  (defaults target this domain). Override with `VITE_DISCORD_CLIENT_ID` /
  `VITE_DISCORD_REDIRECT_URI` / `VITE_API_BASE` if the domain changes, then
  rebuild caddy.
