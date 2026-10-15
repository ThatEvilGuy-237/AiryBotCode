<script setup lang="ts">
import { computed, ref } from 'vue'
import { Button, Field, Icon, MultiSelect, NumberField, Select, Slider, TagInput, Textarea, TextField, Toggle } from '@airy/ui'
import ChannelPicker from '@/components/ChannelPicker.vue'
import type { DiscordEntity } from '@/lib/api'
import { controlFor, type Setting } from '@/lib/commands'
import { parseIdList, parseRoleRewards, serializeIdList, serializeRoleRewards, type RewardRow } from '@/lib/discordIds'
import {
  COMMON_EMOJI, durationUnit, humanizeDuration, keyvalueKind, listItems, listKind, parseKeyValue, parsePlaceholders,
  serializeKeyValue, serializeList, sliderRange, type KvRow,
} from '@/lib/uiHints'

const props = defineProps<{
  setting: Setting
  modelValue: string
  label: string
  channels: DiscordEntity[]
  categories: DiscordEntity[]
  roles: DiscordEntity[]
}>()
const emit = defineEmits<{ 'update:modelValue': [value: string] }>()

const control = computed(() => controlFor({ uiHint: props.setting.uiHint, value: props.setting.value }))
const value = computed({ get: () => props.modelValue, set: (v: string) => emit('update:modelValue', v) })

const many = (list: DiscordEntity[]) => new Set(list.map(e => e.guild)).size > 1
const optionsOf = (list: DiscordEntity[], keep: string[]) => {
  const known = list.map(e => ({ label: many(list) ? `${e.name} · ${e.guild}` : e.name, value: e.id }))
  const unknown = keep.filter(id => id && id !== '0' && !list.some(e => e.id === id)).map(id => ({ label: `Unknown (${id})`, value: id }))
  return [...known, ...unknown]
}
const listFor = computed(() => (control.value === 'channel' || control.value === 'channels' ? props.channels : control.value === 'category' ? props.categories : props.roles))
const singleOptions = computed(() => [{ label: 'None', value: '0' }, ...optionsOf(listFor.value, [value.value])])
const single = computed({ get: () => (value.value && value.value !== '' ? value.value : '0'), set: (v: string) => (value.value = v) })
const ids = computed({ get: () => parseIdList(value.value), set: (v: string[]) => (value.value = serializeIdList(v)) })
const manyOptions = computed(() => optionsOf(listFor.value, ids.value))

const toggle = computed({ get: () => value.value.trim().toLowerCase() === 'true', set: (v: boolean) => (value.value = v ? 'true' : 'false') })
const number = computed({
  get: () => (value.value.trim() === '' || !Number.isFinite(Number(value.value)) ? null : Number(value.value)),
  set: (v: number | null) => (value.value = v === null ? '' : String(v)),
})
const range = computed(() => sliderRange(props.setting.uiHint ?? '') ?? { min: 0, max: 1, step: 1 })
const sliderValue = computed({ get: () => number.value ?? range.value.min, set: (v: number) => (value.value = String(v)) })
const unit = computed(() => durationUnit(props.setting.uiHint ?? ''))
const spelled = computed(() => humanizeDuration(value.value, props.setting.uiHint ?? ''))

const placeholders = computed(() => parsePlaceholders(props.setting.uiHint ?? ''))
const box = ref<HTMLElement | null>(null)
function insert(token: string) {
  const area = box.value?.querySelector('textarea')
  const text = `{${token}}`
  if (!area) {
    value.value += text
    return
  }
  const start = area.selectionStart ?? value.value.length
  const end = area.selectionEnd ?? start
  value.value = value.value.slice(0, start) + text + value.value.slice(end)
  requestAnimationFrame(() => {
    area.focus()
    area.setSelectionRange(start + text.length, start + text.length)
  })
}

const items = computed({
  get: () => listItems(value.value),
  set: (v: string[]) => {
    const kind = listKind(props.setting.uiHint ?? '')
    value.value = serializeList(kind === 'number' ? v.filter(x => x.trim() !== '' && Number.isFinite(Number(x))) : v, kind)
  },
})

const rewards = ref<RewardRow[]>(parseRoleRewards(props.modelValue))
function saveRewards() {
  value.value = serializeRoleRewards(rewards.value)
}
const roleOptions = computed(() => optionsOf(props.roles, rewards.value.map(r => r.roleId)))

const pairs = ref<KvRow[]>(parseKeyValue(props.modelValue) ?? [])
function savePairs() {
  value.value = serializeKeyValue(pairs.value, keyvalueKind(props.setting.uiHint ?? ''))
}
</script>

<template>
  <div class="control">
    <Toggle v-if="control === 'toggle'" v-model="toggle" :label="label" />

    <NumberField v-else-if="control === 'number'" v-model="number" allow-empty :aria-label="label" />

    <div v-else-if="control === 'duration'" class="row">
      <NumberField v-model="number" :min="0" :unit="unit" allow-empty :aria-label="label" />
      <span class="aside">{{ spelled }}</span>
    </div>

    <div v-else-if="control === 'slider'" class="row">
      <Slider v-model="sliderValue" :min="range.min" :max="range.max" :step="range.step" :label="label" class="grow" />
      <span class="aside mono">{{ sliderValue }}</span>
    </div>

    <Textarea v-else-if="control === 'textarea'" v-model="value" :rows="3" />
    <Textarea v-else-if="control === 'json'" v-model="value" :rows="4" mono />

    <div v-else-if="control === 'template'" ref="box" class="stack">
      <Textarea v-model="value" :rows="3" />
      <div v-if="placeholders.length" class="chips" role="group" :aria-label="`Insert into ${label}`">
        <Button v-for="p in placeholders" :key="p.token" size="sm" variant="outline" @click="insert(p.token)">
          <Icon name="plus" :size="14" />{{ p.label }}
        </Button>
      </div>
    </div>

    <div v-else-if="control === 'emoji'" class="stack">
      <TextField v-model="value" />
      <div class="chips" role="group" :aria-label="`Pick an emoji for ${label}`">
        <Button v-for="e in COMMON_EMOJI" :key="e" size="sm" variant="ghost" :aria-label="`Use ${e}`" :aria-pressed="value === e" @click="value = e">{{ e }}</Button>
      </div>
    </div>

    <ChannelPicker v-else-if="control === 'channel'" :model-value="single" :channels="channels" none @update:model-value="v => (single = v ?? '0')" />
    <Select v-else-if="control === 'category' || control === 'role'" v-model="single" :options="singleOptions" />

    <MultiSelect v-else-if="control === 'channels' || control === 'roles'" v-model="ids" :options="manyOptions" :placeholder="control === 'channels' ? 'No channels' : 'No roles'" clearable :aria-label="label" />

    <div v-else-if="control === 'roleRewards'" class="stack">
      <div v-for="(r, i) in rewards" :key="i" class="pair">
        <NumberField :model-value="r.level === '' ? null : Number(r.level)" :min="1" :aria-label="`Level of reward ${i + 1}`" @update:model-value="v => { r.level = v === null ? '' : String(v); saveRewards() }" />
        <Field :label="`Role of reward ${i + 1}`" hide-label>
          <Select :model-value="r.roleId" :options="roleOptions" placeholder="Pick a role" @update:model-value="v => { r.roleId = v; saveRewards() }" />
        </Field>
        <Button variant="ghost" size="sm" :aria-label="`Remove reward ${i + 1}`" @click="rewards.splice(i, 1); saveRewards()"><Icon name="trash-2" :size="16" /></Button>
      </div>
      <div><Button size="sm" variant="outline" @click="rewards.push({ level: '', roleId: '' })"><Icon name="plus" :size="14" />Add a reward</Button></div>
    </div>

    <TagInput v-else-if="control === 'list'" v-model="items" placeholder="Add and press Enter" />

    <div v-else-if="control === 'keyvalue'" class="stack">
      <div v-for="(p, i) in pairs" :key="i" class="pair">
        <Field :label="`Name ${i + 1}`" hide-label><TextField v-model="p.key" @update:model-value="savePairs" /></Field>
        <Field :label="`Value ${i + 1}`" hide-label><TextField v-model="p.val" @update:model-value="savePairs" /></Field>
        <Button variant="ghost" size="sm" :aria-label="`Remove row ${i + 1}`" @click="pairs.splice(i, 1); savePairs()"><Icon name="trash-2" :size="16" /></Button>
      </div>
      <div><Button size="sm" variant="outline" @click="pairs.push({ key: '', val: '' })"><Icon name="plus" :size="14" />Add a row</Button></div>
    </div>

    <TextField v-else v-model="value" />
  </div>
</template>

<style scoped>
.control {
  min-width: 0;
}
.row {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}
.grow {
  flex: 1;
}
.aside {
  color: var(--color-muted);
  font-size: var(--font-size-sm);
  white-space: nowrap;
}
.mono {
  font-family: var(--font-mono);
  min-width: 3ch;
  text-align: right;
}
.stack {
  display: grid;
  gap: var(--space-2);
}
.chips {
  display: flex;
  flex-wrap: wrap;
  gap: var(--space-1);
}
.pair {
  display: grid;
  grid-template-columns: minmax(0, 120px) minmax(0, 1fr) auto;
  gap: var(--space-2);
  align-items: center;
}
</style>
