<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { token } from '../lib/auth'
import { api } from '../lib/api'
import { type BotSetting, emptyBotSetting } from '../types/botSetting'
import BotList from '../components/BotList.vue'
import SettingsForm from '../components/SettingsForm.vue'

const bots = ref<BotSetting[]>([])
const selectedBotId = ref<string | null>(null)
const draft = ref<BotSetting>(emptyBotSetting())

const loading = ref(false)
const saving = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

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
}

function selectBot(botId: string): void {
  const found = bots.value.find((b) => b.botId === botId)
  if (!found) return
  selectedBotId.value = botId
  success.value = false
  error.value = null
  // Work on a copy; never leak the token back from a previous selection.
  draft.value = { ...found, token: '' }
}

async function save(): Promise<void> {
  if (!selectedBotId.value) return
  saving.value = true
  error.value = null
  success.value = false
  try {
    const updated = await api.updateBotSettings(draft.value.botId, draft.value)
    const index = bots.value.findIndex((b) => b.botId === updated.botId)
    if (index !== -1) bots.value[index] = updated
    draft.value = { ...updated, token: '' }
    success.value = true
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to save settings.'
  } finally {
    saving.value = false
  }
}

onMounted(loadBots)
</script>

<template>
  <div class="page">
    <h1>Bot Settings</h1>

    <p v-if="!token" class="notice">Please log in on the Home page to manage bot settings.</p>

    <template v-else>
      <p v-if="loading" class="notice">Loading…</p>
      <p v-else-if="bots.length === 0" class="notice">
        No bot settings found. A bot writes its row to the database on first run.
      </p>

      <div v-else class="layout">
        <BotList :bots="bots" :selected-bot-id="selectedBotId" @select="selectBot" />

        <div class="content">
          <div v-if="error" class="banner error">{{ error }}</div>
          <div v-if="success" class="banner success">Settings saved.</div>

          <template v-if="selectedBotId">
            <h2 class="content-title">{{ draft.botName || 'Bot' }}</h2>
            <SettingsForm :bot="draft" :saving="saving" @save="save" />
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

.page > h1 {
  margin-bottom: 1.5rem;
  font-size: 1.9rem;
  background: linear-gradient(90deg, var(--foxfire), var(--violet));
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
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
  .page > h1 {
    font-size: 1.5rem;
    margin-bottom: 1rem;
  }
  /* Stack the bot list above the editor instead of side-by-side. */
  .layout {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
}
</style>
