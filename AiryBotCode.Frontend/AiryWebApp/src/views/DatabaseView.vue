<script setup lang="ts">
import { ref, computed } from 'vue'
import { tables } from '../lib/mockDatabase'
import DataTable from '../components/DataTable.vue'

const selectedId = ref<string>(tables[0]?.id ?? '')
const selected = computed(() => tables.find((t) => t.id === selectedId.value) ?? null)
</script>

<template>
  <div class="page">
    <h1>Database</h1>
    <p class="notice">Preview only — not yet connected to a backend data API.</p>

    <div class="layout">
      <aside class="table-list">
        <button
          v-for="table in tables"
          :key="table.id"
          type="button"
          class="table-item"
          :class="{ active: table.id === selectedId }"
          @click="selectedId = table.id"
        >
          {{ table.name }}
        </button>
      </aside>

      <div class="content">
        <template v-if="selected">
          <h2 class="content-title">{{ selected.name }}</h2>
          <DataTable :columns="selected.columns" :rows="selected.rows" />
        </template>
        <p v-else class="notice">Select a table.</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page {
  padding: 2rem;
}

.page > h1 {
  margin-bottom: 0.5rem;
}

.notice {
  color: var(--muted-color);
  margin-bottom: 1.5rem;
}

.layout {
  display: grid;
  grid-template-columns: 240px 1fr;
  gap: 1.5rem;
  align-items: start;
}

.table-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.table-item {
  text-align: left;
  border: 1px solid var(--border-color);
  background: #fff;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.15s ease;
}

.table-item:hover {
  border-color: var(--primary-color);
}

.table-item.active {
  border-color: var(--primary-color);
  background-color: #eef5fd;
}

.content-title {
  margin-bottom: 1rem;
}
</style>
