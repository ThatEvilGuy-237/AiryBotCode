import type { AiryChannel, Bot, HiveCheck, HiveConnection } from './api'

export type Concern = { tone: 'info' | 'warn' | 'danger'; title: string; fix: { label: string; to: string } }

export type HomeFacts = {
  bot: Bot | null
  hive: HiveConnection | null
  check: HiveCheck | null
  channels: AiryChannel[] | null
}

export function channelName(c: AiryChannel): string {
  return c.channelName ?? `channel ${c.channelId}`
}

export function concerns({ bot, hive, check, channels }: HomeFacts): Concern[] {
  const found: Concern[] = []
  const settings = { label: 'Open settings', to: '/settings' }
  const channelsFix = { label: 'Open channels', to: '/channels' }
  if (bot && !bot.hasToken) found.push({ tone: 'danger', title: 'This bot has no Discord token, so it cannot sign in.', fix: settings })
  if (bot && !bot.enabled) found.push({ tone: 'info', title: 'This bot is turned off.', fix: settings })
  if (hive && !hive.url) found.push({ tone: 'warn', title: 'Airy has no Hive connection, so it answers in no channel.', fix: { label: 'Connect the Hive', to: '/channels' } })
  if (check) {
    const tone = check.reachable ? 'warn' : 'danger'
    for (const problem of new Set(check.problems)) found.push({ tone, title: problem, fix: channelsFix })
  }
  for (const c of channels ?? []) {
    if (!c.enabled) continue
    if (c.channelName === null) found.push({ tone: 'warn', title: `Channel ${c.channelId} is gone from its server, or the bot cannot see it.`, fix: channelsFix })
    else if (c.lastDecisionError) found.push({ tone: 'warn', title: `In ${c.channelName} the last check failed: ${c.lastDecisionError}`, fix: channelsFix })
  }
  return found
}

export type ConcernGroup = { tone: Concern['tone']; fix: Concern['fix']; items: string[] }

const weight = { info: 0, warn: 1, danger: 2 } as const

export function grouped(list: Concern[]): ConcernGroup[] {
  const groups = new Map<string, ConcernGroup>()
  for (const c of list) {
    const g = groups.get(c.fix.to)
    if (!g) groups.set(c.fix.to, { tone: c.tone, fix: c.fix, items: [c.title] })
    else {
      g.items.push(c.title)
      if (weight[c.tone] > weight[g.tone]) g.tone = c.tone
    }
  }
  return [...groups.values()].sort((a, b) => weight[b.tone] - weight[a.tone])
}
