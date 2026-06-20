// Compact "time ago" formatting for read-only status displays. Returns a short
// human string ("just now", "3m ago", "2h ago", "5d ago"); empty for missing or
// unparseable input so callers can `v-if` it away.
const DIVISIONS: { limit: number; div: number; unit: string }[] = [
  { limit: 60, div: 1, unit: 's' },
  { limit: 3600, div: 60, unit: 'm' },
  { limit: 86400, div: 3600, unit: 'h' },
  { limit: 2592000, div: 86400, unit: 'd' },
  { limit: 31536000, div: 2592000, unit: 'mo' },
  { limit: Infinity, div: 31536000, unit: 'y' },
]

export function relativeTime(iso: string | null | undefined, now: number = Date.now()): string {
  if (!iso) return ''
  const then = Date.parse(iso)
  if (Number.isNaN(then)) return ''

  const seconds = Math.max(0, (now - then) / 1000)
  if (seconds < 10) return 'just now'

  for (const { limit, div, unit } of DIVISIONS) {
    if (seconds < limit) return `${Math.floor(seconds / div)}${unit} ago`
  }
  return ''
}
