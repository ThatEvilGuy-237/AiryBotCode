import { ref } from 'vue'

const TOKEN_KEY = 'jwt'

// Host-scoped cookie helpers — the JWT is shared across every port of this host
// (login :443, Hive prod :10000, Hive dev :8443) via a host-only cookie (cookies
// ignore the port, unlike per-origin localStorage). localStorage is kept for
// same-origin back-compat; the cookie is read first.
function readCookie(name: string): string | null {
  const m = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'))
  return m ? decodeURIComponent(m[1]) : null
}
function decodeExp(jwt: string): number | null {
  try {
    const payload = JSON.parse(atob(jwt.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')))
    return typeof payload.exp === 'number' ? payload.exp : null
  } catch {
    return null
  }
}
function writeCookie(value: string): void {
  const exp = decodeExp(value)
  const secure = location.protocol === 'https:' ? '; secure' : ''
  const maxAge = exp ? `; max-age=${Math.max(0, exp - Math.floor(Date.now() / 1000))}` : ''
  document.cookie = `${TOKEN_KEY}=${encodeURIComponent(value)}; path=/; samesite=lax${secure}${maxAge}`
}
function clearCookie(): void {
  const secure = location.protocol === 'https:' ? '; secure' : ''
  document.cookie = `${TOKEN_KEY}=; path=/; max-age=0; samesite=lax${secure}`
}

// Reactive JWT shared across the app; seeded from the host cookie (cross-port),
// then localStorage.
export const token = ref<string | null>(readCookie(TOKEN_KEY) ?? localStorage.getItem(TOKEN_KEY))

export function setToken(value: string): void {
  token.value = value
  writeCookie(value)
  localStorage.setItem(TOKEN_KEY, value)
}

export function clearToken(): void {
  token.value = null
  clearCookie()
  localStorage.removeItem(TOKEN_KEY)
}

// Where the standalone login app lives (served by Caddy under /login).
export const LOGIN_URL = '/login/'

/** True only when the stored JWT exists and hasn't expired. */
export function isAuthenticated(): boolean {
  if (!token.value) return false
  const exp = decodeExp(token.value)
  return exp !== null && exp * 1000 > Date.now()
}

/** Drop any token and send the user back to the login app. */
export function redirectToLogin(): void {
  clearToken()
  window.location.replace(LOGIN_URL)
}

/**
 * The API redirects back to the SPA with `#token=<jwt>` after Discord login.
 * Pull it out of the URL fragment, store it, and clean the address bar.
 */
export function captureTokenFromHash(): void {
  const fragment = new URLSearchParams(window.location.hash.substring(1))
  const tokenFromRedirect = fragment.get('token')
  if (tokenFromRedirect) {
    setToken(tokenFromRedirect)
    history.replaceState(null, '', window.location.pathname + window.location.search)
  }
}
