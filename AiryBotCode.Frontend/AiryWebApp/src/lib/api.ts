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
  settings: CommandSetting[]
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

  // ---- Per-command settings + bot reload ----
  getCommands(): Promise<CommandConfig[]> {
    return json<CommandConfig[]>('/api/commands')
  },

  async saveCommand(commandName: string, settings: { key: string; value: string }[]): Promise<boolean> {
    const res = await authFetch(`/api/commands/${encodeURIComponent(commandName)}`, {
      method: 'PUT',
      body: JSON.stringify({ settings }),
    })
    return res.ok
  },

  async reloadBot(): Promise<boolean> {
    const res = await authFetch('/api/bot/reload', { method: 'POST' })
    return res.ok
  },

  // ---- Live database explorer ----
  getDbSchemas(): Promise<string[]> {
    return json<string[]>('/api/db/schemas')
  },

  getDbTables(schema: string): Promise<{ name: string; rowCount: number }[]> {
    return json(`/api/db/tables?schema=${encodeURIComponent(schema)}`)
  },

  getDbData(
    schema: string,
    table: string,
    limit = 100,
  ): Promise<{ columns: string[]; rows: Record<string, unknown>[] }> {
    return json(
      `/api/db/data?schema=${encodeURIComponent(schema)}&table=${encodeURIComponent(table)}&limit=${limit}`,
    )
  },
}
