export interface Placeholder {
  token: string
  label: string
}

export function isTemplate(uiHint: string | undefined): boolean {
  return uiHint?.startsWith('template:') ?? false
}

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

const UNIT_SECONDS: Record<string, number> = { seconds: 1, minutes: 60, hours: 3600, days: 86400 }

export function durationUnit(uiHint: string | undefined): string {
  return uiHint?.startsWith('duration:') ? uiHint.slice('duration:'.length).trim() : ''
}

export function isDuration(uiHint: string | undefined): boolean {
  return durationUnit(uiHint) in UNIT_SECONDS
}

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

export type ListKind = 'number' | 'text'

export function listKind(uiHint: string | undefined): ListKind | null {
  if (uiHint === 'list:number') return 'number'
  if (uiHint === 'list:text') return 'text'
  return null
}

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

export function serializeList(items: string[], kind: ListKind | null): string {
  return JSON.stringify(kind === 'number' ? items.map((x) => Number(x)) : items)
}

export type KeyValueKind = 'number' | 'text'
export interface KvRow { key: string; val: string }

export function keyvalueKind(uiHint: string | undefined): KeyValueKind | null {
  if (uiHint === 'keyvalue:number') return 'number'
  if (uiHint === 'keyvalue:text') return 'text'
  return null
}

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

export function serializeKeyValue(rows: KvRow[], kind: KeyValueKind | null): string {
  const o: Record<string, string | number> = {}
  for (const r of rows) {
    const k = r.key.trim()
    if (!k) continue
    o[k] = kind === 'number' ? Number(r.val) : r.val
  }
  return JSON.stringify(o)
}

export const COMMON_EMOJI = ['✅', '☑️', '✔️', '🎉', '⭐', '🔥', '💯', '👍', '🆗', '🍪']

export function isEmoji(uiHint: string | undefined): boolean {
  return uiHint === 'emoji'
}
