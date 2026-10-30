import { csrfToken, forget, unsafe } from './auth'

export class ApiError extends Error {
  constructor(public status: number, message: string, public reason = '') {
    super(message)
  }
}

export const Reauthenticate = 'reauthenticate'

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
  namesKnown: boolean
  answerMode: AnswerMode
  enabled: boolean
  listens: boolean
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

export type DiscordEntity = { id: string; name: string; guild: string }

export type HiveAgent = { id: string; name: string }

export type HiveSave = { url: string; agentId: string; appKey?: string }

export type ChannelWebhook = {
  id: number
  channelId: string
  name: string
  webhookUrl: string
  hasSecret: boolean
  secret?: string | null
  mode: 'sync' | 'async'
  enabled: boolean
}

export type HiveTry = { yes: boolean | null; sure: boolean; confidence: number; error: string | null; summary: string; answers: boolean }

export type CommandConfig = { commandName: string; enabled: boolean; settings?: import('./commands').Setting[] }

export type Suggestion = {
  id: number
  title: string
  body: string
  submittedBy: string | null
  createdAt: string
  status: 'new' | 'reviewed' | 'approved' | 'rejected' | string
  responseWhy: string | null
  responseEstimate: string | null
  respondedAt: string | null
  approved: boolean
  approvedAt: string | null
}

export type ShareLink = { code: string; sharePath: string }

export type CountingStatus = {
  channelId: string
  channelName: string | null
  currentCount: number
  highScore: number
  bossActive: boolean
  bossSpawnedAt: string | null
  lastUserId: string | null
  lastUserName: string | null
  updatedAt: string | null
}

export type LevelUser = { rank: number; userId: string; userName: string | null; xp: number; level: number; lastXpAt: string | null }

export type LevelBoard = { total: number; skip: number; take: number; users: LevelUser[] }

export type TableInfo = { name: string; rowCount: number }

export type TableData = { columns: string[]; rows: unknown[][] }

export type CountingChannel = { channelId: string; channelName: string | null; currentCount: number; highScore: number }

export type BotLook = { primary: string | null; accent: string | null; image: string | null }

export function readError(raw: string, status: number): string {
  const text = raw.trim()
  const fallback = `Airy answered ${status}.`
  if (!text || text.startsWith('<')) return fallback
  try {
    const body = JSON.parse(text) as unknown
    if (typeof body === 'string') return body
    if (body && typeof body === 'object') {
      const o = body as { error?: unknown; errors?: Record<string, unknown>; title?: unknown }
      if (typeof o.error === 'string') return o.error
      const first = o.errors ? Object.values(o.errors).flat()[0] : undefined
      if (typeof first === 'string') return first
      if (typeof o.title === 'string') return o.title
    }
    return fallback
  } catch {
    return text.length < 300 ? text : fallback
  }
}

function reasonOf(raw: string): string {
  try {
    const body = JSON.parse(raw) as { reason?: unknown }
    return typeof body?.reason === 'string' ? body.reason : ''
  } catch {
    return ''
  }
}

async function call<T>(method: string, path: string, body?: unknown): Promise<T> {
  const headers: Record<string, string> = body === undefined ? {} : { 'Content-Type': 'application/json' }
  const csrf = unsafe(method) ? csrfToken() : ''
  if (csrf) headers['X-Evil-Csrf'] = csrf
  let response: Response
  try {
    response = await fetch(path, {
      method,
      credentials: 'same-origin',
      cache: 'no-store',
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    })
  } catch {
    throw new ApiError(0, 'Airy could not be reached.')
  }
  if (!response.ok) {
    const raw = await response.text()
    const reason = reasonOf(raw)
    if (response.status === 401 && reason !== Reauthenticate) {
      forget()
      location.assign('/login')
      throw new ApiError(401, 'Signed out.', reason)
    }
    throw new ApiError(response.status, readError(raw, response.status), reason)
  }
  if (response.status === 204) return undefined as T
  const text = await response.text()
  return (text ? JSON.parse(text) : undefined) as T
}

export const api = {
  bots: () => call<Bot[]>('GET', '/api/settings'),
  addBot: (bot: Bot & { token: string }) => call<Bot>('POST', '/api/settings', bot),
  saveBot: (bot: Bot & { token?: string }) => call<Bot>('PUT', `/api/settings/${bot.botId}`, bot),
  dropBot: (botId: string) => call<void>('DELETE', `/api/settings/${botId}`),
  revealToken: (botId: string) => call<{ token: string }>('POST', `/api/settings/${botId}/token`),
  databases: () => call<string[]>('GET', '/api/db/databases'),
  look: (botId: string) => call<BotLook>('GET', `/api/settings/${botId}/theme`),
  saveLook: (botId: string, look: { primary: string; accent: string; image: string | null }) =>
    call<Bot>('PUT', `/api/settings/${botId}/theme`, { ...look, data: null }),
  useAvatar: (botId: string) => call<void>('POST', `/api/settings/${botId}/avatar`),
  channels: (botId: string) => call<AiryChannel[]>('GET', `/api/channels?botId=${botId}`),
  hive: (botId: string) => call<HiveConnection>('GET', `/api/settings/${botId}/hive`),
  hiveCheck: (botId: string) => call<HiveCheck>('POST', `/api/settings/${botId}/hive/check`, {}),
  hiveAgents: (botId: string, url: string, appKey: string) => call<HiveAgent[]>('POST', `/api/settings/${botId}/hive/agents`, { url, appKey }),
  saveHive: (botId: string, body: HiveSave) => call<void>('PUT', `/api/settings/${botId}/hive`, body),
  tryHive: (botId: string, message: string, channel: string) => call<HiveTry>('POST', `/api/settings/${botId}/hive/try`, { message, channel }),
  dropHive: (botId: string) => call<void>('DELETE', `/api/settings/${botId}/hive`),
  discordChannels: (botId: string) => call<DiscordEntity[]>('GET', `/api/settings/${botId}/discord/channels`),
  addChannel: (botId: string, channelId: string, answerMode: AnswerMode) =>
    call<AiryChannel>('POST', `/api/channels?botId=${botId}`, { channelId, answerMode, enabled: true, listens: true }),
  saveChannel: (botId: string, c: Pick<AiryChannel, 'id' | 'answerMode' | 'enabled' | 'listens'>) =>
    call<void>('PUT', `/api/channels/${c.id}?botId=${botId}`, { answerMode: c.answerMode, enabled: c.enabled, listens: c.listens }),
  dropChannel: (botId: string, id: number) => call<void>('DELETE', `/api/channels/${id}?botId=${botId}`),
  webhooks: (botId: string) => call<ChannelWebhook[]>('GET', `/api/channelwebhooks?botId=${botId}`),
  addWebhook: (botId: string, w: Omit<ChannelWebhook, 'id' | 'hasSecret'>) => call<ChannelWebhook>('POST', `/api/channelwebhooks?botId=${botId}`, w),
  saveWebhook: (botId: string, w: Omit<ChannelWebhook, 'hasSecret'>) => call<void>('PUT', `/api/channelwebhooks/${w.id}?botId=${botId}`, w),
  dropWebhook: (botId: string, id: number) => call<void>('DELETE', `/api/channelwebhooks/${id}?botId=${botId}`),
  commands: (botId: string) => call<CommandConfig[]>('GET', `/api/commands?botId=${botId}`),
  commandsFull: (botId: string) => call<import('./commands').Command[]>('GET', `/api/commands?botId=${botId}`),
  saveCommand: (botId: string, name: string, settings: { key: string; value: string }[]) =>
    call<void>('PUT', `/api/commands/${encodeURIComponent(name)}?botId=${botId}`, { settings }),
  setCommandOn: (botId: string, name: string, enabled: boolean) =>
    call<void>('PUT', `/api/commands/${encodeURIComponent(name)}/enabled?botId=${botId}`, { enabled }),
  restartBots: () => call<{ requested: boolean }>('POST', '/api/bot/reload'),
  discordCategories: (botId: string) => call<DiscordEntity[]>('GET', `/api/settings/${botId}/discord/categories`),
  discordRoles: (botId: string) => call<DiscordEntity[]>('GET', `/api/settings/${botId}/discord/roles`),
  suggestions: () => call<Suggestion[]>('GET', '/api/suggestions'),
  addSuggestion: (title: string, body: string) => call<Suggestion>('POST', '/api/suggestions', { title, body, submittedBy: null }),
  approveSuggestion: (id: number) => call<unknown>('POST', `/api/suggestions/${id}/approve`),
  rejectSuggestion: (id: number) => call<unknown>('POST', `/api/suggestions/${id}/reject`),
  dropSuggestion: (id: number) => call<void>('DELETE', `/api/suggestions/${id}`),
  shareLink: () => call<ShareLink>('GET', '/api/suggestions/sharelink'),
  renewShareLink: () => call<ShareLink>('POST', '/api/suggestions/sharelink/regenerate'),
  levels: (botId: string, skip: number, take = 25) => call<LevelBoard>('GET', `/api/featurestatus/${botId}/leveling?skip=${skip}&take=${take}`),
  countingFull: (botId: string) => call<CountingStatus[]>('GET', `/api/featurestatus/${botId}/counting`),
  schemas: (db: string) => call<string[]>('GET', `/api/db/schemas?database=${encodeURIComponent(db)}`),
  tables: (db: string, schema: string) => call<TableInfo[]>('GET', `/api/db/tables?schema=${encodeURIComponent(schema)}&database=${encodeURIComponent(db)}`),
  tableData: (db: string, schema: string, table: string) =>
    call<TableData>('GET', `/api/db/data?schema=${encodeURIComponent(schema)}&table=${encodeURIComponent(table)}&limit=100&database=${encodeURIComponent(db)}`),
  counting: (botId: string) => call<CountingChannel[]>('GET', `/api/featurestatus/${botId}/counting`),
}
