import { signOut, token } from './auth'

export class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message)
  }
}

export type Bot = {
  botId: string
  botName: string
  enabled: boolean
  adminRoleIds: string
  evilId: string
  logChannelId: string
  evilLogChannelId: string
  hasToken: boolean
  databaseName?: string | null
}

export type AnswerMode = 'always' | 'mention' | 'decide'

export type AiryChannel = {
  id: number
  channelId: string
  channelName: string | null
  guild: string | null
  answerMode: AnswerMode
  enabled: boolean
  lastDecisionAt: string | null
  lastDecision: string | null
  lastDecisionError: string | null
}

export type HiveConnection = { url: string | null; agentId: string | null; hasKey: boolean; keyHint: string | null }

export type HiveCheck = {
  reachable: boolean
  canRun: boolean
  agentFound: boolean
  canDecide: boolean
  agents: { id: string; name: string }[]
  deciders: string[]
  problems: string[]
}

export type CommandConfig = { commandName: string; enabled: boolean }

export type Suggestion = { id: number; title: string; status: string }

export type CountingChannel = { channelId: string; channelName: string | null; currentCount: number; highScore: number }

export type BotLook = { primary: string | null; accent: string | null; image: string | null }

async function call<T>(method: string, path: string, body?: unknown): Promise<T> {
  let response: Response
  try {
    response = await fetch(path, {
      method,
      headers: {
        ...(token.value ? { Authorization: `Bearer ${token.value}` } : {}),
        ...(body === undefined ? {} : { 'Content-Type': 'application/json' }),
      },
      body: body === undefined ? undefined : JSON.stringify(body),
    })
  } catch {
    throw new ApiError(0, 'Airy could not be reached.')
  }
  if (response.status === 401) {
    signOut()
    throw new ApiError(401, 'Signed out.')
  }
  if (!response.ok) {
    const text = (await response.text()).trim()
    throw new ApiError(response.status, text && text.length < 300 && !text.startsWith('<') ? text.replace(/^"|"$/g, '') : `Airy answered ${response.status}.`)
  }
  if (response.status === 204) return undefined as T
  const text = await response.text()
  return (text ? JSON.parse(text) : undefined) as T
}

export const api = {
  bots: () => call<Bot[]>('GET', '/api/settings'),
  look: (botId: string) => call<BotLook>('GET', `/api/settings/${botId}/theme`),
  channels: (botId: string) => call<AiryChannel[]>('GET', `/api/channels?botId=${botId}`),
  hive: (botId: string) => call<HiveConnection>('GET', `/api/settings/${botId}/hive`),
  hiveCheck: (botId: string) => call<HiveCheck>('POST', `/api/settings/${botId}/hive/check`, {}),
  commands: (botId: string) => call<CommandConfig[]>('GET', `/api/commands?botId=${botId}`),
  suggestions: () => call<Suggestion[]>('GET', '/api/suggestions'),
  counting: (botId: string) => call<CountingChannel[]>('GET', `/api/featurestatus/${botId}/counting`),
}

export async function openGate(password: string): Promise<string> {
  let response: Response
  try {
    response = await fetch('/api/auth/gate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ password }),
    })
  } catch {
    throw new ApiError(0, 'Airy could not be reached.')
  }
  if (response.status === 401 || response.status === 403) throw new ApiError(response.status, 'That password is not right.')
  if (response.status === 429) throw new ApiError(429, 'Too many tries. Wait a minute and try again.')
  if (!response.ok) throw new ApiError(response.status, `Airy answered ${response.status}.`)
  return ((await response.json()) as { token: string }).token
}

export function discordSignInUrl(gateToken: string): string {
  const client = import.meta.env.VITE_DISCORD_CLIENT_ID ?? '1318870826862379018'
  const query = new URLSearchParams({
    client_id: client,
    redirect_uri: `${location.origin}/api/auth/discord/redirect`,
    response_type: 'code',
    scope: 'identify',
    state: gateToken,
  })
  return `https://discord.com/api/oauth2/authorize?${query}`
}
