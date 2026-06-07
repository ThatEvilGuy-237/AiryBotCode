<script setup lang="ts">
// Static, component-based navigation. Each entry is a router-link so the active
// route is highlighted automatically via the exact-active class.
interface NavItem {
  to: string
  label: string
}

const items: NavItem[] = [
  { to: '/', label: 'Home' },
  { to: '/commands', label: 'Commands' },
  { to: '/bot-settings', label: 'Bot Settings' },
  { to: '/database', label: 'Database' },
]
</script>

<template>
  <aside class="sidebar">
    <div class="brand">
      <span class="emblem" aria-hidden="true"></span>
      <div class="brand-text">
        <h1>Airy</h1>
        <p>kitsune control ✦</p>
      </div>
    </div>

    <nav class="sidebar-nav">
      <router-link v-for="item in items" :key="item.to" :to="item.to" class="nav-link">
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

.brand {
  display: flex;
  align-items: center;
  gap: 0.85rem;
  margin-bottom: 2.25rem;
  padding: 0 0.25rem;
}

/* a softly spinning pink sigil */
.emblem {
  position: relative;
  width: 42px;
  height: 42px;
  border-radius: 50%;
  background: conic-gradient(from 0deg, var(--foxfire), var(--violet), var(--foxfire));
  animation: spin 12s linear infinite;
  flex-shrink: 0;
}
.emblem::after {
  content: '';
  position: absolute;
  inset: 9px;
  border-radius: 50%;
  background: #ffffff;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.brand-text h1 {
  font-size: 1.5rem;
  font-weight: 700;
  line-height: 1;
  background: linear-gradient(90deg, var(--foxfire), var(--violet));
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
}

.brand-text p {
  margin: 0.2rem 0 0;
  font-size: 0.72rem;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: var(--muted-color);
}

.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
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
