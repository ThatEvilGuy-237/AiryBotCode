// Runtime configuration. Override via a .env file (VITE_API_BASE_URL,
// VITE_DISCORD_CLIENT_ID) without touching code; the fallbacks match the
// API's launchSettings (http://localhost:7215) and the existing Discord app.
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:7215'

// The Hive (Neural-Spine UI). Different origin → the token is handed over via
// the `#token=` fragment. Override with VITE_HIVE_URL.
export const HIVE_URL = import.meta.env.VITE_HIVE_URL ?? 'http://localhost:5175'

const DISCORD_CLIENT_ID = import.meta.env.VITE_DISCORD_CLIENT_ID ?? '1318870826862379018'
const DISCORD_SCOPE = 'identify'
const DISCORD_REDIRECT_URI = `${API_BASE_URL}/api/auth/discord/redirect`

export const DISCORD_AUTH_URL =
  'https://discord.com/api/oauth2/authorize' +
  `?client_id=${DISCORD_CLIENT_ID}` +
  `&redirect_uri=${encodeURIComponent(DISCORD_REDIRECT_URI)}` +
  '&response_type=code' +
  `&scope=${encodeURIComponent(DISCORD_SCOPE)}`
