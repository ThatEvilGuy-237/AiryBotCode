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

Two security layers, both via our own login app at **`/login`** (no browser
basic-auth popup):
1. **Access-key gate** — `POST /api/auth/gate` checks the shared password
   (`Panel:GatePassword` in `AiryBotCode.Api/appsettings.json`, constant-time
   compare) and returns a short-lived **gate token** (a JWT with a distinct
   audience `AiryBotCode-Gate`, 10-min expiry).
2. **Discord login** — the login app sends the gate token to Discord as OAuth
   `state`; the `/api/auth/discord/redirect` callback **rejects any login whose
   `state` isn't a valid gate token**, so the password step is enforced
   server-side. On success it issues the real JWT and the API checks an
   allowlist (`Discord:AllowedUserIds`).

The gate token's separate audience means it can NEVER be replayed as an access
token against `[Authorize]` endpoints. The main SPA's router guard bounces any
visitor without a valid JWT back to `/login`, and a 401 from `/api/*` does the
same.

## One-time setup you must do

1. **Discord Developer Portal** → your app (`1318870826862379018`) → OAuth2:
   - Copy the **Client Secret** into `AiryBotCode.Api/appsettings.json`
     (`Discord:ClientSecret`).
   - Add this **Redirect URL**: `https://thatevilguy.duckdns.org/api/auth/discord/redirect`
2. **Router**: forward TCP **80** and **443** to this host (`192.168.2.100`).
3. Confirm the allowlist (`Discord:AllowedUserIds`) holds the Discord user IDs
   allowed to log in (defaults to `405431299323461634`). Empty array = allow any.

## Credentials

- Access-key gate — plaintext password in `AiryBotCode.Api/appsettings.json`
  under `Panel:GatePassword`. Change it there, then `docker compose up -d --force-recreate api`.
  (The old Caddy basic-auth `.env` vars `PANEL_USER`/`PANEL_PASSWORD_HASH` are no
  longer used.)
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
