<script setup lang="ts">
// Migrated to @hive/ui (Item E) — schema/table selectors are FilterPills; re-themes
// with the image-derived theme. Functionality unchanged.
import { ref, computed, onMounted } from 'vue'
import { PageHeader, FilterPills, Card } from '@hive/ui'
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

const schemaItems = computed(() => schemas.value.map((s) => ({ value: s, label: s })))
const tableItems = computed(() =>
  tables.value.map((t) => ({ value: t.name, label: t.name, count: t.rowCount })))

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
    <PageHeader :title="db" subtitle="Live read-only explorer — schemas, tables and the first 100 rows." />

    <p v-if="error" class="error">{{ error }}</p>

    <div class="selectors">
      <div class="sel">
        <span class="label">Schemas</span>
        <FilterPills v-if="schemaItems.length" :model-value="selectedSchema" :items="schemaItems"
          @update:model-value="selectSchema" />
        <span v-else-if="!error" class="muted">Loading…</span>
      </div>

      <div v-if="selectedSchema" class="sel">
        <span class="label">Tables</span>
        <FilterPills v-if="tableItems.length" :model-value="selectedTable" :items="tableItems"
          @update:model-value="selectTable" />
        <span v-else class="muted">No tables in this schema.</span>
      </div>
    </div>

    <Card class="content">
      <template v-if="selectedTable">
        <h2 class="content-title">{{ selectedSchema }}.{{ selectedTable }}</h2>
        <p v-if="loading" class="muted">Loading rows…</p>
        <DataTable v-else :columns="columns" :rows="rows" />
        <p v-if="!loading && rows.length >= 100" class="muted note">Showing first 100 rows.</p>
      </template>
      <p v-else class="muted">Select a table.</p>
    </Card>
  </div>
</template>

<style scoped>
.page { padding: var(--space-5); display: flex; flex-direction: column; gap: var(--space-4); }
.back-link {
  display: inline-block;
  color: var(--color-muted);
  text-decoration: none;
  font-weight: 600;
  font-size: var(--font-size-sm);
}
.back-link:hover { color: var(--color-accent); }

.error { color: var(--color-danger); font-size: var(--font-size-sm); margin: 0; }
.muted { color: var(--color-muted); }
.note { font-size: var(--font-size-xs); margin-top: var(--space-2); }

.selectors { display: flex; flex-direction: column; gap: var(--space-3); }
.sel { display: flex; flex-direction: column; gap: var(--space-2); }
.label {
  font-size: var(--font-size-xs);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--color-muted);
  font-family: var(--font-mono);
}

.content { min-width: 0; }
.content-title { margin: 0 0 var(--space-3); font-family: var(--font-mono); font-size: var(--font-size-md); }

@media (max-width: 768px) {
  .page { padding: var(--space-3); }
}
</style>
