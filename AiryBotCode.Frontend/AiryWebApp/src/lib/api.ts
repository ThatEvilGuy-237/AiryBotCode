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

// A pickable Discord channel or role for the settings pickers.
export interface DiscordEntity {
  id: string
  name: string
  guild: string
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

// ---- Read-only feature status (counting game + leveling leaderboard) ----
// Snowflake ids arrive as STRINGS (precision-safe — never JSON.parse/Number them).
export interface CountingChannelStatus {
  channelId: string
  channelName: string | null
  currentCount: number
  highScore: number
  bossActive: boolean
  bossSpawnedAt: string | null
  lastUserId: string | null
  lastUserName: string | null
  updatedAt: string
}

export interface LeaderboardEntry {
  rank: number
  userId: string
  userName: string
  xp: number
  level: number
  lastXpAt: string
}

export interface LevelingBoard {
  total: number
  skip: number
  take: number
  users: LeaderboardEntry[]
}

// ---- Feature suggestions / ideas board ----
export interface Suggestion {
  id: number
  title: string
  body: string
  submittedBy: string
  createdAt: string
  status: string // new | reviewed | approved | rejected
  responseWhy: string | null
  responseEstimate: string | null
  respondedAt: string | null
  approved: boolean
  approvedAt: string | null
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

  /** Set a bot's theme palette (primary + accent hex) and optionally the
   * server-side image theme (downscaled data URL + derived-theme JSON, so it
   * follows across devices). Returns the updated bot. */
  async setTheme(botId: string, primary: string, accent: string, image?: string, data?: string): Promise<BotSetting> {
    const res = await authFetch(`/api/settings/${encodeURIComponent(botId)}/theme`, {
      method: 'PUT',
      body: JSON.stringify({ primary, accent, image: image ?? null, data: data ?? null }),
    })
    if (!res.ok) throw new ApiError(res.status, `Could not save the theme (${res.status}).`)
    return (await res.json()) as BotSetting
  },

  /** Fetch a bot's full theme (palette + server-stored image + derived theme). */
  async getTheme(botId: string): Promise<{ primary: string | null; accent: string | null; image: string | null; data: string | null }> {
    const res = await authFetch(`/api/settings/${encodeURIComponent(botId)}/theme`)
    if (!res.ok) throw new ApiError(res.status, `Could not load the theme (${res.status}).`)
    return await res.json()
  },

  /** Ask the bot to set its Discord avatar from the saved theme image. The bot
   * applies it within ~5s; Discord rate-limits avatar changes to ~2/hour. */
  async setBotAvatar(botId: string): Promise<void> {
    const res = await authFetch(`/api/settings/${encodeURIComponent(botId)}/avatar`, { method: 'POST' })
    if (!res.ok) {
      const msg = res.status === 400
        ? 'Save an image on the Theme page first.'
        : `Could not request the avatar update (${res.status}).`
      throw new ApiError(res.status, msg)
    }
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

  // ---- Discord lookups for channel/role pickers (empty on any failure) ----
  async getBotChannels(botId: string): Promise<DiscordEntity[]> {
    try {
      return await json<DiscordEntity[]>(`/api/settings/${encodeURIComponent(botId)}/discord/channels`)
    } catch {
      return []
    }
  },

  async getBotRoles(botId: string): Promise<DiscordEntity[]> {
    try {
      return await json<DiscordEntity[]>(`/api/settings/${encodeURIComponent(botId)}/discord/roles`)
    } catch {
      return []
    }
  },

  async getBotCategories(botId: string): Promise<DiscordEntity[]> {
    try {
      return await json<DiscordEntity[]>(`/api/settings/${encodeURIComponent(botId)}/discord/categories`)
    } catch {
      return []
    }
  },

  // ---- Read-only feature status ----
  /** Counting-game state per channel for a bot (current count, high score, boss). */
  getCountingStatus(botId: string): Promise<CountingChannelStatus[]> {
    return json<CountingChannelStatus[]>(`/api/featurestatus/${encodeURIComponent(botId)}/counting`)
  },

  /** XP leaderboard for a bot (highest XP first) plus the total ranked-user count. */
  getLevelingBoard(botId: string, skip = 0, take = 25): Promise<LevelingBoard> {
    return json<LevelingBoard>(
      `/api/featurestatus/${encodeURIComponent(botId)}/leveling?skip=${skip}&take=${take}`,
    )
  },

  // ---- Feature suggestions / ideas board ----
  getSuggestions(): Promise<Suggestion[]> {
    return json<Suggestion[]>('/api/suggestions')
  },

  /** Current public share link (code + /suggest/{code} path) for the board. */
  getSuggestionShareLink(): Promise<{ code: string; sharePath: string }> {
    return json<{ code: string; sharePath: string }>('/api/suggestions/sharelink')
  },

  /** Mint a fresh share code — invalidates the previous link. */
  async regenerateSuggestionShareLink(): Promise<{ code: string; sharePath: string }> {
    const res = await authFetch('/api/suggestions/sharelink/regenerate', { method: 'POST' })
    if (!res.ok) throw new ApiError(res.status, `Could not regenerate the link (${res.status}).`)
    return (await res.json()) as { code: string; sharePath: string }
  },

  async createSuggestion(title: string, body: string, submittedBy?: string): Promise<Suggestion> {
    const res = await authFetch('/api/suggestions', {
      method: 'POST',
      body: JSON.stringify({ title, body, submittedBy: submittedBy ?? null }),
    })
    if (!res.ok) throw new ApiError(res.status, `Could not submit the suggestion (${res.status}).`)
    return (await res.json()) as Suggestion
  },

  /** Maintainer review: why it's a good idea + a rough time estimate. */
  async respondSuggestion(id: number, why: string, estimate: string): Promise<Suggestion> {
    const res = await authFetch(`/api/suggestions/${id}/respond`, {
      method: 'PUT',
      body: JSON.stringify({ why, estimate }),
    })
    if (!res.ok) throw new ApiError(res.status, `Could not save the response (${res.status}).`)
    return (await res.json()) as Suggestion
  },

  async approveSuggestion(id: number): Promise<Suggestion> {
    const res = await authFetch(`/api/suggestions/${id}/approve`, { method: 'POST' })
    if (!res.ok) throw new ApiError(res.status, `Could not approve (${res.status}).`)
    return (await res.json()) as Suggestion
  },

  async rejectSuggestion(id: number): Promise<Suggestion> {
    const res = await authFetch(`/api/suggestions/${id}/reject`, { method: 'POST' })
    if (!res.ok) throw new ApiError(res.status, `Could not reject (${res.status}).`)
    return (await res.json()) as Suggestion
  },

  async deleteSuggestion(id: number): Promise<void> {
    const res = await authFetch(`/api/suggestions/${id}`, { method: 'DELETE' })
    if (!res.ok) throw new ApiError(res.status, `Could not delete (${res.status}).`)
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
