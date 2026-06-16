<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { PageHeader, Button } from '@hive/ui'
import { token } from '../lib/auth'
import { api } from '../lib/api'
import { type BotSetting, emptyBotSetting } from '../types/botSetting'
import SettingsForm from '../components/SettingsForm.vue'
import { useBots } from '../lib/bots'

// The bot being edited is the one selected in the sidebar switcher.
const {
  currentBot,
  currentBotId,
  createRequested,
  loadBots,
  applyUpsert,
  applyReplace,
  applyRemove,
} = useBots()

const databases = ref<string[]>([])
const draft = ref<BotSetting>(emptyBotSetting())
const creating = ref(false)

const saving = ref(false)
const error = ref<string | null>(null)
const success = ref(false)
const reloading = ref(false)
const reloadMsg = ref('')

function syncDraft(): void {
  // Work on a copy; never carry a token back into the editable form.
  draft.value = currentBot.value ? { ...currentBot.value, token: '' } : emptyBotSetting()
}

function startCreate(): void {
  creating.value = true
  error.value = null
  success.value = false
  draft.value = emptyBotSetting()
}

onMounted(async () => {
  await loadBots()
  try {
    databases.value = await api.getDatabases()
  } catch {
    databases.value = []
  }
  if (createRequested.value) {
    createRequested.value = false
    startCreate()
  } else {
    syncDraft()
  }
})

// "New bot" pressed in the switcher while this page is already open.
watch(createRequested, (req) => {
  if (req) {
    createRequested.value = false
    startCreate()
  }
})

// Selecting a different bot in the switcher reloads the editor.
watch(currentBotId, () => {
  if (!creating.value) {
    success.value = false
    error.value = null
    syncDraft()
  }
})

async function reloadFleet(): Promise<void> {
  reloading.value = true
  reloadMsg.value = ''
  const ok = await api.reloadBot()
  reloading.value = false
  reloadMsg.value = ok
    ? 'Restart requested — the fleet will reload shortly (added/enabled bots come online).'
    : 'Restart request failed.'
  setTimeout(() => (reloadMsg.value = ''), 6000)
}

async function save(): Promise<void> {
  saving.value = true
  error.value = null
  success.value = false
  try {
    if (creating.value) {
      const created = await api.createBot(draft.value)
      creating.value = false
      applyUpsert(created)
    } else {
      const oldId = currentBotId.value
      if (!oldId) return
      // Route by the ORIGINAL id; the body may carry a new id (re-key).
      const updated = await api.updateBotSettings(oldId, draft.value)
      applyReplace(oldId, updated)
    }
    syncDraft()
    success.value = true
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to save settings.'
  } finally {
    saving.value = false
  }
}

async function removeBot(): Promise<void> {
  const id = currentBotId.value
  if (!id) return
  if (!window.confirm('Remove this bot from the roster? This cannot be undone.')) return
  error.value = null
  try {
    await api.deleteBot(id)
    creating.value = false
    applyRemove(id)
    syncDraft()
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to delete bot.'
  }
}
</script>

<template>
  <div class="page">
    <PageHeader :title="creating ? 'New bot' : currentBot?.botName || 'Settings'">
      <template #actions>
        <template v-if="token">
          <Button variant="outline" :disabled="reloading" @click="reloadFleet">{{ reloading ? 'Restarting…' : '⟳ Restart fleet' }}</Button>
          <Button v-if="!creating && currentBot" variant="ghost" @click="removeBot">Delete</Button>
        </template>
      </template>
    </PageHeader>

    <p v-if="reloadMsg" class="banner ok">{{ reloadMsg }}</p>
    <p v-if="!token" class="notice">Please log in on the Home page to manage bot settings.</p>

    <template v-else>
      <p v-if="error" class="banner err">{{ error }}</p>
      <p v-if="success" class="banner ok">{{ creating ? 'Bot added.' : 'Settings saved.' }}</p>

      <SettingsForm
        v-if="creating || currentBot"
        :bot="draft"
        :saving="saving"
        :databases="databases"
        :creating="creating"
        @save="save"
      />
      <p v-else class="notice">No bots yet — use <strong>New bot</strong> in the bot menu (top-left) to add one.</p>
    </template>
  </div>
</template>

<style scoped>
.page { padding: 2rem; display: flex; flex-direction: column; gap: 1rem; max-width: 760px; }
.notice { color: var(--color-muted); }
.banner { border-radius: var(--radius); padding: .7rem 1rem; font-weight: 500; border: var(--border); }
.banner.ok { color: var(--color-ok); background: var(--color-surface-mute); border-color: var(--color-accent-edge); }
.banner.err { color: var(--color-danger); background: var(--color-surface-mute); }
@media (max-width: 768px) { .page { padding: 1rem; } }
</style>
