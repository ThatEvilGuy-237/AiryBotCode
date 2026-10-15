export function parseIdList(raw: string | null | undefined): string[] {
  if (!raw) return []
  const found = raw.match(/\d+/g)
  if (!found) return []
  return [...new Set(found)]
}

export function serializeIdList(ids: string[]): string {
  return `[${ids.join(',')}]`
}

export interface RewardRow {
  level: string
  roleId: string
}

export function parseRoleRewards(raw: string | null | undefined): RewardRow[] {
  if (!raw) return []
  const rows: RewardRow[] = []
  const re = /"?(\d+)"?\s*:\s*(\d+)/g
  let m: RegExpExecArray | null
  while ((m = re.exec(raw)) !== null) rows.push({ level: m[1], roleId: m[2] })
  return rows
}

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
