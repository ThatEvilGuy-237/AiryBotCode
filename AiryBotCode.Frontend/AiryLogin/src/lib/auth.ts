// Shared with the main app via same-origin localStorage (key `jwt`).
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

/** True only when a non-expired JWT is already stored. */
export function isAuthenticated(): boolean {
  const token = localStorage.getItem(TOKEN_KEY)
  if (!token) return false
  const exp = decodeExp(token)
  return exp !== null && exp * 1000 > Date.now()
}
