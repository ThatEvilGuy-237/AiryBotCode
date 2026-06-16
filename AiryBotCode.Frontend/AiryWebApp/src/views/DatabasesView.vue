<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api, ApiError } from '../lib/api'

const router = useRouter()
const databases = ref<string[]>([])
const loading = ref(true)
const error = ref('')

const adding = ref(false)
const newName = ref('')
const creating = ref(false)
const createError = ref('')

async function load() {
  loading.value = true
  error.value = ''
  try {
    databases.value = await api.getDatabases()
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to load databases.'
  } finally {
    loading.value = false
  }
}

function open(db: string) {
  router.push({ name: 'database', params: { db } })
}

async function create() {
  if (creating.value || !newName.value.trim()) return
  creating.value = true
  createError.value = ''
  try {
    await api.createDatabase(newName.value.trim())
    const name = newName.value.trim()
    newName.value = ''
    adding.value = false
    await load()
    open(name)
  } catch (e) {
    createError.value = e instanceof ApiError ? e.message : 'Could not create the database.'
  } finally {
    creating.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="page">
    <header class="page-header">
      <h1>Databases</h1>
      <button type="button" class="add-btn" @click="adding = !adding">+ Add database</button>
    </header>

    <div v-if="adding" class="add-row">
      <input
        v-model="newName"
        type="text"
        placeholder="new_database_name"
        autocomplete="off"
        @keydown.enter.prevent="create"
      />
      <button type="button" class="create-btn" :disabled="creating || !newName.trim()" @click="create">
        {{ creating ? 'Creating…' : 'Create' }}
      </button>
      <button type="button" class="cancel-btn" @click="adding = false">Cancel</button>
    </div>
    <p v-if="createError" class="error">{{ createError }}</p>

    <p v-if="error" class="error">{{ error }}</p>
    <p v-if="loading" class="muted">Loading…</p>

    <div v-else class="grid">
      <button v-for="db in databases" :key="db" type="button" class="db-card" @click="open(db)">
        <span class="db-icon" aria-hidden="true">🗄️</span>
        <span class="db-name">{{ db }}</span>
        <span class="db-go" aria-hidden="true">›</span>
      </button>
      <p v-if="!databases.length" class="muted">No databases.</p>
    </div>
  </div>
</template>

<style scoped>
.page { padding: 2rem; }
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
.add-btn {
  background: linear-gradient(90deg, var(--foxfire), var(--foxfire-deep));
  color: #fff;
  border: none;
  border-radius: 999px;
  padding: 0.55rem 1.2rem;
  font-weight: 600;
  cursor: pointer;
  white-space: nowrap;
}
.add-btn:hover { filter: brightness(1.05); }

.add-row {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  flex-wrap: wrap;
  margin-bottom: 1rem;
}
.add-row input {
  flex: 1 1 14rem;
  min-width: 0;
  padding: 0.6rem 0.8rem;
  border: 1px solid var(--input-border, #ecc0cf);
  border-radius: 8px;
  font-size: 16px;
  background: var(--surface-2);
}
.create-btn {
  background: var(--foxfire);
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 0.6rem 1.1rem;
  font-weight: 600;
  cursor: pointer;
}
.create-btn:disabled { opacity: 0.55; cursor: default; }
.cancel-btn {
  background: var(--surface-2);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 0.6rem 1rem;
  cursor: pointer;
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
.db-card {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-align: left;
  background: var(--surface);
  border: 1px solid var(--border-color);
  border-radius: 14px;
  padding: 1.1rem 1.25rem;
  cursor: pointer;
  font: inherit;
  transition: all 0.15s ease;
}
.db-card:hover {
  transform: translateY(-2px);
  border-color: var(--foxfire);
  box-shadow: 0 8px 22px rgba(232, 70, 122, 0.15);
}
.db-icon { font-size: 1.4rem; }
.db-name {
  font-weight: 600;
  color: var(--text-color);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  flex: 1;
}
.db-go { color: var(--muted-color); font-size: 1.4rem; }

@media (max-width: 768px) {
  .page { padding: 1rem; }
  .page-header h1 { font-size: 1.5rem; }
  .grid { grid-template-columns: 1fr; }
}
</style>
