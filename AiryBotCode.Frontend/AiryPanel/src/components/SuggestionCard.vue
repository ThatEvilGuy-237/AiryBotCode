<script setup lang="ts">
import { computed, ref } from 'vue'
import { Badge } from '@airy/ui'
import type { Suggestion } from '@/lib/api'
import { ago } from '@/lib/time'

const props = defineProps<{ suggestion: Suggestion }>()
const open = ref(false)
const long = computed(() => props.suggestion.body.length > 280 || props.suggestion.body.split('\n').length > 4)
const state = computed(() => ({
  new: { label: 'Waiting', tone: 'accent' },
  reviewed: { label: 'Reviewed', tone: 'accent' },
  approved: { label: 'Approved', tone: 'ok' },
  rejected: { label: 'Not now', tone: 'muted' },
}[props.suggestion.status] ?? { label: props.suggestion.status, tone: 'neutral' }) as { label: string; tone: 'accent' | 'ok' | 'muted' | 'neutral' })
</script>

<template>
  <article class="card">
    <header class="top">
      <h3>{{ suggestion.title }}</h3>
      <Badge :tone="state.tone">{{ state.label }}</Badge>
    </header>
    <p class="body" :class="{ clamp: long && !open }">{{ suggestion.body }}</p>
    <button v-if="long" type="button" class="more" :aria-expanded="open" @click="open = !open">{{ open ? 'Less' : 'More' }}</button>
    <div v-if="suggestion.responseWhy || suggestion.responseEstimate" class="review">
      <span class="who">Airy's read</span>
      <p v-if="suggestion.responseWhy">{{ suggestion.responseWhy }}</p>
      <p v-if="suggestion.responseEstimate" class="estimate">Effort: {{ suggestion.responseEstimate }}</p>
    </div>
    <footer class="foot">
      <span class="meta">{{ suggestion.submittedBy || 'anonymous' }} · {{ ago(suggestion.createdAt) }}</span>
      <span class="actions"><slot /></span>
    </footer>
  </article>
</template>

<style scoped>
.card {
  display: grid;
  gap: var(--space-2);
  padding: var(--space-4);
  border: var(--border);
  border-radius: var(--r-2);
  background: var(--color-surface);
}
.card > * {
  min-width: 0;
}
.top {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--space-3);
}
h3 {
  flex: 1;
  min-width: 0;
  margin: 0;
  font-size: var(--font-size-base);
  color: var(--color-fg);
  overflow-wrap: anywhere;
}
.body {
  margin: 0;
  color: var(--color-text-dim);
  white-space: pre-line;
  overflow-wrap: anywhere;
}
.clamp {
  display: -webkit-box;
  -webkit-line-clamp: 4;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.more {
  justify-self: start;
  min-height: 32px;
  padding: 0;
  border: 0;
  background: none;
  color: var(--color-accent);
  font: inherit;
  font-size: var(--font-size-sm);
  cursor: pointer;
}
.review {
  display: grid;
  gap: var(--space-1);
  padding: var(--space-2) var(--space-3);
  border-radius: var(--r-1);
  background: var(--color-surface-mute);
  box-shadow: inset 2px 0 0 var(--color-accent);
}
.review > * {
  min-width: 0;
  overflow-wrap: anywhere;
}
.review p {
  margin: 0;
  color: var(--color-text-dim);
  font-size: var(--font-size-sm);
}
.who {
  font-size: var(--font-size-xs);
  color: var(--color-muted);
}
.foot {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-2);
}
.meta {
  color: var(--color-muted);
  font-size: var(--font-size-sm);
}
.actions {
  display: flex;
  flex-wrap: wrap;
  gap: var(--space-2);
}
@media (max-width: 860px) {
  .more {
    min-height: 44px;
  }
}
</style>
