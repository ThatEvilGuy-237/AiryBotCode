<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { api, ApiError } from '../lib/api'
import DataTable from '../components/DataTable.vue'
import type { Column } from '../lib/mockDatabase'

// The database to explore comes from the route (/database/:db).
const props = defineProps<{ db: string }>()

const schemas = ref<string[]>([])
const tables = ref<{ name: string; rowCount: number }[]>([])
const selectedSchema = ref('')
const selectedTable = ref('')
const columns = ref<Column[]>([])
const rows = ref<Record<string, string | number>[]>([])

const loading = ref(false)
const error = ref('')

async function loadSchemas() {
  error.value = ''
  try {
    schemas.value = await api.getDbSchemas(props.db)
    if (schemas.value.length) {
      await selectSchema(schemas.value.includes('public') ? 'public' : schemas.value[0])
    }
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to load schemas.'
  }
}

async function selectSchema(schema: string) {
  selectedSchema.value = schema
  selectedTable.value = ''
  columns.value = []
  rows.value = []
  error.value = ''
  try {
    tables.value = await api.getDbTables(schema, props.db)
    if (tables.value.length) await selectTable(tables.value[0].name)
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to load tables.'
  }
}

async function selectTable(table: string) {
  selectedTable.value = table
  loading.value = true
  error.value = ''
  try {
    const data = await api.getDbData(selectedSchema.value, table, 100, props.db)
    columns.value = data.columns.map((c) => ({ key: c, label: c }))
    rows.value = data.rows.map((r) => {
      const out: Record<string, string | number> = {}
      for (const k of Object.keys(r)) {
        const v = r[k]
        out[k] = v === null || v === undefined ? '' : typeof v === 'number' ? v : String(v)
      }
      return out
    })
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to load table data.'
  } finally {
    loading.value = false
  }
}

onMounted(loadSchemas)
</script>

<template>
  <div class="page">
    <router-link :to="{ name: 'databases' }" class="back-link">‹ Databases</router-link>
    <h1>{{ db }}</h1>

    <p v-if="error" class="error">{{ error }}</p>

    <!-- Schemas -->
    <div class="section-label">Schemas</div>
    <div class="chips">
      <button
        v-for="s in schemas"
        :key="s"
        type="button"
        class="chip"
        :class="{ active: s === selectedSchema }"
        @click="selectSchema(s)"
      >
        {{ s }}
      </button>
      <span v-if="!schemas.length && !error" class="muted">Loading…</span>
    </div>

    <div class="layout">
      <!-- Tables in the selected schema -->
      <aside class="table-list">
        <div class="section-label">Tables</div>
        <button
          v-for="t in tables"
          :key="t.name"
          type="button"
          class="table-item"
          :class="{ active: t.name === selectedTable }"
          @click="selectTable(t.name)"
        >
          <span class="table-name">{{ t.name }}</span>
          <span class="row-count">{{ t.rowCount }}</span>
        </button>
        <p v-if="selectedSchema && !tables.length" class="muted">No tables in this schema.</p>
      </aside>

      <!-- Rows -->
      <div class="content">
        <template v-if="selectedTable">
          <h2 class="content-title">{{ selectedSchema }}.{{ selectedTable }}</h2>
          <p v-if="loading" class="muted">Loading rows…</p>
          <DataTable v-else :columns="columns" :rows="rows" />
          <p v-if="!loading && rows.length >= 100" class="muted note">Showing first 100 rows.</p>
        </template>
        <p v-else class="muted">Select a table.</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page { padding: 2rem; }
.back-link {
  display: inline-block;
  margin-bottom: 0.6rem;
  color: var(--muted-color);
  text-decoration: none;
  font-weight: 600;
  font-size: 0.85rem;
}
.back-link:hover { color: var(--foxfire); }
.page > h1 {
  margin-bottom: 1rem;
  font-size: 1.9rem;
  background: linear-gradient(90deg, var(--foxfire), var(--violet));
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
}
.section-label {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--muted-color);
  margin-bottom: 0.5rem;
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
.note { font-size: 0.8rem; margin-top: 0.5rem; }

.chips { display: flex; flex-wrap: wrap; gap: 0.5rem; margin-bottom: 1.5rem; }
.chip {
  border: 1px solid var(--border-color);
  background: var(--surface);
  color: var(--text-color);
  border-radius: 999px;
  padding: 0.4rem 0.9rem;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.15s ease;
}
.chip:hover { border-color: var(--violet); background: var(--surface-hover); }
.chip.active { border-color: var(--foxfire); background: var(--foxfire); color: #fff; }

.layout {
  display: grid;
  grid-template-columns: 240px 1fr;
  gap: 1.5rem;
  align-items: start;
}
.table-list { display: flex; flex-direction: column; gap: 0.5rem; }
.table-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  text-align: left;
  color: var(--text-color);
  border: 1px solid var(--border-color);
  background: var(--surface);
  border-radius: 10px;
  padding: 0.7rem 0.9rem;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.15s ease;
}
.table-item:hover { border-color: var(--violet); background: var(--surface-hover); }
.table-item.active { border-color: var(--foxfire); background: var(--surface-2); }
.table-name { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.row-count { color: var(--muted-color); font-size: 0.75rem; flex-shrink: 0; }
.content-title { margin-bottom: 1rem; }
.content { min-width: 0; }

@media (max-width: 768px) {
  .page { padding: 1rem; }
  .layout { grid-template-columns: 1fr; gap: 1rem; }
  .table-list {
    flex-direction: row;
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
    padding-bottom: 0.25rem;
  }
  .table-item { flex-shrink: 0; }
}
</style>
