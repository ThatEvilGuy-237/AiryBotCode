import { describe, expect, it } from 'vitest'
import type { AiryChannel, Bot } from '../api'
import { concerns, grouped } from '../home'

const bot: Bot = { botId: '1', botName: 'Airy', enabled: true, adminRoleIds: '', evilId: '0', logChannelId: '0', evilLogChannelId: '0', hasToken: true }
const channel = (over: Partial<AiryChannel>): AiryChannel => ({
  id: 1, channelId: '10', channelName: '#chat', guild: 'Server', namesKnown: true, listens: true, answerMode: 'decide', enabled: true,
  lastDecisionAt: null, lastDecision: null, lastDecisionError: null, ...over,
})

describe('Home concerns', () => {
  it('says nothing when all is well', () => {
    expect(concerns({ bot, hive: { url: 'http://spine:7000', agentId: 'a', hasKey: true, keyHint: null }, check: null, channels: [channel({})] })).toEqual([])
  })

  it('names a missing token, a missing Hive, a gone channel and a failed check, and skips paused channels', () => {
    const found = concerns({
      bot: { ...bot, hasToken: false },
      hive: { url: null, agentId: null, hasKey: false, keyHint: null },
      check: null,
      channels: [channel({ channelName: null, channelId: '11' }), channel({ id: 2, lastDecisionError: 'no decider' }), channel({ id: 3, channelName: null, enabled: false, listens: false })],
    })
    expect(found.map(f => f.tone)).toEqual(['danger', 'warn', 'warn', 'warn'])
    expect(found[2].title).toContain('11')
    expect(found[3].title).toContain('no decider')
  })

  it('does not call a channel gone when Discord could not be read', () => {
    expect(concerns({ bot, hive: null, check: null, channels: [channel({ channelName: null, namesKnown: false })] })).toEqual([])
  })

  it('groups concerns that share a fix and keeps the worst tone first', () => {
    const groups = grouped(concerns({
      bot: { ...bot, hasToken: false },
      hive: { url: 'http://spine:7000', agentId: 'a', hasKey: true, keyHint: null },
      check: { reachable: false, canRun: false, agentFound: false, canDecide: false, agents: [], deciders: [], problems: ['down'] },
      channels: [channel({ lastDecisionError: 'refused' })],
    }))
    expect(groups.map(g => [g.fix.to, g.tone, g.items.length])).toEqual([['/settings', 'danger', 1], ['/channels', 'danger', 2]])
  })
})
