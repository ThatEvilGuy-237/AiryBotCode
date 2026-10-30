const base = (process.env.DISCORD_CHECK_URL ?? 'http://127.0.0.1:10000').replace(/\/$/, '')
const publicUrl = (process.env.DISCORD_CHECK_PUBLIC ?? 'http://localhost:10000/api').replace(/\/$/, '')
const clientId = process.env.DISCORD_CHECK_CLIENT_ID ?? ''
if (!clientId) {
  console.error('DISCORD_CHECK_CLIENT_ID is required: the dummy client id the dev API was started with (AIRY_DEV_DISCORD_CLIENT_ID), never the real one')
  process.exit(2)
}
const failures = []
let passed = 0

function check(name, want, got) {
  const ok = JSON.stringify(want) === JSON.stringify(got)
  if (ok) passed += 1
  else failures.push(name)
  console.log(`${ok ? 'PASS' : 'FAIL'} ${name}${ok ? '' : ` want ${JSON.stringify(want)} got ${JSON.stringify(got)}`}`)
}

async function get(path, cookie) {
  const headers = { 'cf-connecting-ip': '198.51.100.77' }
  if (cookie) headers.cookie = cookie
  const response = await fetch(base + path, { headers, redirect: 'manual' })
  const text = await response.text()
  let json = {}
  try { json = text ? JSON.parse(text) : {} } catch { json = {} }
  return { status: response.status, json, location: response.headers.get('location') ?? '', cookies: response.headers.getSetCookie() }
}

const binding = (r) => (r.cookies.find((c) => c.startsWith('__Host-evil-oauth=')) ?? '').split(';')[0]
const query = (location) => Object.fromEntries(new URL(location, 'http://x').searchParams)

const version = await get('/api/version')
check('/api/version lists external.discord (Discord sign-in on in this dev run)', true, (version.json.modules ?? []).includes('external.discord'))

const start = await get(`/api/auth/external/discord/start?mode=browser&next=${encodeURIComponent('/login?next=%2Fsettings')}`)
const to = start.location ? new URL(start.location) : null
check('the panel button\'s start answers 302 to Discord\'s authorize page', [302, 'https://discord.com/oauth2/authorize'], [start.status, to ? `${to.origin}${to.pathname}` : ''])
check('... with this app\'s client id, scope identify only, PKCE S256 and the callback under the public URL', [clientId, 'identify', 'S256', `${publicUrl}/auth/external/discord/callback`],
  [to?.searchParams.get('client_id'), to?.searchParams.get('scope'), to?.searchParams.get('code_challenge_method'), to?.searchParams.get('redirect_uri')])
check('... and sets the binding cookie', true, binding(start).length > 20)

const state = to?.searchParams.get('state') ?? ''
const denied = await get(`/api/auth/external/discord/callback?state=${encodeURIComponent(state)}&error=access_denied`, binding(start))
check('Discord sending the person back without a code: 302 to /login?next=/settings&error=provider-error, no session', [302, '/login', { next: '/settings', error: 'provider-error' }, false],
  [denied.status, new URL(denied.location || '/', 'http://x').pathname, query(denied.location || '/'), denied.cookies.some((c) => c.startsWith('__Host-evil-session='))])
const again = await get(`/api/auth/external/discord/callback?state=${encodeURIComponent(state)}&code=x`, binding(start))
check('the same state again: 302 with error=state-used', [302, 'state-used'], [again.status, query(again.location || '/').error])

const other = await get('/api/auth/external/discord/start?mode=browser&next=%2Flogin')
const otherState = new URL(other.location).searchParams.get('state') ?? ''
const unbound = await get(`/api/auth/external/discord/callback?state=${encodeURIComponent(otherState)}&code=x`)
check('a callback without the binding cookie (started in another browser): 302 with error=state-not-bound, no session', [302, 'state-not-bound', false],
  [unbound.status, query(unbound.location || '/').error, unbound.cookies.some((c) => c.startsWith('__Host-evil-session='))])
const unknown = await get('/api/auth/external/discord/callback?state=not-a-state&code=x')
check('an unknown state is 400 state-unknown', [400, 'state-unknown'], [unknown.status, unknown.json.reason])
for (const [next, label] of [['//evil.example/x', 'another site'], ['https://evil.example/', 'an absolute URL']]) {
  const bad = await get(`/api/auth/external/discord/start?mode=browser&next=${encodeURIComponent(next)}`)
  check(`next pointing at ${label} is refused: 400 bad-next`, [400, 'bad-next'], [bad.status, bad.json.reason])
}
const login = await get('/login?next=%2Fsettings&error=no-email')
check('the panel page itself loads for the refusal redirect', 200, login.status)

console.log(`\n${passed} of ${passed + failures.length} passed`)
if (failures.length) process.exit(1)
