// Runtime config. By default the API is reached SAME-ORIGIN (window.location.origin),
// so the gate call + Discord redirect target whatever host the panel was loaded
// from — works on any public hostname without a rebuild. Override with
// VITE_API_BASE_URL to pin an absolute base; dev falls back to the local API port.
export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ||
  (import.meta.env.DEV ? 'http://localhost:7215' : window.location.origin)

const DISCORD_CLIENT_ID = import.meta.env.VITE_DISCORD_CLIENT_ID ?? '1318870826862379018'
const DISCORD_SCOPE = 'identify'
const DISCORD_REDIRECT_URI = `${API_BASE_URL}/api/auth/discord/redirect`

// Where to land once a valid session exists (the main control panel, root SPA).
export const APP_URL = '/'

// The Hive (Neural-Spine UI). The token is passed via `#token=` since it's a
// different origin. Override with VITE_HIVE_URL.
export const HIVE_URL = import.meta.env.VITE_HIVE_URL ?? '/hive'

/**
 * Build the Discord OAuth URL, carrying the gate token in `state` so the API
 * callback can verify the password step was completed before issuing a JWT.
 */
export function discordAuthUrl(gateToken: string): string {
  return (
    'https://discord.com/api/oauth2/authorize' +
    `?client_id=${DISCORD_CLIENT_ID}` +
    `&redirect_uri=${encodeURIComponent(DISCORD_REDIRECT_URI)}` +
    '&response_type=code' +
    `&scope=${encodeURIComponent(DISCORD_SCOPE)}` +
    `&state=${encodeURIComponent(gateToken)}`
  )
}
