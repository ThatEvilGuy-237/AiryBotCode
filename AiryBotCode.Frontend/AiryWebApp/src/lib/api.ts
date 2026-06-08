import { API_BASE_URL } from './config'
import { token, redirectToLogin } from './auth'
import type { BotSetting } from '../types/botSetting'

export interface CommandSetting {
  key: string
  value: string
  description: string
  category: string
  uiHint: string
  isReloadable: boolean
}

export interface CommandConfig {
  commandName: string
  enabled: boolean
  settings: CommandSetting[]
}

export interface ChannelWebhook {
  id: number
  channelId: string
  name: string
  webhookUrl: string
  hasSecret: boolean
  secret?: string | null
  mode: string
  enabled: boolean
}

export class ApiError extends Error {
  readonly status: number
  constructor(status: number, message: string) {
    super(message)
    this.status = status
    this.name = 'ApiError'
  }
}

async function authFetch(path: string, init: RequestInit = {}): Promise<Response> {
  const headers = new Headers(init.headers)
  if (token.value) {
    headers.set('Authorization', `Bearer ${token.value}`)
  }
  if (init.body) {
    headers.set('Content-Type', 'application/json')
  }

  let response: Response
  try {
    response = await fetch(`${API_BASE_URL}${path}`, { ...init, headers })
  } catch {
    throw new ApiError(0, 'Failed to reach the API. Is it running?')
  }

  if (response.status === 401) {
    // Expired/invalid token — drop it and send the user back to the login app.
    redirectToLogin()
    throw new ApiError(401, 'Your session expired. Please log in again.')
  }

  return response
}

async function json<T>(path: string, init: RequestInit = {}): Promise<T> {
  const response = await authFetch(path, init)
  if (!response.ok) {
    throw new ApiError(response.status, `Request failed (${response.status} ${response.statusText}).`)
  }
  return (await response.json()) as T
}

export const api = {
  /** Plain-text health check (`/ping` returns "pong"). */
  async ping(): Promise<string> {
    const response = await authFetch('/ping')
    if (!response.ok) {
      throw new ApiError(response.status, `Ping failed (${response.status} ${response.statusText}).`)
    }
    return await response.text()
  },

  getBotSettings(): Promise<BotSetting[]> {
    return json<BotSetting[]>('/api/settings')
  },

  updateBotSettings(botId: string, setting: BotSetting): Promise<BotSetting> {
    return json<BotSetting>(`/api/settings/${botId}`, {
      method: 'PUT',
      body: JSON.stringify(setting),
    })
  },

  /** Add a new bot to the roster. */
  async createBot(setting: BotSetting): Promise<BotSetting> {
    const res = await authFetch('/api/settings', {
      method: 'POST',
      body: JSON.stringify(setting),
    })
    if (res.status === 409) throw new ApiError(409, 'A bot with that ID already exists.')
    if (!res.ok) throw new ApiError(res.status, `Could not add the bot (${res.status}).`)
    return (await res.json()) as BotSetting
  },

  // ---- Channel → webhook chat links ----
  getChannelWebhooks(botId: string): Promise<ChannelWebhook[]> {
    return json<ChannelWebhook[]>(`/api/channelwebhooks?botId=${encodeURIComponent(botId)}`)
  },

  async createChannelWebhook(botId: string, link: Partial<ChannelWebhook>): Promise<ChannelWebhook> {
    const res = await authFetch(`/api/channelwebhooks?botId=${encodeURIComponent(botId)}`, {
      method: 'POST',
      body: JSON.stringify(link),
    })
    if (!res.ok) throw new ApiError(res.status, `Could not add the link (${res.status}).`)
    return (await res.json()) as ChannelWebhook
  },

  async updateChannelWebhook(botId: string, id: number, link: Partial<ChannelWebhook>): Promise<boolean> {
    const res = await authFetch(`/api/channelwebhooks/${id}?botId=${encodeURIComponent(botId)}`, {
      method: 'PUT',
      body: JSON.stringify(link),
    })
    return res.ok
  },

  async deleteChannelWebhook(botId: string, id: number): Promise<boolean> {
    const res = await authFetch(`/api/channelwebhooks/${id}?botId=${encodeURIComponent(botId)}`, {
      method: 'DELETE',
    })
    return res.ok
  },

  /** Set a bot's theme palette (primary + accent hex). Returns the updated bot. */
  async setTheme(botId: string, primary: string, accent: string): Promise<BotSetting> {
    const res = await authFetch(`/api/settings/${encodeURIComponent(botId)}/theme`, {
      method: 'PUT',
      body: JSON.stringify({ primary, accent }),
    })
    if (!res.ok) throw new ApiError(res.status, `Could not save the theme (${res.status}).`)
    return (await res.json()) as BotSetting
  },

  /** Remove a bot from the roster. */
  async deleteBot(botId: string): Promise<void> {
    const res = await authFetch(`/api/settings/${encodeURIComponent(botId)}`, { method: 'DELETE' })
    if (!res.ok) throw new ApiError(res.status, `Could not delete the bot (${res.status}).`)
  },

  /** Reveal the stored bot token — requires re-entering the access password. */
  async revealBotToken(botId: string, password: string): Promise<string> {
    const res = await authFetch(`/api/settings/${encodeURIComponent(botId)}/token`, {
      method: 'POST',
      body: JSON.stringify({ password }),
    })
    if (res.status === 403) throw new ApiError(403, 'Wrong password.')
    if (!res.ok) throw new ApiError(res.status, `Could not reveal token (${res.status}).`)
    const data = (await res.json()) as { token: string }
    return data.token
  },

  // ---- Per-bot command config + bot reload ----
  getCommands(botId: string): Promise<CommandConfig[]> {
    return json<CommandConfig[]>(`/api/commands?botId=${encodeURIComponent(botId)}`)
  },

  async saveCommand(
    botId: string,
    commandName: string,
    settings: { key: string; value: string }[],
  ): Promise<boolean> {
    const res = await authFetch(
      `/api/commands/${encodeURIComponent(commandName)}?botId=${encodeURIComponent(botId)}`,
      { method: 'PUT', body: JSON.stringify({ settings }) },
    )
    return res.ok
  },

  async setCommandEnabled(botId: string, commandName: string, enabled: boolean): Promise<boolean> {
    const res = await authFetch(
      `/api/commands/${encodeURIComponent(commandName)}/enabled?botId=${encodeURIComponent(botId)}`,
      { method: 'PUT', body: JSON.stringify({ enabled }) },
    )
    return res.ok
  },

  async reloadBot(): Promise<boolean> {
    const res = await authFetch('/api/bot/reload', { method: 'POST' })
    return res.ok
  },

  // ---- Live database explorer ----
  getDatabases(): Promise<string[]> {
    return json<string[]>('/api/db/databases')
  },

  async createDatabase(name: string): Promise<boolean> {
    const res = await authFetch('/api/db/databases', {
      method: 'POST',
      body: JSON.stringify({ name }),
    })
    if (res.status === 409) throw new ApiError(409, 'A database with that name already exists.')
    if (res.status === 400) throw new ApiError(400, 'Invalid name — letters, digits and underscores only.')
    if (!res.ok) throw new ApiError(res.status, `Could not create the database (${res.status}).`)
    return true
  },

  getDbSchemas(database?: string): Promise<string[]> {
    return json<string[]>(`/api/db/schemas${database ? `?database=${encodeURIComponent(database)}` : ''}`)
  },

  getDbTables(schema: string, database?: string): Promise<{ name: string; rowCount: number }[]> {
    const db = database ? `&database=${encodeURIComponent(database)}` : ''
    return json(`/api/db/tables?schema=${encodeURIComponent(schema)}${db}`)
  },

  getDbData(
    schema: string,
    table: string,
    limit = 100,
    database?: string,
  ): Promise<{ columns: string[]; rows: Record<string, unknown>[] }> {
    const db = database ? `&database=${encodeURIComponent(database)}` : ''
    return json(
      `/api/db/data?schema=${encodeURIComponent(schema)}&table=${encodeURIComponent(table)}&limit=${limit}${db}`,
    )
  },
}
