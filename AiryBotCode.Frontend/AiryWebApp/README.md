# Airy Web App (Vue)

Vue 3 + TypeScript + Vite control panel for AiryBotCode. Replaces the earlier
Svelte mock UI and is wired to the real API (`AiryBotCode.Api`).

## What's here

- **Home** — Discord OAuth login + an API ping check.
- **Bot Settings** — loads bots from `GET /api/settings`, edits them in a
  grouped, component-based form, and saves via `PUT /api/settings/{botId}`.
  A saved change is picked up by a bot on its next settings load/restart.
- **Database** — placeholder browser on mock data (no data API yet).

## Running

```bash
npm install
npm run dev      # http://localhost:5173
npm run build    # type-check + production build
```

The API must be running (default `http://localhost:7215`) with `Database:*`
configured so the settings endpoints are active, and its CORS policy allows
`http://localhost:5173`.

## Config

Override the defaults with a `.env` file:

```
VITE_API_BASE_URL=http://localhost:7215
VITE_DISCORD_CLIENT_ID=1318870826862379018
```

## Notes

- Discord snowflake IDs are handled as **strings** end-to-end — they exceed
  JavaScript's safe-integer range, so the API serialises them as strings.
- The bot token is **write-only**: it is never returned by the API. Leave the
  token field blank when saving to keep the stored value.
