<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useBots } from '../lib/bots'

const router = useRouter()
const { bots, currentBot, currentBotId, selectBot, loadBots, requestCreate } = useBots()
const open = ref(false)

onMounted(() => loadBots())

function pick(id: string) {
  selectBot(id)
  open.value = false
}

function newBot() {
  open.value = false
  requestCreate()
  router.push({ name: 'bot-settings' })
}

function initial(name?: string | null) {
  return (name?.trim()?.[0] ?? '?').toUpperCase()
}
</script>

<template>
  <div class="switcher">
    <button class="trigger" type="button" @click="open = !open">
      <span class="avatar">{{ initial(currentBot?.botName) }}</span>
      <span class="meta">
        <span class="name">{{ currentBot?.botName || 'No bot selected' }}</span>
        <span class="sub">switch bot ✦</span>
      </span>
      <span class="chev" :class="{ up: open }">▾</span>
    </button>

    <template v-if="open">
      <div class="backdrop" @click="open = false"></div>
      <div class="menu" role="menu">
        <button
          v-for="b in bots"
          :key="b.botId"
          type="button"
          class="item"
          :class="{ active: b.botId === currentBotId }"
          @click="pick(b.botId)"
        >
          <span class="avatar sm">{{ initial(b.botName) }}</span>
          <span class="item-name">{{ b.botName || 'Unnamed' }}</span>
          <span v-if="b.botId === currentBotId" class="check">✓</span>
        </button>
        <p v-if="!bots.length" class="empty">No bots yet.</p>

        <div class="divider"></div>
        <button type="button" class="item new" @click="newBot">
          <span class="avatar sm plus">＋</span>
          <span class="item-name">New bot</span>
        </button>
      </div>
    </template>
  </div>
</template>

<style scoped>
.switcher {
  position: relative;
}

.trigger {
  display: flex;
  align-items: center;
  gap: 0.7rem;
  width: 100%;
  padding: 0.6rem 0.7rem;
  background: #fff;
  border: 1px solid var(--border-color);
  border-radius: 12px;
  cursor: pointer;
  text-align: left;
  transition: border-color 0.15s ease, box-shadow 0.15s ease;
}
.trigger:hover {
  border-color: var(--foxfire);
  box-shadow: 0 4px 14px rgba(232, 70, 122, 0.12);
}

.avatar {
  flex-shrink: 0;
  width: 38px;
  height: 38px;
  border-radius: 11px;
  display: grid;
  place-items: center;
  font-weight: 700;
  color: #fff;
  background: linear-gradient(135deg, var(--foxfire), var(--violet));
  box-shadow: 0 2px 8px rgba(232, 70, 122, 0.35);
}
.avatar.sm {
  width: 30px;
  height: 30px;
  border-radius: 9px;
  font-size: 0.85rem;
}
.avatar.plus {
  background: var(--surface-hover);
  color: var(--foxfire);
  box-shadow: none;
  border: 1px dashed var(--foxfire);
}

.meta {
  display: flex;
  flex-direction: column;
  min-width: 0;
  flex: 1;
}
.name {
  font-weight: 700;
  color: var(--text-color);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.sub {
  font-size: 0.68rem;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--muted-color);
}
.chev {
  color: var(--muted-color);
  transition: transform 0.18s ease;
}
.chev.up {
  transform: rotate(180deg);
}

.backdrop {
  position: fixed;
  inset: 0;
  z-index: 60;
}
.menu {
  position: absolute;
  top: calc(100% + 0.4rem);
  left: 0;
  right: 0;
  z-index: 61;
  background: #fff;
  border: 1px solid var(--border-color);
  border-radius: 12px;
  box-shadow: 0 18px 44px rgba(58, 44, 54, 0.22);
  padding: 0.4rem;
  max-height: 60vh;
  overflow-y: auto;
}

.item {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  width: 100%;
  padding: 0.5rem 0.55rem;
  border: none;
  background: transparent;
  border-radius: 9px;
  cursor: pointer;
  text-align: left;
  font: inherit;
  color: var(--text-color);
}
.item:hover {
  background: var(--surface-hover);
}
.item.active {
  background: rgba(232, 70, 122, 0.08);
}
.item-name {
  flex: 1;
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-weight: 500;
}
.check {
  color: var(--foxfire);
  font-weight: 700;
}
.item.new .item-name {
  color: var(--foxfire-deep);
  font-weight: 600;
}
.empty {
  margin: 0.3rem 0.55rem;
  color: var(--muted-color);
  font-size: 0.85rem;
}
.divider {
  height: 1px;
  background: var(--border-color);
  margin: 0.4rem 0.2rem;
}
</style>
