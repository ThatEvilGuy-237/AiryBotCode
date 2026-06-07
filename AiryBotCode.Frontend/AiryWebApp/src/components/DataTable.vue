<script setup lang="ts">
import type { Column } from '../lib/mockDatabase'

defineProps<{
  columns: Column[]
  rows: Record<string, string | number>[]
}>()
</script>

<template>
  <div class="table-wrap">
    <table>
      <thead>
        <tr>
          <th v-for="col in columns" :key="col.key">{{ col.label }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(row, index) in rows" :key="index">
          <td v-for="col in columns" :key="col.key">{{ row[col.key] }}</td>
        </tr>
        <tr v-if="rows.length === 0">
          <td :colspan="columns.length" class="empty">No rows.</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.table-wrap {
  background-color: var(--surface);
  border: 1px solid var(--border-color);
  border-radius: 14px;
  box-shadow: 0 4px 16px var(--shadow);
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  text-align: left;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--border-color);
}

td {
  color: var(--text-color);
  max-width: 360px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

th {
  background-color: var(--surface-2);
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  color: var(--violet);
}

tbody tr:hover td {
  background-color: var(--surface-hover);
}

tbody tr:last-child td {
  border-bottom: none;
}

.empty {
  color: var(--muted-color);
  text-align: center;
}
</style>
