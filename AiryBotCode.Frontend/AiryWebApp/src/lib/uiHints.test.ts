import { describe, it, expect } from 'vitest'
import * as h from './uiHints'

describe('template placeholders', () => {
  it('detects the hint prefix', () => {
    expect(h.isTemplate('template:user,level')).toBe(true)
    expect(h.isTemplate('text')).toBe(false)
    expect(h.isTemplate(undefined)).toBe(false)
  })

  it('parses named placeholders (token === label)', () => {
    expect(h.parsePlaceholders('template:user,level,xp')).toEqual([
      { token: 'user', label: 'user' },
      { token: 'level', label: 'level' },
      { token: 'xp', label: 'xp' },
    ])
  })

  it('parses positional token=Label pairs and trims', () => {
    expect(h.parsePlaceholders('template:0=User, 1=Minutes , 2=End Time')).toEqual([
      { token: '0', label: 'User' },
      { token: '1', label: 'Minutes' },
      { token: '2', label: 'End Time' },
    ])
  })

  it('returns [] for non-template hints and drops empties', () => {
    expect(h.parsePlaceholders('text')).toEqual([])
    expect(h.parsePlaceholders('template:user,,xp')).toEqual([
      { token: 'user', label: 'user' },
      { token: 'xp', label: 'xp' },
    ])
  })
})

describe('duration', () => {
  it('recognizes known units only', () => {
    expect(h.isDuration('duration:seconds')).toBe(true)
    expect(h.isDuration('duration:minutes')).toBe(true)
    expect(h.isDuration('duration:fortnights')).toBe(false)
    expect(h.isDuration('number')).toBe(false)
  })

  it('extracts the unit', () => {
    expect(h.durationUnit('duration:hours')).toBe('hours')
    expect(h.durationUnit('number')).toBe('')
  })

  it('humanizes the raw value in its unit', () => {
    expect(h.humanizeDuration('1440', 'duration:minutes')).toBe('= 1d')
    expect(h.humanizeDuration('300', 'duration:seconds')).toBe('= 5m')
    expect(h.humanizeDuration('90', 'duration:seconds')).toBe('= 1m 30s')
    expect(h.humanizeDuration('6', 'duration:seconds')).toBe('= 6s')
  })

  it('returns empty for non-positive / non-numeric', () => {
    expect(h.humanizeDuration('0', 'duration:seconds')).toBe('')
    expect(h.humanizeDuration('abc', 'duration:seconds')).toBe('')
  })
})

describe('slider', () => {
  it('parses min,max and defaults step to 1', () => {
    expect(h.sliderRange('slider:2,15')).toEqual({ min: 2, max: 15, step: 1 })
    expect(h.sliderRange('slider:0,10,2')).toEqual({ min: 0, max: 10, step: 2 })
  })

  it('rejects invalid / inverted bounds', () => {
    expect(h.sliderRange('slider:5,5')).toBeNull()
    expect(h.sliderRange('slider:10,2')).toBeNull()
    expect(h.sliderRange('slider:a,b')).toBeNull()
    expect(h.sliderRange('number')).toBeNull()
    expect(h.isSlider('slider:1,3')).toBe(true)
    expect(h.isSlider('slider:bad')).toBe(false)
  })
})

describe('list', () => {
  it('reads the kind', () => {
    expect(h.listKind('list:number')).toBe('number')
    expect(h.listKind('list:text')).toBe('text')
    expect(h.listKind('json')).toBeNull()
  })

  it('parses JSON arrays and rejects non-arrays', () => {
    expect(h.parseJsonArray('[10, 20]')).toEqual([10, 20])
    expect(h.parseJsonArray('')).toEqual([])
    expect(h.parseJsonArray('{"a":1}')).toBeNull()
    expect(h.parseJsonArray('not json')).toBeNull()
  })

  it('isList requires the kind AND a parseable array', () => {
    expect(h.isList('list:number', '[1,2]')).toBe(true)
    expect(h.isList('list:number', 'broken')).toBe(false) // degrades to json textarea
    expect(h.isList('json', '[1,2]')).toBe(false)
  })

  it('serializes back, coercing numbers for list:number', () => {
    expect(h.serializeList(['10', '20'], 'number')).toBe('[10,20]')
    expect(h.serializeList(['a', 'b'], 'text')).toBe('["a","b"]')
  })

  it('round-trips a number list', () => {
    const items = h.listItems('[10,20,50]')
    expect(items).toEqual(['10', '20', '50'])
    expect(h.serializeList([...items, '100'], 'number')).toBe('[10,20,50,100]')
  })
})

describe('keyvalue', () => {
  it('reads the kind', () => {
    expect(h.keyvalueKind('keyvalue:number')).toBe('number')
    expect(h.keyvalueKind('keyvalue:text')).toBe('text')
    expect(h.keyvalueKind('json')).toBeNull()
  })

  it('parses a JSON object into rows; rejects non-objects', () => {
    expect(h.parseKeyValue('{"1 hour":60,"5 hours":300}')).toEqual([
      { key: '1 hour', val: '60' },
      { key: '5 hours', val: '300' },
    ])
    expect(h.parseKeyValue('')).toEqual([])
    expect(h.parseKeyValue('[1,2]')).toBeNull()
    expect(h.parseKeyValue('42')).toBeNull()
    expect(h.parseKeyValue('nope')).toBeNull()
  })

  it('isKeyValue requires the kind AND a parseable object', () => {
    expect(h.isKeyValue('keyvalue:number', '{"a":1}')).toBe(true)
    expect(h.isKeyValue('keyvalue:number', '[1]')).toBe(false) // degrades to json
    expect(h.isKeyValue('json', '{"a":1}')).toBe(false)
  })

  it('serializes rows back, coercing numbers and dropping blank keys', () => {
    expect(h.serializeKeyValue([{ key: 'a', val: '1' }, { key: 'b', val: '2' }], 'number')).toBe('{"a":1,"b":2}')
    expect(h.serializeKeyValue([{ key: 'x', val: 'hi' }], 'text')).toBe('{"x":"hi"}')
    expect(h.serializeKeyValue([{ key: '  ', val: '9' }, { key: 'k', val: '3' }], 'number')).toBe('{"k":3}')
  })

  it('round-trips a label→minutes map', () => {
    const rows = h.parseKeyValue('{"1 hour ago":60,"24 hours":1440}')!
    expect(h.serializeKeyValue(rows, 'number')).toBe('{"1 hour ago":60,"24 hours":1440}')
  })
})

describe('emoji', () => {
  it('detects the hint and exposes quick-picks', () => {
    expect(h.isEmoji('emoji')).toBe(true)
    expect(h.isEmoji('text')).toBe(false)
    expect(h.COMMON_EMOJI).toContain('✅')
    expect(h.COMMON_EMOJI.length).toBeGreaterThan(0)
  })
})
