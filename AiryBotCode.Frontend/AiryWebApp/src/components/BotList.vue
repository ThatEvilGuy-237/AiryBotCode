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
  background-color: var(--card-background);
  border-radius: 12px;
  padding: 1rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
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
  border: 1px solid var(--border-color);
  background: #fff;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  cursor: pointer;
  transition: all 0.15s ease;
}

.bot-item:hover {
  border-color: var(--primary-color);
}

.bot-item.active {
  border-color: var(--primary-color);
  background-color: #eef5fd;
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
</style>
