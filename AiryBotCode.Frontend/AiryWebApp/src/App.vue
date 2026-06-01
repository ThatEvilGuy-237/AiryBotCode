<script setup lang="ts">
import { onMounted } from 'vue'
import AppSidebar from './components/AppSidebar.vue'
import { captureTokenFromHash } from './lib/auth'

// Grab the JWT the API hands back via the #token=… redirect fragment, if any.
onMounted(captureTokenFromHash)
</script>

<template>
  <div class="app-layout">
    <AppSidebar />
    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
.app-layout {
  display: grid;
  grid-template-columns: var(--sidebar-width, 250px) 1fr;
  height: 100vh;
}

.main-content {
  background-color: var(--background-color);
  overflow-y: auto;
  height: 100vh;
}
</style>
