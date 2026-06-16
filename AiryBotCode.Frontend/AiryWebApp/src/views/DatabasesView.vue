<script setup lang="ts">
// Migrated to @hive/ui (Item E) — re-themes with the image-derived theme.
// Functionality unchanged.
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { PageHeader, Card, Button, TextField } from '@hive/ui'
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
    <PageHeader title="Databases" subtitle="Browse and create Postgres databases on the bot's server.">
      <template #actions>
        <Button variant="solid" @click="adding = !adding">+ Add database</Button>
      </template>
    </PageHeader>

    <Card v-if="adding" class="add-card">
      <div class="add-row">
        <TextField v-model="newName" placeholder="new_database_name" @keydown.enter.prevent="create" />
        <Button variant="solid" :disabled="creating || !newName.trim()" @click="create">
          {{ creating ? 'Creating…' : 'Create' }}
        </Button>
        <Button variant="ghost" @click="adding = false">Cancel</Button>
      </div>
      <p v-if="createError" class="error">{{ createError }}</p>
    </Card>

    <p v-if="error" class="error">{{ error }}</p>
    <p v-if="loading" class="muted">Loading…</p>

    <div v-else class="grid">
      <Card v-for="db in databases" :key="db" class="db-card" @click="open(db)">
        <span class="db-icon" aria-hidden="true">🗄️</span>
        <span class="db-name">{{ db }}</span>
        <span class="db-go" aria-hidden="true">›</span>
      </Card>
      <p v-if="!databases.length" class="muted">No databases.</p>
    </div>
  </div>
</template>

<style scoped>
.page { padding: var(--space-5); display: flex; flex-direction: column; gap: var(--space-4); }

.add-card { margin: 0; }
.add-row { display: flex; gap: var(--space-2); align-items: center; flex-wrap: wrap; }
.add-row :deep(.tf), .add-row > :first-child { flex: 1 1 14rem; min-width: 0; }

.error {
  color: var(--color-danger);
  font-size: var(--font-size-sm);
  margin: var(--space-2) 0 0;
}
.muted { color: var(--color-muted); }

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: var(--space-3);
}
.db-card {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  cursor: pointer;
  transition: border-color 0.15s ease;
}
.db-card:hover { border-color: var(--color-accent); }
.db-icon { font-size: 1.4rem; }
.db-name {
  font-weight: 600;
  font-family: var(--font-mono);
  color: var(--color-fg);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  flex: 1;
}
.db-go { color: var(--color-muted); font-size: 1.4rem; }

@media (max-width: 768px) {
  .page { padding: var(--space-3); }
  .grid { grid-template-columns: 1fr; }
}
</style>
