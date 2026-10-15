<script setup lang="ts">
import { computed } from 'vue'
import { Field, Select } from '@airy/ui'
import { bot, botId, botImage, bots, pickBot } from '@/lib/bots'

const options = computed(() => bots.value.map(b => ({ label: b.botName || b.botId, value: b.botId })))
const value = computed({
  get: () => botId.value ?? '',
  set: (id: string) => pickBot(id),
})
const initial = computed(() => (bot.value?.botName ?? '?').trim().charAt(0).toUpperCase())
</script>

<template>
  <div class="switcher">
    <span class="avatar" aria-hidden="true">
      <img v-if="botImage" :src="botImage" alt="" />
      <span v-else>{{ initial }}</span>
    </span>
    <Field label="Bot" hide-label>
      <Select v-model="value" :options="options" placeholder="No bots yet" :disabled="!options.length" />
    </Field>
  </div>
</template>

<style scoped>
.switcher {
  display: grid;
  grid-template-columns: 36px minmax(0, 1fr);
  align-items: center;
  gap: var(--space-2);
}
.avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  overflow: hidden;
  display: grid;
  place-items: center;
  background: var(--color-surface-hi);
  color: var(--color-fg);
  font-weight: 700;
}
.avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
</style>
