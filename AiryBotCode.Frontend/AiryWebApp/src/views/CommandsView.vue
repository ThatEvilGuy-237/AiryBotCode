<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { api, ApiError, type CommandConfig } from '../lib/api'
import CommandModal from '../components/CommandModal.vue'

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
  loading.value = true
  error.value = ''
  try {
    commands.value = await api.getCommands()
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to load commands.'
  } finally {
    loading.value = false
  }
}

async function onSave(settings: { key: string; value: string }[]) {
  if (!selected.value) return
  const name = selected.value.commandName
  const ok = await api.saveCommand(name, settings)
  if (ok) {
    // reflect locally
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
  reloadMsg.value = ok ? 'Reload requested — the bot will restart shortly.' : 'Reload request failed.'
  setTimeout(() => (reloadMsg.value = ''), 5000)
}

onMounted(load)
</script>

<template>
  <div class="page">
    <header class="page-header">
      <div>
        <h1>Commands</h1>
        <p class="sub">Tap a command to edit its settings. Reloadable settings auto-apply; others need a bot reload.</p>
      </div>
      <div class="actions">
        <input v-model="search" type="search" placeholder="Search…" class="search" />
        <button class="reload-btn" :disabled="reloading" @click="reload">
          {{ reloading ? 'Reloading…' : 'Reload bot' }}
        </button>
      </div>
    </header>

    <p v-if="reloadMsg" class="reload-msg">{{ reloadMsg }}</p>
    <p v-if="error" class="error">{{ error }}</p>

    <p v-if="loading" class="muted">Loading commands…</p>
    <div v-else class="grid">
      <button
        v-for="c in filtered"
        :key="c.commandName"
        type="button"
        class="card"
        @click="selected = c"
      >
        <h3>{{ c.commandName }}</h3>
        <div class="meta">
          <span>{{ c.settings.length }} setting{{ c.settings.length === 1 ? '' : 's' }}</span>
          <span v-if="autoCount(c)" class="pill">{{ autoCount(c) }} auto</span>
        </div>
      </button>
      <p v-if="!filtered.length" class="muted">No commands found.</p>
    </div>

    <CommandModal
      v-if="selected"
      :command="selected"
      @save="onSave"
      @close="selected = null"
    />
  </div>
</template>

<style scoped>
.page { padding: 2rem; }
.page-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 1.5rem;
}
h1 {
  margin: 0;
  font-size: 1.9rem;
  background: linear-gradient(90deg, var(--foxfire), var(--violet));
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
}
.sub { margin: 0.35rem 0 0; color: var(--muted-color); max-width: 520px; }
.actions { display: flex; gap: 0.6rem; flex-wrap: wrap; }
.search {
  border: 1px solid var(--input-border, #ecc0cf);
  border-radius: 999px;
  padding: 0.55rem 1rem;
  font-size: 16px;
  background: #fff;
  min-width: 160px;
}
.reload-btn {
  background: var(--foxfire);
  color: #fff;
  border: none;
  border-radius: 999px;
  padding: 0.55rem 1.2rem;
  font-weight: 700;
  cursor: pointer;
  white-space: nowrap;
}
.reload-btn:hover:not(:disabled) { background: var(--foxfire-deep); }
.reload-btn:disabled { opacity: 0.55; cursor: default; }

.reload-msg {
  background: #e7f6ec;
  border: 1px solid #bce4cb;
  color: #1f9254;
  padding: 0.7rem 1rem;
  border-radius: 10px;
  margin-bottom: 1rem;
}
.error {
  background: #fde8ef;
  border: 1px solid var(--border-color);
  color: var(--foxfire-deep);
  padding: 0.7rem 1rem;
  border-radius: 10px;
  margin-bottom: 1rem;
}
.muted { color: var(--muted-color); }

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 1rem;
}
.card {
  height: 120px;
  text-align: left;
  background: var(--surface);
  border: 1px solid var(--border-color);
  border-radius: 14px;
  padding: 1.1rem 1.25rem;
  cursor: pointer;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  transition: all 0.15s ease;
  font: inherit;
}
.card:hover {
  transform: translateY(-2px);
  border-color: var(--foxfire);
  box-shadow: 0 8px 22px rgba(232, 70, 122, 0.15);
}
.card h3 {
  margin: 0;
  font-size: 1.05rem;
  color: var(--text-color);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.meta { display: flex; align-items: center; gap: 0.5rem; color: var(--muted-color); font-size: 0.82rem; }
.pill {
  background: #e7f6ec;
  color: #1f9254;
  border-radius: 999px;
  padding: 0.1rem 0.5rem;
  font-weight: 700;
  font-size: 0.72rem;
}

@media (max-width: 768px) {
  .page { padding: 1rem; }
  h1 { font-size: 1.5rem; }
  .actions { width: 100%; }
  .search { flex: 1; min-width: 0; }
  .grid { grid-template-columns: 1fr 1fr; }
}
</style>
