import { describe, expect, it } from 'vitest'
import { changed, commandName, controlFor, groups, settingName, type Setting } from '../commands'

const setting = (key: string, uiHint: string, value = '', category = 'General'): Setting =>
  ({ key, value, uiHint, category, description: null, isReloadable: true })

describe('command names and labels', () => {
  it('reads command and setting names as words', () => {
    expect(commandName('CountingCommand')).toBe('Counting')
    expect(commandName('SpamCatcherCommand')).toBe('Spam catcher')
    expect(commandName('UserlogsCommand')).toBe('User logs')
    expect(settingName('BossTimeoutSeconds')).toBe('Boss timeout seconds')
    expect(settingName('LevelUpChannelId')).toBe('Level up channel')
    expect(settingName('NoXpChannelIds')).toBe('No XP channels')
    expect(settingName('MonitoredRoleIds')).toBe('Monitored roles')
  })

  it('picks a control per hint and falls back to text for a list that is not valid JSON', () => {
    expect(controlFor(setting('A', 'boolean'))).toBe('toggle')
    expect(controlFor(setting('A', 'duration:minutes'))).toBe('duration')
    expect(controlFor(setting('A', 'slider:2,15'))).toBe('slider')
    expect(controlFor(setting('A', 'template:next'))).toBe('template')
    expect(controlFor(setting('A', 'roles', '[1,2]'))).toBe('roles')
    expect(controlFor(setting('A', 'list:number', '[1,2]'))).toBe('list')
    expect(controlFor(setting('A', 'list:number', 'oops'))).toBe('text')
    expect(controlFor(setting('A', 'keyvalue:text', '{"a":"b"}'))).toBe('keyvalue')
    expect(controlFor(setting('A', 'emoji'))).toBe('emoji')
    expect(controlFor(setting('A', ''))).toBe('text')
  })

  it('groups by category and sends only what changed', () => {
    const list = [setting('A', 'number', '1', 'Counting'), setting('B', 'number', '2', ''), setting('C', 'number', '3', 'Counting')]
    expect(groups(list).map(g => [g.name, g.settings.length])).toEqual([['Counting', 2], ['General', 1]])
    expect(changed(list, { A: '1', B: '5' })).toEqual([{ key: 'B', value: '5' }])
  })
})
