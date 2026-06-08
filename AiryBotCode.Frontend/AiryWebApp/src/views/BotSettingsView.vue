<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { token } from '../lib/auth'
import { api } from '../lib/api'
import { type BotSetting, emptyBotSetting } from '../types/botSetting'
import BotList from '../components/BotList.vue'
import SettingsForm from '../components/SettingsForm.vue'

const bots = ref<BotSetting[]>([])
const databases = ref<string[]>([])
const selectedBotId = ref<string | null>(null)
const draft = ref<BotSetting>(emptyBotSetting())
const creating = ref(false)

const loading = ref(false)
const saving = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

const reloading = ref(false)
const reloadMsg = ref('')

async function loadBots(): Promise<void> {
  if (!token.value) return
  loading.value = true
  error.value = null
  try {
    bots.value = await api.getBotSettings()
    if (bots.value.length > 0) {
      selectBot(bots.value[0].botId)
    }
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to load settings.'
  } finally {
    loading.value = false
  }
  // Databases for the per-bot picker (best-effort; non-fatal).
  try {
    databases.value = await api.getDatabases()
  } catch {
    databases.value = []
  }
}

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

function selectBot(botId: string): void {
  const found = bots.value.find((b) => b.botId === botId)
  if (!found) return
  creating.value = false
  selectedBotId.value = botId
  success.value = false
  error.value = null
  // Work on a copy; never leak the token back from a previous selection.
  draft.value = { ...found, token: '' }
}

// Start a blank "add bot" draft.
function addBot(): void {
  creating.value = true
  selectedBotId.value = null
  success.value = false
  error.value = null
  draft.value = emptyBotSetting()
}

async function save(): Promise<void> {
  saving.value = true
  error.value = null
  success.value = false
  try {
    if (creating.value) {
      const created = await api.createBot(draft.value)
      bots.value.push(created)
      selectBot(created.botId)
    } else {
      if (!selectedBotId.value) return
      // Route by the ORIGINAL id; the body may carry a new id (re-key).
      const updated = await api.updateBotSettings(selectedBotId.value, draft.value)
      const index = bots.value.findIndex((b) => b.botId === selectedBotId.value)
      if (index !== -1) bots.value[index] = updated
      selectedBotId.value = updated.botId
      draft.value = { ...updated, token: '' }
    }
    success.value = true
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to save settings.'
  } finally {
    saving.value = false
  }
}

async function removeBot(botId: string): Promise<void> {
  if (!window.confirm('Remove this bot from the roster? This cannot be undone.')) return
  error.value = null
  try {
    await api.deleteBot(botId)
    bots.value = bots.value.filter((b) => b.botId !== botId)
    creating.value = false
    if (bots.value.length > 0) {
      selectBot(bots.value[0].botId)
    } else {
      selectedBotId.value = null
      draft.value = emptyBotSetting()
    }
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to delete bot.'
  }
}

onMounted(loadBots)
</script>

<template>
  <div class="page">
    <header class="page-header">
      <h1>Bot Settings</h1>
      <div v-if="token" class="header-actions">
        <button type="button" class="restart-btn" :disabled="reloading" @click="reloadFleet">
          {{ reloading ? 'Restarting…' : '⟳ Restart fleet' }}
        </button>
        <button type="button" class="add-btn" @click="addBot">+ Add bot</button>
      </div>
    </header>

    <p v-if="reloadMsg" class="banner success">{{ reloadMsg }}</p>
    <p v-if="!token" class="notice">Please log in on the Home page to manage bot settings.</p>

    <template v-else>
      <p v-if="loading" class="notice">Loading…</p>
      <p v-else-if="bots.length === 0 && !creating" class="notice">
        No bots yet. Use <strong>+ Add bot</strong> to register one.
      </p>

      <div v-else class="layout">
        <BotList :bots="bots" :selected-bot-id="selectedBotId" @select="selectBot" />

        <div class="content">
          <div v-if="error" class="banner error">{{ error }}</div>
          <div v-if="success" class="banner success">{{ creating ? 'Bot added.' : 'Settings saved.' }}</div>

          <template v-if="creating">
            <h2 class="content-title">New bot</h2>
            <SettingsForm :bot="draft" :saving="saving" :databases="databases" creating @save="save" />
          </template>

          <template v-else-if="selectedBotId">
            <div class="content-head">
              <h2 class="content-title">{{ draft.botName || 'Bot' }}</h2>
              <button type="button" class="delete-btn" @click="removeBot(selectedBotId)">Delete</button>
            </div>
            <SettingsForm :bot="draft" :saving="saving" :databases="databases" @save="save" />
          </template>

          <p v-else class="notice">Select a bot to edit its settings.</p>
        </div>
      </div>
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
  background: #fff;
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
