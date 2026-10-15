const units: [Intl.RelativeTimeFormatUnit, number][] = [
  ['year', 31536000],
  ['month', 2592000],
  ['week', 604800],
  ['day', 86400],
  ['hour', 3600],
  ['minute', 60],
]

const format = new Intl.RelativeTimeFormat('en', { numeric: 'auto' })

export function ago(iso: string | null | undefined, now = Date.now()): string {
  if (!iso) return ''
  const seconds = Math.round((new Date(iso).getTime() - now) / 1000)
  for (const [unit, size] of units) {
    if (Math.abs(seconds) >= size) return format.format(Math.round(seconds / size), unit)
  }
  return 'just now'
}
