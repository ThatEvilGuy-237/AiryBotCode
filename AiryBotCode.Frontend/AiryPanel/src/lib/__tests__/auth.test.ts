import { describe, expect, it } from 'vitest'
import { csrfToken, nextPath, signInMessage, unsafe } from '../auth'
import { ago } from '../time'

describe('sessions and times', () => {
  it('reads the CSRF token from its cookie only', () => {
    const jar = { value: 'other=1' }
    Object.defineProperty(document, 'cookie', { configurable: true, get: () => jar.value })
    expect(csrfToken()).toBe('')
    jar.value = 'other=1; __Host-evil-csrf=abc%2Fdef'
    expect(csrfToken()).toBe('abc/def')
  })

  it('sends the CSRF header on writes only', () => {
    expect(['POST', 'PUT', 'DELETE', 'PATCH'].every(unsafe)).toBe(true)
    expect(['GET', 'HEAD', 'OPTIONS', 'get'].some(unsafe)).toBe(false)
  })

  it('goes back only to a page of the panel after signing in', () => {
    expect(nextPath('/settings?bot=1')).toBe('/settings?bot=1')
    expect(nextPath('//evil.example/x')).toBe('/')
    expect(nextPath('https://evil.example/')).toBe('/')
    expect(nextPath('/login?next=/x')).toBe('/')
    expect(nextPath(undefined)).toBe('/')
    expect(nextPath(['/a'])).toBe('/')
  })

  it('says why a sign-in failed in words', () => {
    expect(signInMessage(401, '')).toBe('That user name or password is not right.')
    expect(signInMessage(429, '')).toBe('Too many tries. Wait a minute and try again.')
    expect(signInMessage(403, 'no-access')).toBe('This account has no access to Airy.')
    expect(signInMessage(403, 'blocked')).toBe('This account was blocked from Airy.')
    expect(signInMessage(400, 'login-incomplete')).toBe('Enter your user name and your password.')
  })

  it('says how long ago in words', () => {
    const now = Date.parse('2026-10-15T16:00:00Z')
    expect(ago('2026-10-15T15:59:50Z', now)).toBe('just now')
    expect(ago('2026-10-15T15:57:00Z', now)).toBe('3 minutes ago')
    expect(ago('2026-10-14T16:00:00Z', now)).toBe('yesterday')
    expect(ago(null, now)).toBe('')
  })
})

describe('error messages', async () => {
  const { readError } = await import('../api')
  it('reads a sentence out of whatever the API sent', () => {
    expect(readError('"A valid botId is required."', 400)).toBe('A valid botId is required.')
    expect(readError('{"error":"Airy already talks in this channel."}', 409)).toBe('Airy already talks in this channel.')
    expect(readError('{"title":"One or more validation errors occurred.","errors":{"Data":["The Data field is required."]}}', 400)).toBe('The Data field is required.')
    expect(readError('<html>oops</html>', 502)).toBe('Airy answered 502.')
    expect(readError('Plain words', 400)).toBe('Plain words')
    expect(readError('', 500)).toBe('Airy answered 500.')
  })
})
