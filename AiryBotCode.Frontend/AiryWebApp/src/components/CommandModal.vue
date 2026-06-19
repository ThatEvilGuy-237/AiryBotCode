<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { Button, Toggle } from '@hive/ui'
import { api, type CommandConfig, type CommandSetting, type DiscordEntity } from '../lib/api'
import {
  parseIdList,
  serializeIdList,
  parseRoleRewards,
  serializeRoleRewards,
  type RewardRow,
} from '../lib/discordIds'

const props = defineProps<{ command: CommandConfig; botId?: string | null }>()
const emit = defineEmits<{
  (e: 'save', settings: { key: string; value: string }[]): void
  (e: 'close'): void
}>()

// Local editable clone so edits don't mutate the parent until saved.
const draft = ref<CommandConfig>(JSON.parse(JSON.stringify(props.command)))
const saving = ref(false)

// Channel/role pickers — fetched lazily, and only if this command actually has a
// setting that needs them. On any failure the lists stay empty and those fields
// gracefully fall back to a plain id input (see usePicker / optionsFor).
const channels = ref<DiscordEntity[]>([])
const categories = ref<DiscordEntity[]>([])
const roles = ref<DiscordEntity[]>([])

const channelHints = ['channel', 'channels']
const roleHints = ['role', 'roles', 'roleRewards']
const needsChannels = computed(() => draft.value.settings.some((s) => channelHints.includes(s.uiHint)))
const needsCategories = computed(() => draft.value.settings.some((s) => s.uiHint === 'category'))
const needsRoles = computed(() => draft.value.settings.some((s) => roleHints.includes(s.uiHint)))

// Reward rows are edited as structured state per setting key, then serialized back
// into the setting's value string on every change (precision-safe via the helpers).
const rewardRows = ref<Record<string, RewardRow[]>>({})

onMounted(async () => {
  for (const s of draft.value.settings) {
    if (s.uiHint === 'roleRewards') rewardRows.value[s.key] = parseRoleRewards(s.value)
  }
  if (!props.botId) return
  const jobs: Promise<void>[] = []
  if (needsChannels.value) jobs.push(api.getBotChannels(props.botId).then((r) => { channels.value = r }))
  if (needsCategories.value) jobs.push(api.getBotCategories(props.botId).then((r) => { categories.value = r }))
  if (needsRoles.value) jobs.push(api.getBotRoles(props.botId).then((r) => { roles.value = r }))
  await Promise.all(jobs)
})

function listFor(setting: CommandSetting): DiscordEntity[] {
  if (channelHints.includes(setting.uiHint)) return channels.value
  if (setting.uiHint === 'category') return categories.value
  if (roleHints.includes(setting.uiHint)) return roles.value
  return []
}

// Single-id dropdown (channel/category/role) — only when we actually fetched options.
function usePicker(setting: CommandSetting): boolean {
  return ['channel', 'category', 'role'].includes(setting.uiHint) && listFor(setting).length > 0
}

// Multi-id checklist (channels/roles) — same fetch gate.
function useMulti(setting: CommandSetting): boolean {
  return (setting.uiHint === 'channels' || setting.uiHint === 'roles') && listFor(setting).length > 0
}

const multiGuild = (list: DiscordEntity[]) => new Set(list.map((e) => e.guild)).size > 1

// --- multi-select (id list) state, kept precision-safe via discordIds helpers ---
function isChecked(setting: CommandSetting, id: string): boolean {
  return parseIdList(setting.value).includes(id)
}

function toggleMulti(setting: CommandSetting, id: string, on: boolean) {
  const ids = parseIdList(setting.value).filter((x) => x !== id)
  if (on) ids.push(id)
  setting.value = serializeIdList(ids)
}

// Options for a multi-select: the live list plus any already-selected id we didn't
// get back from Discord (so an existing config is never silently dropped).
function multiOptions(setting: CommandSetting): { id: string; label: string }[] {
  const list = listFor(setting)
  const showGuild = multiGuild(list)
  const opts = list.map((e) => ({ id: e.id, label: showGuild ? `${e.name} · ${e.guild}` : e.name }))
  for (const id of parseIdList(setting.value)) {
    if (!list.some((e) => e.id === id)) opts.push({ id, label: `id ${id} (not found)` })
  }
  return opts
}

// --- role-reward map editor (level -> role) ---
function useRewards(setting: CommandSetting): boolean {
  return setting.uiHint === 'roleRewards' && roles.value.length > 0
}

function rowsFor(setting: CommandSetting): RewardRow[] {
  return (rewardRows.value[setting.key] ??= [])
}

function syncRewards(setting: CommandSetting) {
  setting.value = serializeRoleRewards(rowsFor(setting))
}

function addReward(setting: CommandSetting) {
  rowsFor(setting).push({ level: '', roleId: '' })
}

function removeReward(setting: CommandSetting, index: number) {
  rowsFor(setting).splice(index, 1)
  syncRewards(setting)
}

// Role <select> options, with the current value preserved if Discord didn't return it.
function rewardRoleOptions(roleId: string): { value: string; label: string }[] {
  const showGuild = multiGuild(roles.value)
  const opts = roles.value.map((e) => ({ value: e.id, label: showGuild ? `${e.name} · ${e.guild}` : e.name }))
  if (roleId && !roles.value.some((e) => e.id === roleId)) {
    opts.push({ value: roleId, label: `id ${roleId} (not found)` })
  }
  return [{ value: '', label: '— select role —' }, ...opts]
}

// Options for a picker: a "None" entry, then the live list, plus the current value
// if it's an id we didn't get back (so an existing config is never silently lost).
function optionsFor(setting: CommandSetting): { value: string; label: string }[] {
  const list = listFor(setting)
  const showGuild = multiGuild(list)
  const opts = list.map((e) => ({ value: e.id, label: showGuild ? `${e.name} · ${e.guild}` : e.name }))
  const cur = setting.value
  if (cur && cur !== '0' && !list.some((e) => e.id === cur)) {
    opts.push({ value: cur, label: `id ${cur} (not found)` })
  }
  return [{ value: '0', label: '— None —' }, ...opts]
}

// Group settings by their declared Category so long commands (e.g. Counting's
// 14 settings) read as labelled sections instead of one flat wall of fields.
// Order is preserved (first-seen category wins); empty category → "General".
const groups = computed(() => {
  const map = new Map<string, CommandSetting[]>()
  for (const s of draft.value.settings) {
    const cat = s.category?.trim() || 'General'
    if (!map.has(cat)) map.set(cat, [])
    map.get(cat)!.push(s)
  }
  return [...map.entries()].map(([name, settings]) => ({ name, settings }))
})

// Only show section headers when there's real grouping to show.
const showHeaders = computed(
  () => groups.value.length > 1 || (groups.value[0]?.name ?? 'General') !== 'General',
)

function save() {
  saving.value = true
  emit(
    'save',
    draft.value.settings.map((s) => ({ key: s.key, value: s.value })),
  )
}
</script>

<template>
  <div class="overlay" @click.self="emit('close')">
    <div class="modal" role="dialog" aria-modal="true">
      <header class="modal-header">
        <h2>{{ draft.commandName }}</h2>
        <button class="close-btn" aria-label="Close" @click="emit('close')">✕</button>
      </header>

      <div class="modal-body">
        <section v-for="group in groups" :key="group.name" class="group">
          <h3 v-if="showHeaders" class="group-title">{{ group.name }}</h3>

          <div v-for="setting in group.settings" :key="setting.key" class="field">
            <div class="label-row">
              <label :for="`f-${setting.key}`">{{ setting.key }}</label>
              <span class="badge" :class="{ reloadable: setting.isReloadable }">
                {{ setting.isReloadable ? 'auto-applies' : 'needs reload' }}
              </span>
            </div>

            <select
              v-if="usePicker(setting)"
              :id="`f-${setting.key}`"
              v-model="setting.value"
              class="picker"
            >
              <option v-for="o in optionsFor(setting)" :key="o.value" :value="o.value">{{ o.label }}</option>
            </select>
            <div v-else-if="useMulti(setting)" class="checklist">
              <label v-for="o in multiOptions(setting)" :key="o.id" class="check">
                <input
                  type="checkbox"
                  :checked="isChecked(setting, o.id)"
                  @change="toggleMulti(setting, o.id, ($event.target as HTMLInputElement).checked)"
                />
                <span>{{ o.label }}</span>
              </label>
            </div>
            <div v-else-if="useRewards(setting)" class="rewards">
              <div v-for="(row, i) in rowsFor(setting)" :key="i" class="reward-row">
                <input
                  v-model="row.level"
                  type="number"
                  min="1"
                  class="reward-level"
                  placeholder="Lvl"
                  @input="syncRewards(setting)"
                />
                <span class="reward-arrow">→</span>
                <select v-model="row.roleId" class="picker reward-role" @change="syncRewards(setting)">
                  <option v-for="o in rewardRoleOptions(row.roleId)" :key="o.value" :value="o.value">{{ o.label }}</option>
                </select>
                <button type="button" class="reward-del" title="Remove" @click="removeReward(setting, i)">✕</button>
              </div>
              <button type="button" class="reward-add" @click="addReward(setting)">+ Add reward</button>
            </div>
            <textarea
              v-else-if="setting.uiHint === 'textarea' || setting.uiHint === 'json'"
              :id="`f-${setting.key}`"
              v-model="setting.value"
              rows="4"
              :class="{ mono: setting.uiHint === 'json' }"
            ></textarea>
            <input
              v-else-if="setting.uiHint === 'number'"
              :id="`f-${setting.key}`"
              v-model="setting.value"
              type="number"
            />
            <div v-else-if="setting.uiHint === 'boolean'" class="switch">
              <Toggle
                :modelValue="setting.value === 'true'"
                @update:modelValue="(v: boolean) => (setting.value = v ? 'true' : 'false')"
              />
              <span>{{ setting.value === 'true' ? 'Enabled' : 'Disabled' }}</span>
            </div>
            <input v-else :id="`f-${setting.key}`" v-model="setting.value" type="text" />

            <p class="help">{{ setting.description }}</p>
          </div>
        </section>

        <p v-if="draft.settings.length === 0" class="empty">No configurable settings.</p>
      </div>

      <footer class="modal-footer">
        <Button variant="outline" @click="emit('close')">Cancel</Button>
        <Button :disabled="saving" @click="save">{{ saving ? 'Saving…' : 'Save' }}</Button>
      </footer>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  background: rgba(58, 44, 54, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1.5rem;
  z-index: 100;
}
.modal {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: 16px;
  width: 100%;
  max-width: 560px;
  max-height: 85vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 24px 60px rgba(0, 0, 0, 0.22);
}
.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1.5rem 0.75rem;
}
.modal-header h2 {
  margin: 0;
  font-size: 1.25rem;
  color: var(--color-accent);
}
.close-btn {
  background: transparent;
  border: none;
  font-size: 1.1rem;
  cursor: pointer;
  color: var(--color-muted);
  padding: 0.25rem 0.5rem;
  border-radius: 8px;
}
.close-btn:hover { background: var(--color-surface-hi); }
.modal-body {
  padding: 0.5rem 1.5rem 1rem;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 1.1rem;
}
.group { display: flex; flex-direction: column; gap: 1.1rem; }
.group + .group { margin-top: 0.25rem; }
.group-title {
  margin: 0;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  color: var(--color-muted);
  padding-bottom: 0.4rem;
  border-bottom: 1px solid var(--color-border);
}
.field { display: flex; flex-direction: column; gap: 0.4rem; }
.label-row { display: flex; align-items: center; justify-content: space-between; gap: 0.5rem; }
label { font-weight: 600; color: var(--color-fg); }
.badge {
  font-size: 0.68rem;
  font-weight: 700;
  padding: 0.12rem 0.5rem;
  border-radius: 999px;
  background: var(--color-surface-hi);
  color: var(--color-muted);
  white-space: nowrap;
}
.badge.reloadable { background: var(--color-surface-mute); color: var(--color-ok); }
input[type='text'],
input[type='number'],
textarea,
.picker {
  width: 100%;
  box-sizing: border-box;
  padding: 0.6rem;
  border: 1px solid var(--color-border);
  border-radius: 8px;
  font-size: 16px;
  background: var(--color-surface);
}
.picker { cursor: pointer; }
.checklist {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
  max-height: 180px;
  overflow-y: auto;
  border: 1px solid var(--color-border);
  border-radius: 8px;
  padding: 0.5rem 0.6rem;
  background: var(--color-surface);
}
.check { display: flex; align-items: center; gap: 0.5rem; font-weight: 400; cursor: pointer; }
.check input { width: 18px; height: 18px; flex: none; }
.rewards { display: flex; flex-direction: column; gap: 0.5rem; }
.reward-row { display: flex; align-items: center; gap: 0.5rem; }
.reward-level { width: 5rem; flex: none; }
.reward-arrow { color: var(--color-muted); flex: none; }
.reward-role { flex: 1; min-width: 0; }
.reward-del {
  flex: none;
  background: transparent;
  border: 1px solid var(--color-border);
  border-radius: 8px;
  width: 2.2rem;
  height: 2.2rem;
  cursor: pointer;
  color: var(--color-muted);
}
.reward-del:hover { color: var(--color-accent); border-color: var(--color-accent); }
.reward-add {
  align-self: flex-start;
  background: var(--color-surface);
  border: 1px dashed var(--color-border);
  border-radius: 8px;
  padding: 0.45rem 0.9rem;
  cursor: pointer;
  color: var(--color-fg);
  font-weight: 500;
}
.reward-add:hover { border-color: var(--color-accent); color: var(--color-accent); }
textarea { resize: vertical; min-height: 70px; }
textarea.mono { font-family: ui-monospace, Menlo, monospace; font-size: 0.85rem; }
.switch { display: flex; align-items: center; gap: 0.5rem; font-weight: 400; }
.help { margin: 0; color: var(--color-muted); font-size: 0.8rem; }
.empty { color: var(--color-muted); font-style: italic; }
.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid var(--color-border);
}

@media (max-width: 768px) {
  .overlay { padding: 0; align-items: stretch; }
  .modal { max-width: 100%; height: 100dvh; max-height: 100dvh; border-radius: 0; }
  .modal-footer {
    position: sticky;
    bottom: 0;
    background: var(--color-surface);
    padding-bottom: calc(1rem + env(safe-area-inset-bottom));
  }
}
</style>
