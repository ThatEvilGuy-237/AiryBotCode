// Re-skin the panel by overriding its core CSS variables. Components that use
// var(--foxfire)/var(--violet) (sidebar, buttons, accents) update live; the
// derived vars (--primary-color etc.) reference --foxfire so they follow.

function clampHex(v: number): string {
  return Math.max(0, Math.min(255, Math.round(v)))
    .toString(16)
    .padStart(2, '0')
}

function darken(hex: string, amount: number): string {
  const h = hex.replace('#', '')
  if (h.length !== 6) return hex
  const r = parseInt(h.slice(0, 2), 16) * (1 - amount)
  const g = parseInt(h.slice(2, 4), 16) * (1 - amount)
  const b = parseInt(h.slice(4, 6), 16) * (1 - amount)
  return `#${clampHex(r)}${clampHex(g)}${clampHex(b)}`
}

const VARS = ['--foxfire', '--foxfire-deep', '--violet']

/** Apply a bot's palette, or reset to the default theme when unset. */
export function applyTheme(primary?: string | null, accent?: string | null): void {
  const root = document.documentElement.style
  if (primary && /^#[0-9a-fA-F]{6}$/.test(primary)) {
    root.setProperty('--foxfire', primary)
    root.setProperty('--foxfire-deep', darken(primary, 0.15))
    root.setProperty('--violet', accent && /^#[0-9a-fA-F]{6}$/.test(accent) ? accent : primary)
  } else {
    VARS.forEach((v) => root.removeProperty(v))
  }
}
