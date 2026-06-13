// Shared JWT (key `jwt`). Stored in a HOST-scoped cookie so it's shared across
// every port of this host (login :443, Hive prod :10000, Hive dev :8443) from a
// single login — cookies ignore the port, unlike localStorage (per-origin).
// localStorage is still written for same-origin back-compat.
const TOKEN_KEY = 'jwt'

function decodeExp(token: string): number | null {
  try {
    const payload = JSON.parse(
      atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')),
    )
    return typeof payload.exp === 'number' ? payload.exp : null
  } catch {
    return null
  }
}

function readCookie(name: string): string | null {
  const m = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'))
  return m ? decodeURIComponent(m[1]) : null
}

/** Host-only cookie (no Domain attr) → sent to + readable on all ports of this host. */
export function writeTokenCookie(token: string): void {
  const exp = decodeExp(token)
  const secure = location.protocol === 'https:' ? '; secure' : ''
  const maxAge = exp ? `; max-age=${Math.max(0, exp - Math.floor(Date.now() / 1000))}` : ''
  document.cookie = `${TOKEN_KEY}=${encodeURIComponent(token)}; path=/; samesite=lax${secure}${maxAge}`
}

export function getToken(): string | null {
  return readCookie(TOKEN_KEY) ?? localStorage.getItem(TOKEN_KEY)
}

/** True only when a non-expired JWT is already stored (cookie or localStorage). */
export function isAuthenticated(): boolean {
  const token = getToken()
  if (!token) return false
  const exp = decodeExp(token)
  return exp !== null && exp * 1000 > Date.now()
}

/** Discord's callback returns here with `#token=<jwt>`; stash it (cookie + localStorage) and clean the URL. */
export function captureTokenFromHash(): void {
  const frag = new URLSearchParams(window.location.hash.slice(1))
  const token = frag.get('token')
  if (token) {
    writeTokenCookie(token)
    localStorage.setItem(TOKEN_KEY, token)
    history.replaceState(null, '', window.location.pathname + window.location.search)
  }
}
