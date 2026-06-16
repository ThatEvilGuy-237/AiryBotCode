<script setup lang="ts">
// Migrated to @hive/ui design tokens (Item E). No @hive/ui table primitive exists,
// so this stays a custom table but themes via the shared tokens (mono for data,
// borders-not-shadows, single accent) so it re-themes with the image theme.
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
  background-color: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius);
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
  padding: var(--space-2) var(--space-3);
  border-bottom: 1px solid var(--color-border);
}

td {
  color: var(--color-fg);
  font-family: var(--font-mono);
  font-size: var(--font-size-sm);
  max-width: 360px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

th {
  background-color: var(--color-bg);
  font-size: var(--font-size-xs);
  text-transform: uppercase;
  letter-spacing: 0.04em;
  color: var(--color-muted);
  font-family: var(--font-mono);
}

tbody tr:hover td {
  background-color: var(--color-bg);
}

tbody tr:last-child td {
  border-bottom: none;
}

.empty {
  color: var(--color-muted);
  text-align: center;
  font-family: inherit;
  font-style: italic;
}
</style>
