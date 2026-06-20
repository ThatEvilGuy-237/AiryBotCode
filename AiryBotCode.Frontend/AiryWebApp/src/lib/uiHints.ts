// Pure parsers + type guards for the command-setting UiHint strings. Kept out of
// CommandModal.vue so the rich-control logic is centralized and unit-testable.
//
// Most rich hints encode their extra data IN the hint string (e.g. "slider:1,10",
// "template:0=User", "list:number") so no DB migration is ever needed — these
// helpers are the single place that decodes them. Every guard is written so the
// modal degrades to a plain input when the hint can't be parsed.

// ---- template: clickable placeholder chips ----
export interface Placeholder {
  token: string // inserted into the message as {token}
  label: string // shown on the chip (== token for plain placeholders)
}

export function isTemplate(uiHint: string | undefined): boolean {
  return uiHint?.startsWith('template:') ?? false
}

// "user,level,xp" -> [{token:'user',label:'user'}, …]
// "0=User,1=Minutes" -> [{token:'0',label:'User'}, …] (positional formats)
export function parsePlaceholders(uiHint: string | undefined): Placeholder[] {
  if (!isTemplate(uiHint)) return []
  return uiHint!.slice('template:'.length)
    .split(',').map((p) => p.trim()).filter(Boolean)
    .map((p) => {
      const eq = p.indexOf('=')
      return eq > 0
        ? { token: p.slice(0, eq).trim(), label: p.slice(eq + 1).trim() }
        : { token: p, label: p }
    })
}

// ---- duration: a number in a fixed unit + a humanized read-out ----
const UNIT_SECONDS: Record<string, number> = { seconds: 1, minutes: 60, hours: 3600, days: 86400 }

export function durationUnit(uiHint: string | undefined): string {
  return uiHint?.startsWith('duration:') ? uiHint.slice('duration:'.length).trim() : ''
}

export function isDuration(uiHint: string | undefined): boolean {
  return durationUnit(uiHint) in UNIT_SECONDS
}

// value "1440" + "duration:minutes" -> "= 1d"; "90" + seconds -> "= 1m 30s".
export function humanizeDuration(value: string, uiHint: string | undefined): string {
  const n = Number(value)
  if (!Number.isFinite(n) || n <= 0) return ''
  let secs = Math.round(n * (UNIT_SECONDS[durationUnit(uiHint)] ?? 1))
  const parts: string[] = []
  for (const [label, size] of [['d', 86400], ['h', 3600], ['m', 60], ['s', 1]] as const) {
    if (secs >= size) { parts.push(`${Math.floor(secs / size)}${label}`); secs %= size }
  }
  return parts.length ? `= ${parts.slice(0, 2).join(' ')}` : ''
}

// ---- slider: a bounded number ----
export interface SliderRange { min: number; max: number; step: number }

export function sliderRange(uiHint: string | undefined): SliderRange | null {
  if (!uiHint?.startsWith('slider:')) return null
  const [min, max, step] = uiHint.slice('slider:'.length).split(',').map((n) => Number(n.trim()))
  if (!Number.isFinite(min) || !Number.isFinite(max) || max <= min) return null
  return { min, max, step: Number.isFinite(step) && step > 0 ? step : 1 }
}

export function isSlider(uiHint: string | undefined): boolean {
  return sliderRange(uiHint) !== null
}

// ---- list: a JSON array edited as chips ----
export type ListKind = 'number' | 'text'

export function listKind(uiHint: string | undefined): ListKind | null {
  if (uiHint === 'list:number') return 'number'
  if (uiHint === 'list:text') return 'text'
  return null
}

// Parses the stored value as a JSON array (the format the bot reads). Returns null
// when it isn't an array, so the caller can fall back to the json textarea.
export function parseJsonArray(value: string | undefined): unknown[] | null {
  const v = (value ?? '').trim()
  if (v === '') return []
  try {
    const parsed = JSON.parse(v)
    return Array.isArray(parsed) ? parsed : null
  } catch { return null }
}

export function isList(uiHint: string | undefined, value: string | undefined): boolean {
  return listKind(uiHint) !== null && parseJsonArray(value) !== null
}

export function listItems(value: string | undefined): string[] {
  return (parseJsonArray(value) ?? []).map((x) => String(x))
}

// Serialize chips back to a JSON array string; number lists coerce to numbers.
export function serializeList(items: string[], kind: ListKind | null): string {
  return JSON.stringify(kind === 'number' ? items.map((x) => Number(x)) : items)
}

// ---- keyvalue: a JSON object edited as key→value rows ----
export type KeyValueKind = 'number' | 'text'
export interface KvRow { key: string; val: string }

export function keyvalueKind(uiHint: string | undefined): KeyValueKind | null {
  if (uiHint === 'keyvalue:number') return 'number'
  if (uiHint === 'keyvalue:text') return 'text'
  return null
}

// Parses the stored value as a JSON object → rows. Returns null for non-objects
// (arrays/scalars/garbage) so the caller can fall back to the json textarea.
export function parseKeyValue(value: string | undefined): KvRow[] | null {
  const v = (value ?? '').trim()
  if (v === '') return []
  try {
    const o = JSON.parse(v)
    if (o === null || typeof o !== 'object' || Array.isArray(o)) return null
    return Object.entries(o).map(([key, val]) => ({ key, val: String(val) }))
  } catch { return null }
}

export function isKeyValue(uiHint: string | undefined, value: string | undefined): boolean {
  return keyvalueKind(uiHint) !== null && parseKeyValue(value) !== null
}

// Rows → JSON object string; number kind coerces values, blank keys are dropped.
export function serializeKeyValue(rows: KvRow[], kind: KeyValueKind | null): string {
  const o: Record<string, string | number> = {}
  for (const r of rows) {
    const k = r.key.trim()
    if (!k) continue
    o[k] = kind === 'number' ? Number(r.val) : r.val
  }
  return JSON.stringify(o)
}

// ---- emoji ----
export const COMMON_EMOJI = ['✅', '☑️', '✔️', '🎉', '⭐', '🔥', '💯', '👍', '🆗', '🍪']

export function isEmoji(uiHint: string | undefined): boolean {
  return uiHint === 'emoji'
}
