<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { Alert, Button, Card, EmptyState, Field, Icon, Modal, PageHeader, Select, Skeleton, Table, TableColumn, TextField, useToast } from '@airy/ui'
import { api, ApiError, type TableData, type TableInfo } from '@/lib/api'

const props = defineProps<{ db?: string }>()
const router = useRouter()
const toast = useToast()

const databases = ref<string[] | null>(null)
const error = ref<string | null>(null)
const creating = ref(false)
const name = ref('')
const createBusy = ref(false)
const createError = ref<string | null>(null)

const schemas = ref<string[]>([])
const schema = ref('')
const tables = ref<TableInfo[] | null>(null)
const table = ref<string | null>(null)
const data = ref<TableData | null>(null)
const dataLoading = ref(false)
const dbError = ref<string | null>(null)

const validName = computed(() => /^[a-z][a-z0-9_]{1,62}$/.test(name.value.trim()))
const rows = computed(() => (data.value?.rows ?? []).map((r, i) => ({ __row: i, ...Object.fromEntries((data.value?.columns ?? []).map((c, j) => [c, cell(r[j])])) })))

function cell(v: unknown): string {
  if (v === null || v === undefined) return '·'
  if (typeof v === 'object') return JSON.stringify(v)
  const s = String(v)
  return s.length > 120 ? `${s.slice(0, 120)}…` : s
}

async function loadList() {
  error.value = null
  try {
    databases.value = (await api.databases()).sort()
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'The databases could not be loaded.'
  }
}

async function openDb(db: string) {
  dbError.value = null
  tables.value = null
  table.value = null
  data.value = null
  try {
    schemas.value = await api.schemas(db)
    schema.value = schemas.value.includes('public') ? 'public' : (schemas.value[0] ?? '')
    if (!schema.value) tables.value = []
  } catch (e) {
    dbError.value = e instanceof Error ? e.message : 'This database could not be read.'
  }
}

async function loadTables() {
  if (!props.db || !schema.value) return
  tables.value = null
  table.value = null
  data.value = null
  try {
    tables.value = (await api.tables(props.db, schema.value)).sort((a, b) => a.name.localeCompare(b.name))
  } catch (e) {
    dbError.value = e instanceof Error ? e.message : 'The tables could not be read.'
  }
}

async function pick(t: string) {
  if (!props.db) return
  table.value = t
  dataLoading.value = true
  data.value = null
  try {
    data.value = await api.tableData(props.db, schema.value, t)
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'The rows could not be read.')
  } finally {
    dataLoading.value = false
  }
}

async function create() {
  createBusy.value = true
  createError.value = null
  try {
    const made = await api.createDatabase(name.value.trim())
    creating.value = false
    name.value = ''
    toast.success(`Database ${made.name} made`)
    await loadList()
    router.push(`/databases/${made.name}`)
  } catch (e) {
    createError.value = e instanceof ApiError && e.status === 409 ? 'A database with that name exists.' : e instanceof Error ? e.message : 'Making it failed.'
  } finally {
    createBusy.value = false
  }
}

watch(() => props.db, db => {
  if (db) openDb(db)
  else loadList()
}, { immediate: true })
watch(schema, loadTables)
</script>

<template>
  <div class="page">
    <template v-if="!db">
      <PageHeader title="Databases" subtitle="// the shared Postgres; reading only, except making a new one">
        <template #actions>
          <Button variant="outline" @click="creating = true"><Icon name="plus" :size="16" />New database</Button>
        </template>
      </PageHeader>
      <Alert v-if="error" variant="danger" :title="error">
        <template #actions><Button size="sm" variant="outline" @click="loadList">Try again</Button></template>
      </Alert>
      <div v-else-if="databases === null" class="list"><Skeleton v-for="i in 5" :key="i" variant="block" height="48px" /></div>
      <EmptyState v-else-if="!databases.length" title="No databases" sub="Make one for a bot that keeps its own data." />
      <ul v-else class="list">
        <li v-for="d in databases" :key="d">
          <RouterLink :to="`/databases/${d}`" class="db"><Icon name="database" :size="16" /><span class="mono">{{ d }}</span><Icon name="chevron-right" :size="16" class="chev" /></RouterLink>
        </li>
      </ul>
    </template>

    <template v-else>
      <PageHeader :title="db" :breadcrumbs="[{ label: 'Databases', to: '/databases' }, { label: db }]" />
      <Alert v-if="dbError" variant="danger" :title="dbError" />
      <div v-else class="explore">
        <Card class="tables">
          <template #head>
            <div class="head">
              <h2>Tables</h2>
              <Field v-if="schemas.length > 1" label="Schema" hide-label class="schema">
                <Select v-model="schema" :options="schemas.map(s => ({ label: s, value: s }))" />
              </Field>
            </div>
          </template>
          <div v-if="tables === null" class="list"><Skeleton v-for="i in 6" :key="i" variant="text" /></div>
          <EmptyState v-else-if="!tables.length" title="No tables" :sub="schemas.length ? 'This schema is empty.' : 'This database has no schema yet.'" />
          <ul v-else class="tlist">
            <li v-for="t in tables" :key="t.name">
              <button type="button" class="t" :aria-pressed="table === t.name" @click="pick(t.name)">
                <span class="mono">{{ t.name }}</span>
                <span class="rows">{{ t.rowCount }}</span>
              </button>
            </li>
          </ul>
        </Card>
        <Card class="rowsCard">
          <template #head><h2>{{ table ?? 'Rows' }}</h2></template>
          <EmptyState v-if="!table" title="Pick a table" sub="The first 100 rows show here. Bot tokens are hidden." />
          <Table
            v-else
            :data="rows"
            row-key="__row"
            :label="`Rows of ${table}`"
            density="compact"
            scroll-x
            :loading="dataLoading"
            empty="No rows"
          >
            <TableColumn v-for="c in data?.columns ?? []" :key="c" :field="c" :header="c" />
          </Table>
        </Card>
      </div>
    </template>

    <Modal :open="creating" title="New database" size="sm" @close="creating = false">
      <form class="form" @submit.prevent="create">
        <Field label="Name" hint="Lowercase letters, digits and _" :error="name && !validName ? 'Start with a letter; lowercase, digits and _ only' : createError ?? undefined">
          <TextField v-model="name" autocomplete="off" />
        </Field>
      </form>
      <template #footer>
        <Button variant="ghost" @click="creating = false">Cancel</Button>
        <Button :loading="createBusy" :disabled="!validName" @click="create">Make it</Button>
      </template>
    </Modal>
  </div>
</template>

<style scoped>
.page {
  display: grid;
  gap: var(--space-5);
}
h2 {
  margin: 0;
  font-size: var(--font-size-lg);
  color: var(--color-fg);
}
.list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  gap: var(--space-2);
}
.db {
  display: grid;
  grid-template-columns: auto minmax(0, 1fr) auto;
  align-items: center;
  gap: var(--space-3);
  min-height: 48px;
  padding: 0 var(--space-4);
  border: var(--border);
  border-radius: var(--r-2);
  background: var(--color-surface);
  color: var(--color-fg);
  text-decoration: none;
}
.db:hover {
  background: var(--color-surface-hi);
}
.db:focus-visible {
  outline: 2px solid var(--color-accent);
  outline-offset: 2px;
}
.mono {
  font-family: var(--font-mono);
  font-size: var(--font-size-sm);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.chev {
  color: var(--color-muted);
}
.explore {
  display: grid;
  grid-template-columns: minmax(220px, 280px) minmax(0, 1fr);
  gap: var(--space-4);
  align-items: start;
}
.head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-2);
  width: 100%;
}
.schema {
  width: 140px;
}
.tlist {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  max-height: 60vh;
  overflow: auto;
}
.t {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-2);
  min-height: 36px;
  padding: 0 var(--space-2);
  border: 0;
  border-radius: var(--r-1);
  background: transparent;
  color: var(--color-fg);
  cursor: pointer;
  text-align: left;
}
.t:hover {
  background: var(--color-surface-hi);
}
.t[aria-pressed='true'] {
  background: var(--color-accent-soft);
  box-shadow: inset 2px 0 0 var(--color-accent);
}
.t:focus-visible {
  outline: 2px solid var(--color-accent);
  outline-offset: -2px;
}
.rows {
  font-family: var(--font-mono);
  font-size: var(--font-size-xs);
  color: var(--color-muted);
}
.form {
  display: grid;
  gap: var(--space-4);
}
@media (max-width: 900px) {
  .explore {
    grid-template-columns: minmax(0, 1fr);
  }
  .t {
    min-height: 44px;
  }
}
</style>
