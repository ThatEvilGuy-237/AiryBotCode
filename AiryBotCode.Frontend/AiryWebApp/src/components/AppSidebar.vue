<script setup lang="ts">
// Navigation grouped by scope: the switcher selects a bot, the "This bot" links
// act on it, and "Server" links are global (not per-bot).
import BotSwitcher from './BotSwitcher.vue'

interface NavItem {
  to: string
  label: string
}

const botItems: NavItem[] = [
  { to: '/commands', label: 'Commands' },
  { to: '/bot-settings', label: 'Settings' },
  { to: '/theme', label: 'Theme' },
]
const serverItems: NavItem[] = [{ to: '/database', label: 'Databases' }]
</script>

<template>
  <aside class="sidebar">
    <BotSwitcher class="bot-switcher" />

    <router-link to="/" class="nav-link home-link">
      <span class="orb" aria-hidden="true"></span>
      <span>Home</span>
    </router-link>

    <nav class="sidebar-nav">
      <p class="nav-label">This bot</p>
      <router-link v-for="item in botItems" :key="item.to" :to="item.to" class="nav-link">
        <span class="orb" aria-hidden="true"></span>
        <span>{{ item.label }}</span>
      </router-link>

      <p class="nav-label">Server</p>
      <router-link v-for="item in serverItems" :key="item.to" :to="item.to" class="nav-link">
        <span class="orb" aria-hidden="true"></span>
        <span>{{ item.label }}</span>
      </router-link>
    </nav>

    <p class="footer-note">a mystical spirit of the digital realm</p>
  </aside>
</template>

<style scoped>
.sidebar {
  display: flex;
  flex-direction: column;
  background: linear-gradient(180deg, var(--surface), var(--surface-hover));
  border-right: 1px solid var(--border-color);
  color: var(--text-color);
  height: 100%;
  padding: 1.5rem 1rem;
  overflow-y: auto;
}

.bot-switcher {
  margin-bottom: 1rem;
}

.home-link {
  margin-bottom: 0.75rem;
}

.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
}

.nav-label {
  margin: 0.6rem 0 0.1rem;
  padding: 0 0.4rem;
  font-size: 0.68rem;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: var(--muted-color);
  opacity: 0.8;
}
.nav-label:first-child {
  margin-top: 0;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: var(--text-color);
  text-decoration: none;
  padding: 0.7rem 0.9rem;
  border-radius: 10px;
  font-weight: 500;
  transition: all 0.2s ease;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.55);
}

.nav-link.router-link-exact-active {
  background: #ffffff;
  color: var(--foxfire-deep);
  box-shadow: inset 0 0 0 1px rgba(232, 70, 122, 0.3);
}

.orb {
  width: 9px;
  height: 9px;
  border-radius: 50%;
  background: var(--muted-color);
  transition: all 0.2s ease;
}

.nav-link:hover .orb {
  background: var(--violet);
}

.nav-link.router-link-exact-active .orb {
  background: var(--foxfire);
}

.footer-note {
  margin-top: auto;
  padding: 0 0.5rem;
  font-size: 0.72rem;
  font-style: italic;
  color: var(--muted-color);
  opacity: 0.7;
}
</style>
