<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import AppSidebar from './components/AppSidebar.vue'

const navOpen = ref(false)
const route = useRoute()
// Close the mobile drawer whenever the route changes.
watch(() => route.path, () => (navOpen.value = false))

// Theming is now @hive/ui-driven: the legacy --foxfire/--surface tokens are bridged
// to the @hive/ui design tokens (style.css), and the image-derived theme is applied
// at startup (main.ts). No more per-bot pink override.
</script>

<template>
  <div class="app-layout" :class="{ 'nav-open': navOpen }">
    <!-- Mobile-only top bar -->
    <header class="topbar">
      <button class="menu-btn" aria-label="Open menu" @click="navOpen = true">☰</button>
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
.app-layout { display: grid; grid-template-columns: 250px 1fr; height: 100dvh; background: var(--color-bg); color: var(--color-fg); }
.topbar { display: none; }
.sidebar-slot { height: 100dvh; position: sticky; top: 0; }
.main-content { background: var(--color-bg); overflow-y: auto; height: 100dvh; }
.scrim { display: none; }

@media (max-width: 768px) {
  .app-layout { grid-template-columns: 1fr; grid-template-rows: 56px 1fr; }
  .topbar { display: flex; align-items: center; gap: var(--space-2); padding: 0 var(--space-3);
    background: var(--color-bg-2); border-bottom: var(--border); color: var(--color-fg); position: sticky; top: 0; z-index: 30; }
  .menu-btn { background: transparent; border: none; color: inherit; display: inline-flex; padding: var(--space-2);
    cursor: pointer; border-radius: var(--radius); font-size: 1.25rem; line-height: 1; }
  .menu-btn:active { background: var(--color-surface-mute); }
  .brand { font-weight: 700; font-size: var(--font-size-lg); }
  .sidebar-slot { position: fixed; top: 0; left: 0; bottom: 0; width: 80vw; max-width: 300px; height: 100dvh;
    transform: translateX(-100%); transition: transform .25s ease; z-index: 50; box-shadow: var(--shadow-2); }
  .app-layout.nav-open .sidebar-slot { transform: translateX(0); }
  .scrim { display: block; position: fixed; inset: 0; background: oklch(0 0 0 / .5); z-index: 40; }
  .main-content { height: auto; }
}
</style>
