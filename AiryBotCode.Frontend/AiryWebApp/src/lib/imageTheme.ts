// Persist the image-derived @hive/ui theme PER BOT, so each bot has its own look
// and the chosen image doubles as that bot's profile picture. Switching bots
// re-themes the whole panel. Client-side (localStorage) — the bot's Discord-embed
// brand colour is the server-side counterpart (api.setTheme).
import { deriveThemeFromImage, applyTheme, clearTheme, type DerivedTheme } from '@hive/ui'

const PREFIX = 'airy.imageTheme.v1.'      // + botId  → per-bot entry
const LEGACY_KEY = 'airy.imageTheme.v1'   // old panel-wide key (pre per-bot)
const ACTIVE_KEY = 'airy.imageTheme.activeBot'

export interface StoredTheme {
  image: string        // data URL — also the bot's profile pic
  theme: DerivedTheme
}

function keyFor(botId: string): string {
  return PREFIX + botId
}

function readKey(key: string): StoredTheme | null {
  try {
    const raw = localStorage.getItem(key)
    return raw ? (JSON.parse(raw) as StoredTheme) : null
  } catch {
    return null
  }
}

/** A bot's stored image + theme, or null. */
export function loadImageTheme(botId: string | null): StoredTheme | null {
  return botId ? readKey(keyFor(botId)) : null
}

export function saveImageTheme(botId: string, image: string, theme: DerivedTheme): void {
  if (!botId) return
  try {
    localStorage.setItem(keyFor(botId), JSON.stringify({ image, theme }))
    localStorage.setItem(ACTIVE_KEY, botId)
    localStorage.removeItem(LEGACY_KEY)   // superseded by a per-bot entry
  } catch {
    /* quota / private mode — theme just won't persist */
  }
}

export function clearImageTheme(botId: string | null): void {
  if (botId) localStorage.removeItem(keyFor(botId))
  clearTheme(document.documentElement)
}

/**
 * Apply a specific bot's stored theme (call on bot switch). Falls back to the
 * default look (clearTheme) when that bot has no saved theme. Records the bot as
 * active so the next startup re-applies it without a flash.
 */
export function applyThemeForBot(botId: string | null): StoredTheme | null {
  const stored = loadImageTheme(botId)
  if (stored) {
    applyTheme(stored.theme, document.documentElement)
    if (botId) localStorage.setItem(ACTIVE_KEY, botId)
  } else {
    clearTheme(document.documentElement)
  }
  return stored
}

/**
 * Apply the last-active bot's theme before mount (no flash of the default look).
 * On first run after the per-bot upgrade, falls back to the old panel-wide theme
 * so the existing look survives until a per-bot theme is saved.
 */
export function applyStartupTheme(): void {
  const active = localStorage.getItem(ACTIVE_KEY)
  const stored = active ? loadImageTheme(active) : null
  if (stored) {
    applyTheme(stored.theme, document.documentElement)
    return
  }
  const legacy = readKey(LEGACY_KEY)
  if (legacy) applyTheme(legacy.theme, document.documentElement)
}

/** Derive a theme from an image element, apply it, and persist it for the bot. */
export function setThemeFromImage(img: HTMLImageElement, dataUrl: string, botId: string): DerivedTheme {
  const theme = deriveThemeFromImage(img)
  applyTheme(theme, document.documentElement)
  saveImageTheme(botId, dataUrl, theme)
  return theme
}
