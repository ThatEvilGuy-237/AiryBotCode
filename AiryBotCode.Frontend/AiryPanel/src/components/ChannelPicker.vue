<script setup lang="ts">
import { computed, onBeforeUnmount, ref } from 'vue'
import { Autocomplete, type AutocompleteOption } from '@airy/ui'
import type { DiscordEntity } from '@/lib/api'

const props = withDefaults(defineProps<{
  modelValue: string | null
  channels: DiscordEntity[]
  exclude?: string[]
  placeholder?: string
  none?: boolean
  disabled?: boolean
}>(), { exclude: () => [], placeholder: 'Find a channel', none: false, disabled: false })
const emit = defineEmits<{ 'update:modelValue': [value: string | null] }>()

const wideQuery = window.matchMedia('(min-width: 521px)')
const wide = ref(wideQuery.matches)
const onWidth = (e: MediaQueryListEvent) => (wide.value = e.matches)
wideQuery.addEventListener('change', onWidth)
onBeforeUnmount(() => wideQuery.removeEventListener('change', onWidth))

const pool = computed(() => props.channels.filter(c => !props.exclude.includes(c.id)))
const many = computed(() => new Set(props.channels.map(c => c.guild)).size > 1)
const option = (c: DiscordEntity): AutocompleteOption => ({ label: c.name, value: c.id, meta: many.value ? c.guild : undefined })
const noneOption: AutocompleteOption = { label: 'None', value: '0' }

async function find(query: string): Promise<AutocompleteOption[]> {
  const q = query.trim().replace(/^#/, '').toLowerCase()
  const hits = pool.value.filter(c => !q || c.name.toLowerCase().includes(q) || c.guild.toLowerCase().includes(q)).slice(0, 120).map(option)
  return props.none && !q ? [noneOption, ...hits] : hits
}

async function resolve(value: string): Promise<AutocompleteOption | null> {
  if (value === '0') return props.none ? noneOption : null
  const hit = props.channels.find(c => c.id === value)
  return hit ? option(hit) : { label: `Unknown (${value})`, value, meta: 'not found' }
}

const value = computed({
  get: () => (props.modelValue && props.modelValue !== '' ? props.modelValue : null),
  set: (v: string | null) => emit('update:modelValue', v),
})
</script>

<template>
  <Autocomplete
    :key="channels.length"
    v-model="value"
    :fetch-options="find"
    :resolve-option="resolve"
    :min-chars="0"
    :debounce="0"
    :cache="false"
    :status="false"
    :badge-shows="wide ? 'meta' : 'none'"
    open-on-focus
    :placeholder="placeholder"
    :disabled="disabled || !channels.length"
    no-match="No channel by that name"
    list-max-height="320px"
  />
</template>
