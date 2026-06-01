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
