import { describe, expect, it } from 'vitest'
import { expiresAt } from '../auth'
import { ago } from '../time'

const jwt = (payload: object) => `x.${btoa(JSON.stringify(payload)).replace(/=+$/, '').replace(/\+/g, '-').replace(/\//g, '_')}.y`

describe('tokens and times', () => {
  it('reads the expiry of a token and refuses junk', () => {
    expect(expiresAt(jwt({ exp: 1800000000 }))).toBe(1800000000000)
    expect(expiresAt('junk')).toBeNull()
    expect(expiresAt(null)).toBeNull()
  })

  it('says how long ago in words', () => {
    const now = Date.parse('2026-10-15T16:00:00Z')
    expect(ago('2026-10-15T15:59:50Z', now)).toBe('just now')
    expect(ago('2026-10-15T15:57:00Z', now)).toBe('3 minutes ago')
    expect(ago('2026-10-14T16:00:00Z', now)).toBe('yesterday')
    expect(ago(null, now)).toBe('')
  })
})
