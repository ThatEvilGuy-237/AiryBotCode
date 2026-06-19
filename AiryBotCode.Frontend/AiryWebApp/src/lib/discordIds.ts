// Helpers for the id-list settings (e.g. NoXpChannelIds / NoXpRoleIds), which the
// bot stores as a JSON array of numbers and deserializes into a `ulong[]`.
//
// Discord snowflakes are 64-bit and overflow JavaScript's safe-integer range, so
// we must NEVER round-trip them through JSON.parse / Number — that silently
// corrupts the trailing digits. Instead we treat ids as opaque strings: pull the
// digit-runs straight out of the stored text, and re-emit them as unquoted JSON
// numbers (which is exactly what System.Text.Json expects for ulong[]).

/** Parse a stored JSON id-array string into ordered, de-duplicated id strings. */
export function parseIdList(raw: string | null | undefined): string[] {
  if (!raw) return []
  const found = raw.match(/\d+/g)
  if (!found) return []
  return [...new Set(found)]
}

/** Serialize id strings back to a compact JSON array of unquoted numbers. */
export function serializeIdList(ids: string[]): string {
  return `[${ids.join(',')}]`
}

// --- Role-reward map (level -> role id), stored as Dictionary<int,ulong> JSON ---
// e.g. {"5":123,"10":456}. Same precision rule: role ids stay strings, never
// Number/JSON.parse. Levels are small ints but we keep them as strings for the
// <input> and only validate on serialize.

export interface RewardRow {
  level: string
  roleId: string
}

/** Parse the stored map JSON into ordered rows. Tolerates quoted/unquoted keys. */
export function parseRoleRewards(raw: string | null | undefined): RewardRow[] {
  if (!raw) return []
  const rows: RewardRow[] = []
  const re = /"?(\d+)"?\s*:\s*(\d+)/g
  let m: RegExpExecArray | null
  while ((m = re.exec(raw)) !== null) rows.push({ level: m[1], roleId: m[2] })
  return rows
}

/** Serialize rows back to map JSON, dropping incomplete rows and de-duping levels (last wins). */
export function serializeRoleRewards(rows: RewardRow[]): string {
  const byLevel = new Map<string, string>()
  for (const r of rows) {
    const level = (r.level ?? '').trim()
    const roleId = (r.roleId ?? '').trim()
    if (!/^\d+$/.test(level) || !/^\d+$/.test(roleId)) continue
    byLevel.set(level, roleId)
  }
  const entries = [...byLevel.entries()].map(([level, roleId]) => `"${level}":${roleId}`)
  return `{${entries.join(',')}}`
}
