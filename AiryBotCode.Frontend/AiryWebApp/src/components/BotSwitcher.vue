<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useBots } from '../lib/bots'
import { loadImageTheme } from '../lib/imageTheme'

const router = useRouter()
const { bots, currentBot, currentBotId, selectBot, loadBots, requestCreate } = useBots()
const open = ref(false)

onMounted(() => loadBots())

// A bot's theme image doubles as its profile picture (null → letter avatar).
function avatar(botId?: string | null): string | null {
  return botId ? (loadImageTheme(botId)?.image ?? null) : null
}

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
      <span class="avatar">
        <img v-if="avatar(currentBotId)" :src="avatar(currentBotId)!" alt="" />
        <template v-else>{{ initial(currentBot?.botName) }}</template>
      </span>
      <span class="meta">
        <span class="name">{{ currentBot?.botName || 'No bot selected' }}</span>
        <span class="sub">switch bot</span>
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
          <span class="avatar sm">
            <img v-if="avatar(b.botId)" :src="avatar(b.botId)!" alt="" />
            <template v-else>{{ initial(b.botName) }}</template>
          </span>
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
.switcher { position: relative; }

.trigger { display: flex; align-items: center; gap: var(--space-3); width: 100%; padding: var(--space-2) var(--space-3);
  background: var(--color-surface); border: var(--border); border-radius: var(--r-3); cursor: pointer; text-align: left;
  color: var(--color-fg); transition: border-color .15s, background .15s; }
.trigger:hover { border-color: var(--color-border-hi); background: var(--color-surface-hi); }

.avatar { flex-shrink: 0; width: 38px; height: 38px; border-radius: var(--r-2); display: grid; place-items: center;
  font-weight: 700; color: var(--color-bg); background: var(--color-accent); overflow: hidden; }
.avatar img { width: 100%; height: 100%; object-fit: cover; display: block; }
.avatar.sm { width: 30px; height: 30px; border-radius: var(--r-1); font-size: var(--font-size-sm); }
.avatar.plus { background: var(--color-surface-mute); color: var(--color-accent); border: 1px dashed var(--color-accent-edge); }

.meta { display: flex; flex-direction: column; min-width: 0; flex: 1; }
.name { font-weight: 700; color: var(--color-fg); overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.sub { font-family: var(--font-mono); font-size: var(--font-size-xs); letter-spacing: .08em; text-transform: uppercase; color: var(--color-muted); }
.chev { color: var(--color-muted); transition: transform .18s ease; }
.chev.up { transform: rotate(180deg); }

.backdrop { position: fixed; inset: 0; z-index: 60; }
.menu { position: absolute; top: calc(100% + var(--space-1)); left: 0; right: 0; z-index: 61;
  background: var(--color-surface); border: var(--border); border-radius: var(--r-3); box-shadow: var(--shadow-2);
  padding: var(--space-1); max-height: 60vh; overflow-y: auto; }

.item { display: flex; align-items: center; gap: var(--space-2); width: 100%; padding: var(--space-2);
  border: 1px solid transparent; background: transparent; border-radius: var(--r-2); cursor: pointer; text-align: left;
  font: inherit; color: var(--color-text-dim); }
.item:hover { background: var(--color-surface-hi); color: var(--color-fg); }
.item.active { background: var(--color-accent-soft); border-color: var(--color-accent-edge); color: var(--color-fg); }
.item-name { flex: 1; min-width: 0; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; font-weight: 500; }
.check { color: var(--color-accent); font-weight: 700; }
.item.new .item-name { color: var(--color-accent); font-weight: 600; }
.empty { margin: var(--space-1) var(--space-2); color: var(--color-muted); font-size: var(--font-size-sm); }
.divider { height: 1px; background: var(--color-border-soft); margin: var(--space-1) var(--space-1); }
</style>
