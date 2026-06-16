<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
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
    <header class="page-header">
      <h1>{{ creating ? 'New bot' : currentBot?.botName || 'Settings' }}</h1>
      <div v-if="token" class="header-actions">
        <button type="button" class="restart-btn" :disabled="reloading" @click="reloadFleet">
          {{ reloading ? 'Restarting…' : '⟳ Restart fleet' }}
        </button>
        <button
          v-if="!creating && currentBot"
          type="button"
          class="delete-btn"
          @click="removeBot"
        >
          Delete
        </button>
      </div>
    </header>

    <p v-if="reloadMsg" class="banner success">{{ reloadMsg }}</p>
    <p v-if="!token" class="notice">Please log in on the Home page to manage bot settings.</p>

    <template v-else>
      <div v-if="error" class="banner error">{{ error }}</div>
      <div v-if="success" class="banner success">{{ creating ? 'Bot added.' : 'Settings saved.' }}</div>

      <SettingsForm
        v-if="creating || currentBot"
        :bot="draft"
        :saving="saving"
        :databases="databases"
        :creating="creating"
        @save="save"
      />
      <p v-else class="notice">
        No bots yet — use <strong>New bot</strong> in the bot menu (top-left) to add one.
      </p>
    </template>
  </div>
</template>

<style scoped>
.page {
  padding: 2rem;
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.page-header h1 {
  margin: 0;
  font-size: 1.9rem;
  background: linear-gradient(90deg, var(--foxfire), var(--violet));
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
}

.header-actions {
  display: flex;
  gap: 0.6rem;
  flex-wrap: wrap;
}
.add-btn {
  background: linear-gradient(90deg, var(--foxfire), var(--foxfire-deep));
  color: #fff;
  border: none;
  border-radius: 999px;
  padding: 0.55rem 1.2rem;
  font-weight: 600;
  cursor: pointer;
  white-space: nowrap;
  box-shadow: 0 2px 8px var(--shadow);
  transition: filter 0.15s ease, transform 0.15s ease;
}
.add-btn:hover {
  filter: brightness(1.05);
  transform: translateY(-1px);
}
.restart-btn {
  background: var(--surface-2);
  color: var(--foxfire-deep);
  border: 1px solid var(--foxfire);
  border-radius: 999px;
  padding: 0.55rem 1.2rem;
  font-weight: 600;
  cursor: pointer;
  white-space: nowrap;
  transition: all 0.15s ease;
}
.restart-btn:hover:not(:disabled) { background: var(--surface-hover); }
.restart-btn:disabled { opacity: 0.55; cursor: default; }

.content-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1rem;
}

.delete-btn {
  background: transparent;
  border: 1px solid rgba(248, 113, 113, 0.5);
  color: var(--danger-color);
  border-radius: 8px;
  padding: 0.45rem 0.9rem;
  font-weight: 500;
  cursor: pointer;
  white-space: nowrap;
  transition: all 0.15s ease;
}
.delete-btn:hover {
  background: rgba(248, 113, 113, 0.12);
}

.notice {
  color: var(--muted-color);
}

.layout {
  display: grid;
  grid-template-columns: 280px 1fr;
  gap: 1.5rem;
  align-items: start;
}

.content {
  min-width: 0;
}

.content-title {
  margin-bottom: 1rem;
}

.banner {
  border-radius: 8px;
  padding: 0.75rem 1rem;
  margin-bottom: 1rem;
  font-weight: 500;
}

.banner.error {
  background-color: rgba(248, 113, 113, 0.12);
  border: 1px solid rgba(248, 113, 113, 0.4);
  color: var(--danger-color);
}

.banner.success {
  background-color: rgba(74, 222, 128, 0.12);
  border: 1px solid rgba(74, 222, 128, 0.4);
  color: var(--success-color);
}

@media (max-width: 768px) {
  .page {
    padding: 1rem;
  }
  .page-header {
    margin-bottom: 1rem;
  }
  .page-header h1 {
    font-size: 1.5rem;
  }
  /* Stack the bot list above the editor instead of side-by-side. */
  .layout {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
}
</style>
