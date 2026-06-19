<script setup lang="ts">
// Migrated to @hive/ui (Item E). Logic unchanged.
import { ref, computed, watch, onMounted } from 'vue'
import { PageHeader, Card, Button, Badge, Toggle } from '@hive/ui'
import { api, ApiError, type CommandConfig } from '../lib/api'
import CommandModal from '../components/CommandModal.vue'
import { useBots } from '../lib/bots'

const { currentBot, currentBotId, loadBots } = useBots()

const commands = ref<CommandConfig[]>([])
const search = ref('')
const loading = ref(true)
const error = ref('')
const selected = ref<CommandConfig | null>(null)

const reloading = ref(false)
const reloadMsg = ref('')

const filtered = computed(() =>
  search.value
    ? commands.value.filter((c) => c.commandName.toLowerCase().includes(search.value.toLowerCase()))
    : commands.value,
)

function autoCount(c: CommandConfig) {
  return c.settings.filter((s) => s.isReloadable).length
}

async function load() {
  if (!currentBotId.value) {
    commands.value = []
    loading.value = false
    return
  }
  loading.value = true
  error.value = ''
  try {
    commands.value = await api.getCommands(currentBotId.value)
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to load commands.'
  } finally {
    loading.value = false
  }
}

async function toggle(c: CommandConfig) {
  if (!currentBotId.value) return
  const next = !c.enabled
  c.enabled = next // optimistic
  const ok = await api.setCommandEnabled(currentBotId.value, c.commandName, next)
  if (!ok) c.enabled = !next
}

async function onSave(settings: { key: string; value: string }[]) {
  if (!selected.value || !currentBotId.value) return
  const name = selected.value.commandName
  const ok = await api.saveCommand(currentBotId.value, name, settings)
  if (ok) {
    const cmd = commands.value.find((c) => c.commandName === name)
    if (cmd) for (const s of settings) {
      const target = cmd.settings.find((x) => x.key === s.key)
      if (target) target.value = s.value
    }
  }
  selected.value = null
}

async function reload() {
  reloading.value = true
  reloadMsg.value = ''
  const ok = await api.reloadBot()
  reloading.value = false
  reloadMsg.value = ok ? 'Restart requested — the fleet will reload shortly.' : 'Restart request failed.'
  setTimeout(() => (reloadMsg.value = ''), 5000)
}

watch(currentBotId, load)
onMounted(async () => {
  await loadBots()
  await load()
})
</script>

<template>
  <div class="page">
    <PageHeader
      title="Commands"
      :subtitle="currentBot
        ? `Toggle which commands ${currentBot.botName} runs; click a card to edit its settings. Changes apply on restart.`
        : 'Pick a bot from the menu to configure its commands.'"
    >
      <template #actions>
        <input v-model="search" type="search" placeholder="Search…" class="search" />
        <Button variant="outline" :disabled="reloading" @click="reload">{{ reloading ? 'Restarting…' : '⟳ Restart fleet' }}</Button>
      </template>
    </PageHeader>

    <p v-if="reloadMsg" class="banner ok">{{ reloadMsg }}</p>
    <p v-if="error" class="banner err">{{ error }}</p>

    <p v-if="!currentBotId" class="muted">No bot selected — pick one in the bot menu (top-left).</p>
    <p v-else-if="loading" class="muted">Loading commands…</p>
    <div v-else class="grid">
      <Card v-for="c in filtered" :key="c.commandName" class="cmd" :class="{ off: !c.enabled }" @click="selected = c">
        <template #head>
          <h3 class="name">{{ c.commandName }}</h3>
          <div style="flex: 1" />
          <span @click.stop><Toggle :modelValue="c.enabled" @update:modelValue="() => toggle(c)" /></span>
        </template>
        <div class="meta">
          <span class="mono">{{ c.settings.length }} setting{{ c.settings.length === 1 ? '' : 's' }}</span>
          <Badge v-if="autoCount(c)" variant="inactive">{{ autoCount(c) }} auto</Badge>
          <Badge :variant="c.enabled ? 'active' : 'inactive'">{{ c.enabled ? 'enabled' : 'disabled' }}</Badge>
        </div>
      </Card>
      <p v-if="!filtered.length" class="muted">No commands found.</p>
    </div>

    <CommandModal
      v-if="selected"
      :command="selected"
      :bot-id="currentBotId"
      @save="onSave"
      @close="selected = null"
    />
  </div>
</template>

<style scoped>
.page { padding: 2rem; display: flex; flex-direction: column; gap: 1rem; }
.muted { color: var(--color-muted); }
.banner { border-radius: var(--radius); padding: .7rem 1rem; font-weight: 500; border: var(--border); }
.banner.ok { color: var(--color-ok); background: var(--color-surface-mute); border-color: var(--color-accent-edge); }
.banner.err { color: var(--color-danger); background: var(--color-surface-mute); }
.search { border: var(--border); border-radius: var(--r-2); padding: var(--space-2) var(--space-3); font-size: 16px;
  background: var(--color-surface-mute); color: var(--color-fg); min-width: 160px; }
.search:focus { outline: none; border-color: var(--color-accent-edge); }
.grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(220px, 1fr)); gap: var(--space-3); }
.cmd { cursor: pointer; transition: border-color .15s, background .15s; }
.cmd:hover { border-color: var(--color-accent-edge); background: var(--color-surface-hi); }
.cmd.off { opacity: .6; }
.name { margin: 0; font-size: var(--font-size-base); overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.meta { display: flex; align-items: center; gap: var(--space-2); flex-wrap: wrap; }
.mono { font-family: var(--font-mono); color: var(--color-muted); font-size: var(--font-size-sm); }
@media (max-width: 768px) { .page { padding: 1rem; } .grid { grid-template-columns: 1fr 1fr; } }
</style>
