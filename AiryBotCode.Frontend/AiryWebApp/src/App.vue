<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import AppSidebar from './components/AppSidebar.vue'

const navOpen = ref(false)
const route = useRoute()
// Close the mobile drawer whenever the route changes.
watch(() => route.path, () => (navOpen.value = false))
</script>

<template>
  <div class="app-layout" :class="{ 'nav-open': navOpen }">
    <!-- Mobile-only top bar -->
    <header class="topbar">
      <button class="menu-btn" aria-label="Open menu" @click="navOpen = true">
        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="18" x2="21" y2="18"/></svg>
      </button>
      <span class="brand">Airy</span>
    </header>

    <div class="sidebar-slot">
      <AppSidebar />
    </div>

    <div v-if="navOpen" class="scrim" @click="navOpen = false"></div>

    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
.app-layout {
  display: grid;
  grid-template-columns: var(--sidebar-width, 250px) 1fr;
  height: 100dvh;
}

.topbar { display: none; }

.sidebar-slot {
  height: 100dvh;
  position: sticky;
  top: 0;
}

.main-content {
  background-color: transparent;
  overflow-y: auto;
  height: 100dvh;
}

.scrim { display: none; }

@media (max-width: 768px) {
  .app-layout {
    grid-template-columns: 1fr;
    grid-template-rows: var(--header-height, 56px) 1fr;
    height: 100dvh;
  }

  .topbar {
    display: flex;
    align-items: center;
    gap: 0.6rem;
    padding: 0 0.75rem;
    background: linear-gradient(90deg, var(--foxfire), var(--violet));
    color: #fff;
    position: sticky;
    top: 0;
    z-index: 30;
  }
  .menu-btn {
    background: transparent;
    border: none;
    color: inherit;
    display: inline-flex;
    padding: 0.55rem;
    cursor: pointer;
    border-radius: 8px;
  }
  .menu-btn:active { background: rgba(255, 255, 255, 0.2); }
  .brand { font-weight: 700; font-size: 1.15rem; }

  .sidebar-slot {
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    width: 80vw;
    max-width: 300px;
    height: 100dvh;
    transform: translateX(-100%);
    transition: transform 0.25s ease;
    z-index: 50;
    box-shadow: 4px 0 24px rgba(0, 0, 0, 0.2);
  }
  .app-layout.nav-open .sidebar-slot { transform: translateX(0); }

  .scrim {
    display: block;
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.4);
    z-index: 40;
  }

  .main-content { height: auto; }
}
</style>
