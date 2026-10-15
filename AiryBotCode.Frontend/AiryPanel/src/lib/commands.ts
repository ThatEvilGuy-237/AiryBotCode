import { isDuration, isEmoji, isKeyValue, isList, isSlider, isTemplate } from './uiHints'

export type Setting = {
  key: string
  value: string
  description: string | null
  category: string | null
  uiHint: string | null
  isReloadable: boolean
}

export type Command = { commandName: string; enabled: boolean; settings: Setting[] }

export type Control =
  | 'toggle' | 'number' | 'duration' | 'slider' | 'text' | 'textarea' | 'json' | 'template' | 'emoji'
  | 'channel' | 'category' | 'role' | 'channels' | 'roles' | 'roleRewards' | 'list' | 'keyvalue'

const names: Record<string, string> = {
  Userlogs: 'User logs',
  Untimeout: 'End a timeout',
  ContactUser: 'Contact a user',
  VerifyUserAge: 'Age check',
  SpamCatcher: 'Spam catcher',
  HiveChat: 'Hive chat',
}

const words = (s: string) => s.replace(/([a-z0-9])([A-Z])/g, '$1 $2').replace(/([A-Z]+)([A-Z][a-z])/g, '$1 $2').split(' ')

export function commandName(raw: string): string {
  const base = raw.replace(/Command$/, '')
  if (names[base]) return names[base]
  const w = words(base)
  return [w[0], ...w.slice(1).map(x => x.toLowerCase())].join(' ')
}

export function settingName(key: string): string {
  const w = words(key.replace(/Ids?$/, m => (m === 'Ids' ? 's' : '')))
    .map(x => (/^(Xp|Id|Url|Api)$/.test(x) ? x.toUpperCase() : x))
  const text = [w[0], ...w.slice(1).map(x => (/^[A-Z]{2,}$/.test(x) ? x : x.toLowerCase()))].join(' ')
  return text.replace(/ s$/, 's')
}

export function controlFor(s: Pick<Setting, 'uiHint' | 'value'>): Control {
  const hint = s.uiHint ?? ''
  if (hint === 'boolean') return 'toggle'
  if (hint === 'channel' || hint === 'category' || hint === 'role' || hint === 'channels' || hint === 'roles' || hint === 'roleRewards') return hint
  if (isTemplate(hint)) return 'template'
  if (isDuration(hint)) return 'duration'
  if (isSlider(hint)) return 'slider'
  if (isEmoji(hint)) return 'emoji'
  if (isList(hint, s.value)) return 'list'
  if (isKeyValue(hint, s.value)) return 'keyvalue'
  if (hint === 'json') return 'json'
  if (hint === 'textarea') return 'textarea'
  if (hint === 'number') return 'number'
  return 'text'
}

export function groups(settings: Setting[]): { name: string; settings: Setting[] }[] {
  const byName = new Map<string, Setting[]>()
  for (const s of settings) {
    const name = s.category?.trim() || 'General'
    byName.set(name, [...(byName.get(name) ?? []), s])
  }
  return [...byName.entries()].map(([name, list]) => ({ name: settingName(name), settings: list }))
}

export function changed(before: Setting[], after: Record<string, string>): { key: string; value: string }[] {
  return before.filter(s => after[s.key] !== undefined && after[s.key] !== s.value).map(s => ({ key: s.key, value: after[s.key] }))
}
