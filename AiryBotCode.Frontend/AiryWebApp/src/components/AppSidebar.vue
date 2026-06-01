<script setup lang="ts">
// Static, component-based navigation. Each entry is a router-link so the active
// route is highlighted automatically via the .router-link-active class.
interface NavItem {
  to: string
  label: string
}

const items: NavItem[] = [
  { to: '/', label: 'Home' },
  { to: '/bot-settings', label: 'Bot Settings' },
  { to: '/database', label: 'Database' },
]
</script>

<template>
  <aside class="sidebar">
    <div class="sidebar-header">
      <h2>Airy Control</h2>
    </div>
    <nav class="sidebar-nav">
      <router-link v-for="item in items" :key="item.to" :to="item.to" class="nav-link">
        <span class="dot" aria-hidden="true"></span>
        <span>{{ item.label }}</span>
      </router-link>
    </nav>
  </aside>
</template>

<style scoped>
.sidebar {
  display: flex;
  flex-direction: column;
  background-color: #2c3e50;
  color: #ecf0f1;
  height: 100vh;
  padding: 1rem;
}

.sidebar-header {
  margin-bottom: 2rem;
  text-align: center;
}

.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: #ecf0f1;
  text-decoration: none;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  font-weight: 500;
  transition: background-color 0.2s;
}

.nav-link:hover {
  background-color: #34495e;
}

.nav-link.router-link-exact-active {
  background-color: var(--primary-color);
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: currentColor;
  opacity: 0.6;
}
</style>
