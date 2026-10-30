import { createHmac } from 'node:crypto'

const before = process.env.WALK_BEFORE ?? ''
const after = process.env.WALK_AFTER ?? ''
const beforeSecret = process.env.WALK_BEFORE_JWT_SECRET ?? ''
const afterCookie = process.env.WALK_AFTER_COOKIE ?? ''
if (!before || !after || !beforeSecret || !afterCookie) {
  console.error('WALK_BEFORE, WALK_AFTER, WALK_BEFORE_JWT_SECRET (a throwaway secret the old build was started with) and WALK_AFTER_COOKIE are required')
  process.exit(2)
}

const b64 = (value) => Buffer.from(typeof value === 'string' ? value : JSON.stringify(value)).toString('base64url')
function jwt(secret) {
  const head = b64({ alg: 'HS256', typ: 'JWT' })
  const body = b64({ nameid: '900000000000000001', unique_name: 'l6-walk', exp: Math.floor(Date.now() / 1000) + 3600, iss: 'l6-walk', aud: 'l6-walk' })
  return `${head}.${body}.${createHmac('sha256', secret).update(`${head}.${body}`).digest('base64url')}`
}
const oldToken = jwt(beforeSecret)

async function get(root, path, auth) {
  const response = await fetch(root + path, { headers: auth, redirect: 'manual' })
  const text = await response.text()
  let json
  try { json = text ? JSON.parse(text) : null } catch { json = text }
  return { status: response.status, json }
}

const canonical = (value) => JSON.stringify(value, (_, v) => (v && typeof v === 'object' && !Array.isArray(v) ? Object.fromEntries(Object.entries(v).sort(([a], [b]) => a.localeCompare(b))) : v))

const bots = (await get(after, '/api/settings', { cookie: afterCookie })).json.map((b) => b.botId)
const share = (await get(after, '/api/suggestions/sharelink', { cookie: afterCookie })).json?.code
const own = (await get(after, '/api/db/databases', { cookie: afterCookie })).json[0]
const tables = (await get(after, `/api/db/tables?schema=public&database=${own}`, { cookie: afterCookie })).json.map((t) => t.name)

const paths = ['/api/settings', '/api/suggestions', '/api/suggestions/sharelink', '/api/db/databases', `/api/db/schemas?database=${own}`, `/api/db/tables?schema=public&database=${own}`,
  '/api/db/schemas?database=postgres', '/api/db/tables?schema=spine', '/api/settings/not-a-number/theme', '/api/channels', '/api/commands', '/api/featurestatus/0/counting']
for (const bot of bots) {
  paths.push(`/api/settings/${bot}/theme`, `/api/channels?botId=${bot}`, `/api/settings/${bot}/hive`, `/api/channelwebhooks?botId=${bot}`, `/api/commands?botId=${bot}`,
    `/api/settings/${bot}/discord/channels`, `/api/settings/${bot}/discord/categories`, `/api/settings/${bot}/discord/roles`,
    `/api/featurestatus/${bot}/leveling?skip=0&take=25`, `/api/featurestatus/${bot}/counting`)
}
for (const table of tables) paths.push(`/api/db/data?schema=public&table=${table}&limit=100&database=${own}`)
if (share) paths.push(`/api/public/suggestions/${share}`)
paths.push('/api/public/suggestions/not-a-code')

let sameStatus = 0
let sameBody = 0
const differ = []
for (const path of paths) {
  const old = await get(before, path, { authorization: `Bearer ${oldToken}` })
  const now = await get(after, path, { cookie: afterCookie })
  if (old.status === now.status) sameStatus += 1
  const body = canonical(old.json) === canonical(now.json)
  if (body) sameBody += 1
  if (old.status !== now.status || !body) differ.push(`${path}: ${old.status} -> ${now.status}${body ? '' : `, body ${String(canonical(old.json)).slice(0, 120)} -> ${String(canonical(now.json)).slice(0, 120)}`}`)
}
console.log(`${paths.length} calls: ${sameStatus} same status, ${sameBody} same body`)
for (const line of differ) console.log(`  ${line}`)
process.exit(sameStatus === paths.length ? 0 : 1)
