<script setup lang="ts">
// Migrated to @hive/ui tokens (Item E — full panel on the shared design system).
// Grouped nav like the Hive portal's PortalSidebar: RouterLinks (SPA-safe) styled
// with the design tokens + accent active state, so it themes with the image theme.
import { useRoute } from 'vue-router'
import BotSwitcher from './BotSwitcher.vue'

const route = useRoute()

const botItems = [
  { to: '/commands', label: 'Commands' },
  { to: '/status', label: 'Status' },
  { to: '/bot-settings', label: 'Settings' },
  { to: '/theme', label: 'Theme' },
  { to: '/webhooks', label: 'Webhooks' },
]
const serverItems = [{ to: '/database', label: 'Databases' }]

const isActive = (to: string) => to === '/' ? route.path === '/' : route.path.startsWith(to)
</script>

<template>
  <aside class="sidebar">
    <div class="brand"><span class="orb" aria-hidden="true" />Airy</div>

    <BotSwitcher class="bot-switcher" />

    <nav class="nav">
      <RouterLink to="/" class="link" :class="{ active: route.path === '/' }">Home</RouterLink>
      <RouterLink to="/suggestions" class="link" :class="{ active: isActive('/suggestions') }">Suggestions</RouterLink>

      <p class="label">This bot</p>
      <RouterLink v-for="i in botItems" :key="i.to" :to="i.to" class="link" :class="{ active: isActive(i.to) }">{{ i.label }}</RouterLink>

      <p class="label">Server</p>
      <RouterLink v-for="i in serverItems" :key="i.to" :to="i.to" class="link" :class="{ active: isActive(i.to) }">{{ i.label }}</RouterLink>
    </nav>

    <p class="footer">// a mystical spirit of the digital realm</p>
  </aside>
</template>

<style scoped>
.sidebar { display: flex; flex-direction: column; gap: var(--space-2); height: 100%; padding: var(--space-4) var(--space-3);
  background: var(--color-bg-2); border-right: var(--border); color: var(--color-fg); overflow-y: auto; }
.brand { display: flex; align-items: center; gap: var(--space-2); font-weight: 700; font-size: var(--font-size-lg); margin-bottom: var(--space-2); }
.brand .orb { width: 10px; height: 10px; border-radius: 50%; background: var(--color-accent); }
.bot-switcher { margin-bottom: var(--space-3); }
.nav { display: flex; flex-direction: column; gap: 2px; }
.label { margin: var(--space-3) 0 var(--space-1); padding: 0 var(--space-2); font-family: var(--font-mono);
  font-size: var(--font-size-xs); letter-spacing: .08em; text-transform: uppercase; color: var(--color-text-ghost); }
.link { display: flex; align-items: center; height: 36px; padding: 0 var(--space-3); color: var(--color-text-dim);
  text-decoration: none; font-size: var(--font-size-base); font-weight: 500; border: 1px solid transparent;
  border-radius: var(--radius); transition: color .12s, background .12s, border-color .12s; }
.link:hover { color: var(--color-fg); background: var(--color-surface-mute); }
.link.active { color: var(--color-fg); background: var(--color-accent-soft); border-color: var(--color-accent-edge);
  box-shadow: inset 2px 0 0 0 var(--color-accent); }
.footer { margin-top: auto; padding: 0 var(--space-2); font-family: var(--font-mono); font-size: var(--font-size-xs); color: var(--color-text-ghost); }
</style>
