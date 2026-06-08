import { ref } from 'vue'

const TOKEN_KEY = 'jwt'

// Reactive JWT shared across the app; seeded from localStorage on load.
export const token = ref<string | null>(localStorage.getItem(TOKEN_KEY))

export function setToken(value: string): void {
  token.value = value
  localStorage.setItem(TOKEN_KEY, value)
}

export function clearToken(): void {
  token.value = null
  localStorage.removeItem(TOKEN_KEY)
}

// Where the standalone login app lives (served by Caddy under /login).
export const LOGIN_URL = '/login/'

function decodeExp(jwt: string): number | null {
  try {
    const payload = JSON.parse(
      atob(jwt.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')),
    )
    return typeof payload.exp === 'number' ? payload.exp : null
  } catch {
    return null
  }
}

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
