<script setup lang="ts">
import type { BotSetting } from '../types/botSetting'

defineProps<{
  bots: BotSetting[]
  selectedBotId: string | null
}>()

const emit = defineEmits<{
  (e: 'select', botId: string): void
}>()
</script>

<template>
  <aside class="bot-list">
    <h3 class="bot-list-title">Bots</h3>
    <p v-if="bots.length === 0" class="empty">No bots configured yet.</p>
    <button
      v-for="bot in bots"
      :key="bot.botId"
      type="button"
      class="bot-item"
      :class="{ active: bot.botId === selectedBotId }"
      @click="emit('select', bot.botId)"
    >
      <span class="bot-name">{{ bot.botName || 'Unnamed bot' }}</span>
      <span class="status" :class="bot.enabled ? 'on' : 'off'">
        {{ bot.enabled ? 'Enabled' : 'Disabled' }}
      </span>
    </button>
  </aside>
</template>

<style scoped>
.bot-list {
  background-color: var(--surface);
  border: 1px solid var(--border-color);
  border-radius: 14px;
  padding: 1rem;
  box-shadow: 0 4px 16px var(--shadow);
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.bot-list-title {
  font-size: 1rem;
  color: var(--muted-color);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 0.5rem;
}

.empty {
  color: var(--muted-color);
  font-size: 0.9rem;
}

.bot-item {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.25rem;
  text-align: left;
  color: var(--text-color);
  border: 1px solid var(--border-color);
  background: var(--surface-2);
  border-radius: 10px;
  padding: 0.75rem 1rem;
  cursor: pointer;
  transition: all 0.15s ease;
}

.bot-item:hover {
  border-color: var(--violet);
  background: var(--surface-hover);
}

.bot-item.active {
  border-color: var(--foxfire);
  background: linear-gradient(90deg, rgba(232, 70, 122, 0.12), rgba(224, 102, 153, 0.08));
}

.bot-name {
  font-weight: 600;
}

.status {
  font-size: 0.75rem;
  font-weight: 500;
}

.status.on {
  color: var(--success-color);
}

.status.off {
  color: var(--muted-color);
}

@media (max-width: 768px) {
  /* Horizontal, swipeable strip of bot chips above the editor. */
  .bot-list {
    flex-direction: row;
    flex-wrap: nowrap;
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
    gap: 0.5rem;
  }
  .bot-list-title {
    display: none;
  }
  .bot-item {
    flex: 0 0 auto;
    min-width: 9rem;
  }
}
</style>
