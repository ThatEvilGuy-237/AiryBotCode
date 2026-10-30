import { execFileSync } from 'node:child_process'
import { pbkdf2Sync, randomBytes, randomUUID } from 'node:crypto'

const base = (process.env.AUTH_CHECK_URL ?? 'http://127.0.0.1:10000').replace(/\/$/, '')
const ownerLogin = process.env.AUTH_CHECK_OWNER ?? 'thatevilguy'
const ownerPassword = process.env.AUTH_CHECK_OWNER_PASSWORD ?? ''
const ownerId = 'bb098677-24cc-40c6-90f3-aa4152db9976'
const bots = Number(process.env.AUTH_CHECK_BOTS ?? 3)
if (!ownerPassword) {
  console.error('AUTH_CHECK_OWNER_PASSWORD is required: the synthetic dev owner of evil_users_dev, never a real one')
  process.exit(2)
}

const dev = (sql) => execFileSync('docker', ['exec', 'airy-postgres-db', 'psql', '-U', 'evil_users_dev_admin', '-d', 'evil_users_dev', '-At', '-v', 'ON_ERROR_STOP=1', '-c', sql], { encoding: 'utf8' }).trim()
const failures = []
let passed = 0

function check(name, want, got) {
  const ok = JSON.stringify(want) === JSON.stringify(got)
  if (ok) passed += 1
  else failures.push(name)
  console.log(`${ok ? 'PASS' : 'FAIL'} ${name}${ok ? '' : ` want ${JSON.stringify(want)} got ${JSON.stringify(got)}`}`)
}

const cookieOf = (jar) => [...jar].map(([k, v]) => `${k}=${v}`).join('; ')

function keep(jar, response) {
  for (const line of response.headers.getSetCookie()) {
    const [pair = '', ...attrs] = line.split(';')
    const at = pair.indexOf('=')
    const name = pair.slice(0, at).trim()
    const value = pair.slice(at + 1).trim()
    const gone = attrs.some((a) => /^\s*max-age=0\s*$/i.test(a) || /^\s*expires=thu, 01 jan 1970/i.test(a))
    if (gone || !value) jar.delete(name)
    else jar.set(name, value)
  }
}

async function call(path, { method = 'GET', body, jar, csrf = true, ip = '198.51.100.7' } = {}) {
  const headers = { 'cf-connecting-ip': ip }
  if (body !== undefined) headers['content-type'] = 'application/json'
  if (jar?.size) headers.cookie = cookieOf(jar)
  if (csrf && method !== 'GET' && jar?.get('__Host-evil-csrf')) headers['x-evil-csrf'] = decodeURIComponent(jar.get('__Host-evil-csrf'))
  const response = await fetch(base + path, { method, headers, body: body === undefined ? undefined : JSON.stringify(body), redirect: 'manual' })
  if (jar) keep(jar, response)
  const text = await response.text()
  let json = {}
  try { json = text ? JSON.parse(text) : {} } catch { json = {} }
  return { status: response.status, json, text, location: response.headers.get('location') ?? '', cookies: response.headers.getSetCookie() }
}

const reason = (r) => String(r.json.reason ?? '')

async function signIn(login, password, ip = '198.51.100.7') {
  const jar = new Map()
  const r = await call('/api/auth/login', { method: 'POST', body: { login, password, mode: 'browser' }, jar, ip })
  return { jar, status: r.status, cookies: r.cookies ?? [], reason: reason(r) }
}

function crowmountainHash(password) {
  const salt = randomBytes(16)
  return `100000.${salt.toString('base64')}.${pbkdf2Sync(password, salt, 100000, 32, 'sha256').toString('base64')}`
}

function person(tag, grantApp, role) {
  const id = randomUUID()
  const name = `tester-${tag}-${randomBytes(3).toString('hex')}`
  const password = randomBytes(18).toString('base64url')
  dev(`insert into evil.users (id, user_name, normalized_user_name, email, normalized_email, email_confirmed, password_hash, security_stamp, concurrency_stamp, display_name, created_at)
       values ('${id}', '${name}', upper('${name}'), '${name}@example.test', upper('${name}@example.test'), false, '${crowmountainHash(password)}', 'L6STAMP${tag.toUpperCase()}', '${randomUUID()}', 'Tester ${tag}', now())`)
  dev(`insert into evil.user_app (user_id, app_id, role, granted_at, granted_by, is_owner) values ('${id}', '${grantApp}', '${role}', now(), 'airy-l6-check', false)`)
  return { id, name, password }
}

const removePerson = (id) => dev(`begin; delete from evil.user_app where user_id = '${id}'; delete from evil.users where id = '${id}'; commit;`)

const created = []
try {
  check('/api/ready is 200 (the users database answers)', 200, (await call('/api/ready')).status)
  check('/api/health is 200', 200, (await call('/api/health')).status)
  const version = await call('/api/version')
  check('/api/version: project airybotcode, users, sessions and audit on, Discord off', ['airybotcode', true, false],
    [version.json.project, ['users', 'sessions', 'audit'].every((m) => version.json.modules?.includes(m)), version.json.modules?.includes('external.discord')])

  check('signed out: auth/me is 401', 401, (await call('/api/auth/me')).status)
  check('signed out: the bots list is 401', 401, (await call('/api/settings')).status)
  check('signed out: the database explorer is 401', 401, (await call('/api/db/databases')).status)
  check('signed out: a bot restart is 401', 401, (await call('/api/bot/reload', { method: 'POST' })).status)
  check('signed out: the panel page loads (the router sends to /login)', [200, true], await call('/').then((r) => [r.status, r.text.includes('id="app"')]))
  check('signed out: /login loads', 200, (await call('/login')).status)
  check('the public ideas board with a wrong code is 404', 404, (await call('/api/public/suggestions/not-a-code')).status)

  check('the old access-password gate is gone (404)', 404, (await call('/api/auth/gate', { method: 'POST', body: { password: 'x' } })).status)
  check('the old Discord start and redirect are gone (404)', [404, 404], [(await call('/api/auth/discord/start?state=x')).status, (await call('/api/auth/discord/redirect?code=x&state=x')).status])
  check('the device pairing routes are gone (404)', [404, 404], [(await call('/api/auth/device/start', { method: 'POST', body: {} })).status, (await call('/api/auth/device/pending')).status])
  check('Discord sign-in is off: its start is 404', 404, (await call('/api/auth/external/discord/start')).status)

  const wrong = await signIn(ownerLogin, `${ownerPassword}-wrong`, '192.0.2.44')
  check('a wrong password is 401', 401, wrong.status)

  const owner = await signIn(ownerLogin, ownerPassword)
  check('the owner signs in by user name (session and CSRF cookies)', [200, true, true], [owner.status, owner.jar.has('__Host-evil-session'), owner.jar.has('__Host-evil-csrf')])
  const flags = (owner.cookies.find((c) => c.startsWith('__Host-evil-session=')) ?? '').split(';').slice(1).map((s) => s.trim().toLowerCase())
    .filter((a) => ['httponly', 'secure', 'samesite=strict', 'path=/'].includes(a)).sort()
  check('the session cookie is HttpOnly, Secure, SameSite=Strict, Path=/', ['httponly', 'path=/', 'samesite=strict', 'secure'], flags)
  const ownerMe = await call('/api/auth/me', { jar: owner.jar })
  check('auth/me: the owner, role owner, the doc id', [200, 'owner', ownerId], [ownerMe.status, ownerMe.json.role, ownerMe.json.user?.id])
  const list = await call('/api/settings', { jar: owner.jar })
  check(`the owner sees the ${bots} bots, no token in the list`, [200, bots, false], [list.status, list.json.length, list.json.some((b) => b.token)])
  const botId = list.json[0]?.botId
  check('the owner reads a bot\'s channels and commands', [200, 200], [(await call(`/api/channels?botId=${botId}`, { jar: owner.jar })).status, (await call(`/api/commands?botId=${botId}`, { jar: owner.jar })).status])
  const databases = await call('/api/db/databases', { jar: owner.jar })
  check('the owner opens the database explorer: its own database only', [200, 1], [databases.status, databases.json.length])
  check('the explorer still refuses the users databases and other schemas (A5)', [403, 403, 403],
    [(await call('/api/db/schemas?database=evil_users_dev', { jar: owner.jar })).status, (await call('/api/db/tables?schema=evil', { jar: owner.jar })).status,
      (await call('/api/db/databases', { method: 'POST', body: { name: 'x' }, jar: owner.jar })).status])

  const noCsrf = await call('/api/suggestions', { method: 'POST', body: { title: 'L6 check', body: 'csrf' }, jar: owner.jar, csrf: false })
  check('a write without the CSRF header is 403 csrf', [403, 'csrf'], [noCsrf.status, reason(noCsrf)])
  const made = await call('/api/suggestions', { method: 'POST', body: { title: 'L6 check', body: 'csrf' }, jar: owner.jar })
  check('the same write with the header is 201', 201, made.status)
  check('... and it can be removed again', true, [200, 204].includes((await call(`/api/suggestions/${made.json.id}`, { method: 'DELETE', jar: owner.jar })).status))

  check('showing a token right after signing in needs no password', 200, (await call(`/api/settings/${botId}/token`, { method: 'POST', jar: owner.jar })).status)
  dev(`update evil.sessions set auth_time = now() - interval '10 minutes' where id = '${ownerMe.json.sessionId}'`)
  const stale = await call(`/api/settings/${botId}/token`, { method: 'POST', jar: owner.jar })
  check('10 minutes after signing in, showing a token asks for the password (401 reauthenticate)', [401, 'reauthenticate'], [stale.status, reason(stale)])
  check('... and the session itself is still signed in', 200, (await call('/api/auth/me', { jar: owner.jar })).status)
  check('a wrong password there is 400 wrong-password', [400, 'wrong-password'], await call('/api/auth/reauth', { method: 'POST', body: { password: 'nope-nope-nope' }, jar: owner.jar }).then((r) => [r.status, reason(r)]))
  check('the right one is 204', 204, (await call('/api/auth/reauth', { method: 'POST', body: { password: ownerPassword }, jar: owner.jar })).status)
  check('then the token shows', 200, (await call(`/api/settings/${botId}/token`, { method: 'POST', jar: owner.jar })).status)

  const member = person('member', 'airybotcode', 'member')
  created.push(member.id)
  const stranger = person('other', 'evil-photos', 'member')
  created.push(stranger.id)

  const refused = await signIn(stranger.name, stranger.password)
  check('only people with an airybotcode grant get in: a person with another app\'s grant is 403 no-access', [403, 'no-access'], [refused.status, refused.reason])

  const lockIp = '203.0.113.9'
  const tries = []
  for (let i = 0; i < 5; i += 1) tries.push((await signIn(member.name, 'not-the-password', lockIp)).status)
  check('lockout per address (G8): four 401s, the fifth wrong try is 429', [401, 401, 401, 401, 429], tries)
  check('... the locked address is refused with the right password too', 429, (await signIn(member.name, member.password, lockIp)).status)
  const memberIn = await signIn(member.name, member.password, '198.51.100.20')
  check('... another address signs in (the forwarded client address reaches the throttle)', 200, memberIn.status)
  check('... and the owner is not affected', 200, (await signIn(ownerLogin, ownerPassword, '198.51.100.7')).status)
  const rehashed = dev(`select left(password_hash, 6), security_stamp from evil.users where id = '${member.id}'`).split('|')
  check('the member\'s foreign hash was rehashed to Identity V3 and the stamp kept (G10)', ['AQAAAA', 'L6STAMPMEMBER'], rehashed)

  const memberMe = await call('/api/auth/me', { jar: memberIn.jar })
  check('auth/me: the member, role member', [200, 'member'], [memberMe.status, memberMe.json.role])
  check('a member uses the panel (bots list 200)', 200, (await call('/api/settings', { jar: memberIn.jar })).status)
  check('the database explorer is the owner\'s only: a member gets 403', 403, (await call('/api/db/databases', { jar: memberIn.jar })).status)

  dev(`delete from evil.user_app where user_id = '${member.id}' and app_id = 'airybotcode'`)
  const after = await call('/api/settings', { jar: memberIn.jar })
  check('access taken away ends the member\'s live session at once', true, [401, 403].includes(after.status))
  const again = await signIn(member.name, member.password, '198.51.100.20')
  check('... and refuses a new sign-in (403 no-access)', [403, 'no-access'], [again.status, again.reason])

  check('sign-out without the CSRF header is 403', 403, (await call('/api/auth/logout', { method: 'POST', jar: owner.jar, csrf: false })).status)
  const old = new Map(owner.jar)
  check('sign-out is 204', 204, (await call('/api/auth/logout', { method: 'POST', jar: owner.jar })).status)
  check('the old cookie no longer opens the panel (401)', 401, (await call('/api/settings', { jar: old })).status)
} finally {
  for (const id of created) removePerson(id)
  check('the synthetic people are gone', '0', dev(`select count(*) from evil.users where id in (${created.map((id) => `'${id}'`).join(',') || "''"})`))
}

console.log(`\n${passed} passed, ${failures.length} failed`)
process.exit(failures.length ? 1 : 0)
