import { ref } from 'vue'

const KEY = 'jwt'

function readCookie(): string | null {
  const hit = document.cookie.split('; ').find(c => c.startsWith(`${KEY}=`))
  return hit ? decodeURIComponent(hit.slice(KEY.length + 1)) : null
}

export function expiresAt(jwt: string | null): number | null {
  if (!jwt) return null
  try {
    const part = jwt.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const exp = JSON.parse(atob(part.padEnd(part.length + ((4 - (part.length % 4)) % 4), '='))).exp
    return typeof exp === 'number' ? exp * 1000 : null
  } catch {
    return null
  }
}

export const token = ref<string | null>(readCookie() ?? localStorage.getItem(KEY))

export function saveToken(jwt: string): void {
  token.value = jwt
  localStorage.setItem(KEY, jwt)
  const exp = expiresAt(jwt)
  const maxAge = exp ? Math.max(0, Math.floor((exp - Date.now()) / 1000)) : 86400
  const secure = location.protocol === 'https:' ? '; secure' : ''
  document.cookie = `${KEY}=${encodeURIComponent(jwt)}; path=/; max-age=${maxAge}; samesite=lax${secure}`
}

export function captureToken(): void {
  const hit = location.hash.match(/token=([^&]+)/)
  if (!hit) return
  saveToken(decodeURIComponent(hit[1]))
  history.replaceState(null, '', location.pathname + location.search)
}

export function isSignedIn(): boolean {
  const exp = expiresAt(token.value)
  return exp !== null && exp > Date.now()
}

export function forget(): void {
  token.value = null
  localStorage.removeItem(KEY)
  document.cookie = `${KEY}=; path=/; max-age=0`
}

export function signOut(): void {
  forget()
  location.assign('/login')
}
