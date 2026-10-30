import { execFileSync } from 'node:child_process'
import { pbkdf2Sync, randomBytes, randomUUID } from 'node:crypto'
import http from 'node:http'

const entry = new URL(process.env.PROD_CHECK_ENTRY ?? 'http://127.0.0.1:8088')
const host = process.env.PROD_CHECK_HOST ?? 'airy.thatevilguy.com'
const client = process.env.PROD_CHECK_CLIENT ?? '198.51.100.61'
const users = (sql) => execFileSync('docker', ['exec', 'airy-postgres-db', 'psql', '-U', 'evil_users_admin', '-d', 'evil_users', '-At', '-v', 'ON_ERROR_STOP=1', '-c', sql], { encoding: 'utf8' }).trim()
const failures = []
let passed = 0

function check(name, want, got) {
  const ok = JSON.stringify(want) === JSON.stringify(got)
  if (ok) passed += 1
  else failures.push(name)
  console.log(`${ok ? 'PASS' : 'FAIL'} ${name}${ok ? '' : ` want ${JSON.stringify(want)} got ${JSON.stringify(got)}`}`)
}

function call(path, { method = 'GET', body, jar, csrf = true } = {}) {
  const headers = { host, 'cf-connecting-ip': client, 'x-forwarded-proto': 'https' }
  const payload = body === undefined ? undefined : JSON.stringify(body)
  if (payload) headers['content-type'] = 'application/json'
  if (jar?.size) headers.cookie = [...jar].map(([k, v]) => `${k}=${v}`).join('; ')
  if (csrf && method !== 'GET' && jar?.get('__Host-evil-csrf')) headers['x-evil-csrf'] = decodeURIComponent(jar.get('__Host-evil-csrf'))
  return new Promise((resolve, reject) => {
    const request = http.request({ host: entry.hostname, port: entry.port, path, method, headers }, (response) => {
      let text = ''
      response.on('data', (chunk) => { text += chunk })
      response.on('end', () => {
        const cookies = [response.headers['set-cookie'] ?? []].flat()
        if (jar) for (const line of cookies) {
          const [pair] = line.split(';')
          const at = pair.indexOf('=')
          const value = pair.slice(at + 1)
          if (!value || /max-age=0/i.test(line)) jar.delete(pair.slice(0, at))
          else jar.set(pair.slice(0, at), value)
        }
        let json = {}
        try { json = text ? JSON.parse(text) : {} } catch { json = {} }
        resolve({ status: response.statusCode, json, text, cookies })
      })
    })
    request.on('error', reject)
    if (payload) request.write(payload)
    request.end()
  })
}

const reason = (r) => String(r.json.reason ?? '')
const id = randomUUID()
const name = `tester-${randomBytes(4).toString('hex')}`
const password = randomBytes(18).toString('base64url')
const salt = randomBytes(16)
const hash = `100000.${salt.toString('base64')}.${pbkdf2Sync(password, salt, 100000, 32, 'sha256').toString('base64')}`

check('/api/ready is 200', 200, (await call('/api/ready')).status)
check('/api/health is 200', 200, (await call('/api/health')).status)
const version = await call('/api/version')
check('/api/version: airybotcode on the users database, Discord off', ['airybotcode', true, false], [version.json.project, version.json.modules?.includes('users'), version.json.modules?.includes('external.discord')])
check('signed out: auth/me is 401', 401, (await call('/api/auth/me')).status)
check('signed out: the bots list is 401', 401, (await call('/api/settings')).status)
check('signed out: the panel page loads', [200, true], await call('/login').then((r) => [r.status, r.text.includes('id="app"')]))
check('the old gate is gone', 404, (await call('/api/auth/gate', { method: 'POST', body: { password: 'x' } })).status)

users(`insert into evil.users (id, user_name, normalized_user_name, email, normalized_email, email_confirmed, password_hash, security_stamp, concurrency_stamp, display_name, created_at)
       values ('${id}', '${name}', upper('${name}'), '${name}@example.test', upper('${name}@example.test'), false, '${hash}', 'L6PRODSTAMP', '${randomUUID()}', 'Tester', now())`)
users(`insert into evil.user_app (user_id, app_id, role, granted_at, granted_by, is_owner) values ('${id}', 'airybotcode', 'member', now(), 'airy-l6-prod-check', false)`)
try {
  const jar = new Map()
  const signIn = await call('/api/auth/login', { method: 'POST', body: { login: name, password, mode: 'browser' }, jar })
  check('a synthetic person with a grant on airybotcode only signs in', 200, signIn.status)
  const flags = (signIn.cookies.find((c) => c.startsWith('__Host-evil-session=')) ?? '').split(';').slice(1).map((s) => s.trim().toLowerCase())
    .filter((a) => ['httponly', 'secure', 'samesite=strict', 'path=/'].includes(a)).sort()
  check('the session cookie is HttpOnly, Secure, SameSite=Strict, Path=/', ['httponly', 'path=/', 'samesite=strict', 'secure'], flags)
  const me = await call('/api/auth/me', { jar })
  check('auth/me: member', [200, 'member'], [me.status, me.json.role])
  const bots = await call('/api/settings', { jar })
  check('one real call: the bots list answers from airy_db with the new role', [200, true], [bots.status, Array.isArray(bots.json) && bots.json.length > 0])
  check('the database explorer is owner-only (403)', 403, (await call('/api/db/databases', { jar })).status)
  check('a write without CSRF is 403', [403, 'csrf'], await call('/api/auth/logout', { method: 'POST', jar, csrf: false }).then((r) => [r.status, reason(r)]))
  check('the foreign hash was rehashed with the stamp kept', ['AQAAAA', 'L6PRODSTAMP'], users(`select left(password_hash, 6), security_stamp from evil.users where id = '${id}'`).split('|'))
  const block = `v4:${Buffer.from(client.split('.').map(Number)).toString('hex')}`
  check('the API saw the forwarded client address, not Caddy\'s (known address block)', block, users(`select string_agg(address_block, ',') from evil.known_addresses where user_id = '${id}'`))
  const old = new Map(jar)
  check('sign-out is 204', 204, (await call('/api/auth/logout', { method: 'POST', jar })).status)
  check('the old cookie is 401 after', 401, (await call('/api/settings', { jar: old })).status)
} finally {
  users(`begin; delete from evil.user_app where user_id = '${id}'; delete from evil.users where id = '${id}'; commit;`)
  check('the synthetic person is deleted', '0', users(`select count(*) from evil.users where id = '${id}'`))
}

console.log(`\n${passed} passed, ${failures.length} failed`)
process.exit(failures.length ? 1 : 0)
