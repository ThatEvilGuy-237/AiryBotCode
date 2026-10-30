import { computed, ref } from 'vue'

export type Me = {
  user: { id: string; userName: string | null; displayName: string | null; email: string | null } | null
  role: string | null
  scopes: string[]
}

export class AuthError extends Error {
  constructor(public status: number, message: string, public reason = '') {
    super(message)
  }
}

const CsrfCookie = '__Host-evil-csrf'

export const me = ref<Me | null>(null)
export const isOwner = computed(() => me.value?.role === 'owner')
export const whoName = computed(() => me.value?.user?.displayName || me.value?.user?.userName || '')

let asked: Promise<Me | null> | null = null

export function csrfToken(): string {
  const hit = document.cookie.split('; ').find(c => c.startsWith(`${CsrfCookie}=`))
  return hit ? decodeURIComponent(hit.slice(CsrfCookie.length + 1)) : ''
}

export function unsafe(method: string): boolean {
  return !['GET', 'HEAD', 'OPTIONS'].includes(method.toUpperCase())
}

export async function readProblem(response: Response): Promise<{ title: string; reason: string; text: string }> {
  const text = await response.text()
  try {
    const body = JSON.parse(text) as { title?: unknown; reason?: unknown }
    return { title: typeof body.title === 'string' ? body.title : '', reason: typeof body.reason === 'string' ? body.reason : '', text }
  } catch {
    return { title: '', reason: '', text }
  }
}

async function send(method: string, path: string, body?: unknown): Promise<Response> {
  const headers: Record<string, string> = body === undefined ? {} : { 'Content-Type': 'application/json' }
  const csrf = unsafe(method) ? csrfToken() : ''
  if (csrf) headers['X-Evil-Csrf'] = csrf
  try {
    return await fetch(path, { method, credentials: 'same-origin', cache: 'no-store', headers, body: body === undefined ? undefined : JSON.stringify(body) })
  } catch {
    throw new AuthError(0, 'Airy could not be reached.')
  }
}

export function whoAmI(again = false): Promise<Me | null> {
  if (!asked || again) {
    asked = send('GET', '/api/auth/me')
      .then(async response => (response.ok ? ((await response.json()) as Me) : null))
      .catch(() => null)
      .then(found => (me.value = found))
  }
  return asked
}

export function nextPath(raw: unknown): string {
  return typeof raw === 'string' && raw.startsWith('/') && !raw.startsWith('//') && !raw.startsWith('/login') ? raw : '/'
}

export function forget(): void {
  me.value = null
  asked = Promise.resolve(null)
}

export function signInMessage(status: number, reason: string): string {
  if (status === 401) return 'That user name or password is not right.'
  if (status === 429) return 'Too many tries. Wait a minute and try again.'
  if (reason === 'no-access') return 'This account has no access to Airy.'
  if (reason === 'blocked') return 'This account was blocked from Airy.'
  if (reason === 'locked') return 'This account is locked. Ask the owner to open it.'
  if (status === 400) return 'Enter your user name and your password.'
  return `Airy answered ${status}.`
}

export async function signIn(login: string, password: string): Promise<Me> {
  const response = await send('POST', '/api/auth/login', { login, password, mode: 'browser' })
  if (!response.ok) {
    const problem = await readProblem(response)
    throw new AuthError(response.status, signInMessage(response.status, problem.reason), problem.reason)
  }
  const found = await whoAmI(true)
  if (!found) throw new AuthError(401, 'Signing in did not hold. Try again.')
  return found
}

export async function confirmPassword(password: string): Promise<void> {
  const response = await send('POST', '/api/auth/reauth', { password })
  if (response.ok) return
  const problem = await readProblem(response)
  if (response.status === 429) throw new AuthError(429, 'Too many tries. Wait a minute and try again.', problem.reason)
  if (response.status === 400) throw new AuthError(400, 'That password is not right.', problem.reason)
  throw new AuthError(response.status, `Airy answered ${response.status}.`, problem.reason)
}

export async function signOut(): Promise<void> {
  await send('POST', '/api/auth/logout').catch(() => undefined)
  forget()
  location.assign('/login')
}

export async function discordSignIn(): Promise<boolean> {
  try {
    const response = await fetch('/api/version', { cache: 'no-store' })
    if (!response.ok) return false
    const version = (await response.json()) as { modules?: unknown }
    return Array.isArray(version.modules) && version.modules.includes('external.discord')
  } catch {
    return false
  }
}

export const discordStart = '/api/auth/external/discord/start'
