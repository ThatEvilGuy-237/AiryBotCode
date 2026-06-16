// Persist the image-derived @hive/ui theme so it survives reloads. The chosen
// image (data URL) doubles as the panel profile picture. Panel-wide (localStorage)
// for now; per-bot backend persistence is a follow-up.
import { deriveThemeFromImage, applyTheme, clearTheme, type DerivedTheme } from '@hive/ui'

const KEY = 'airy.imageTheme.v1'

export interface StoredTheme {
  image: string        // data URL — also the panel profile pic
  theme: DerivedTheme
}

export function loadImageTheme(): StoredTheme | null {
  try {
    const raw = localStorage.getItem(KEY)
    return raw ? (JSON.parse(raw) as StoredTheme) : null
  } catch {
    return null
  }
}

export function saveImageTheme(image: string, theme: DerivedTheme): void {
  try {
    localStorage.setItem(KEY, JSON.stringify({ image, theme }))
  } catch {
    /* quota / private mode — theme just won't persist */
  }
}

export function clearImageTheme(): void {
  localStorage.removeItem(KEY)
  clearTheme(document.documentElement)
}

/** Apply the stored theme to the whole page (call once at app startup). */
export function applyStoredTheme(): StoredTheme | null {
  const stored = loadImageTheme()
  if (stored) applyTheme(stored.theme, document.documentElement)
  return stored
}

/** Derive a theme from an image element, apply it, and persist it (+ the image). */
export function setThemeFromImage(img: HTMLImageElement, dataUrl: string): DerivedTheme {
  const theme = deriveThemeFromImage(img)
  applyTheme(theme, document.documentElement)
  saveImageTheme(dataUrl, theme)
  return theme
}
