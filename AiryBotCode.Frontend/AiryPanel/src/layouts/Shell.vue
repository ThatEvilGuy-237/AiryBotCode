<script setup lang="ts">
import { computed, nextTick, onMounted, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { ActionPill, AppSidebar, BrandMark, Button, Icon, SidebarItem, SidebarSection, ToastHost, useAppUpdate, useInstallPrompt } from '@airy/ui'
import BotSwitcher from '@/components/BotSwitcher.vue'
import { signOut } from '@/lib/auth'
import { loadBots } from '@/lib/bots'

const route = useRoute()
const menuOpen = ref(false)
const side = ref<HTMLElement | null>(null)

const { available: updateReady, updating, update } = useAppUpdate({ current: __BUILD__, url: '/version.json', read: body => (body as { build?: string }).build })
const { canInstall, install } = useInstallPrompt()
const installLater = ref(true)
const pill = computed(() => (updateReady.value ? 'update' : canInstall.value && !installLater.value ? 'install' : null))

function notNow() {
  installLater.value = true
  localStorage.setItem('airy.install-later', String(Date.now() + 30 * 86400000))
}

async function openMenu() {
  menuOpen.value = true
  await nextTick()
  side.value?.querySelector<HTMLElement>('a, select, button')?.focus()
}

watch(() => route.fullPath, () => { menuOpen.value = false })

onMounted(() => {
  installLater.value = Number(localStorage.getItem('airy.install-later') ?? 0) > Date.now()
  loadBots()
})
</script>

<template>
  <div class="shell" :class="{ open: menuOpen }" @keydown.esc="menuOpen = false">
    <div ref="side" class="side">
      <AppSidebar width="256px" label="Airy">
        <template #brand>
          <div class="brand">
            <BrandMark name="Airy" :size="26" />
            <BotSwitcher />
          </div>
        </template>
        <SidebarSection label="This bot" name="bot">
          <SidebarItem to="/" exact icon="home" label="Home" />
          <SidebarItem to="/channels" icon="hash" label="Channels" />
          <SidebarItem to="/commands" icon="wrench" label="Commands" />
          <SidebarItem to="/activity" icon="history" label="Activity" />
          <SidebarItem to="/look" icon="image" label="Look" />
          <SidebarItem to="/settings" icon="settings" label="Settings" />
        </SidebarSection>
        <SidebarSection label="All bots" name="all">
          <SidebarItem to="/suggestions" icon="inbox" label="Suggestions" />
          <SidebarItem to="/databases" icon="database" label="Databases" />
        </SidebarSection>
      </AppSidebar>
    </div>
    <div class="scrim" aria-hidden="true" @click="menuOpen = false" />
    <div class="main" :inert="menuOpen || undefined">
      <header class="topbar">
        <button class="menu" type="button" aria-label="Open the menu" @click="openMenu">
          <Icon name="menu" :size="20" />
        </button>
        <BrandMark class="phone-mark" name="Airy" :size="22" />
        <span class="spacer" />
        <ActionPill v-if="pill === 'update'" class="top-pill" icon="refresh-cw" label="Update" busy-label="Updating" :busy="updating" @click="update" />
        <ActionPill v-else-if="pill === 'install'" class="top-pill" icon="download" label="Install app" dismiss-label="Not now" @click="install" @dismiss="notNow" />
        <Button variant="ghost" size="sm" @click="signOut"><Icon name="log-out" :size="16" />Sign out</Button>
      </header>
      <div v-if="pill" class="pill-row">
        <ActionPill v-if="pill === 'update'" icon="refresh-cw" label="Update" busy-label="Updating" :busy="updating" @click="update" />
        <ActionPill v-else icon="download" label="Install app" dismiss-label="Not now" @click="install" @dismiss="notNow" />
      </div>
      <main class="content">
        <RouterView />
      </main>
    </div>
    <ToastHost />
  </div>
</template>

<style scoped>
.shell {
  display: grid;
  grid-template-columns: 256px minmax(0, 1fr);
  min-height: 100vh;
  min-height: 100dvh;
  background: var(--color-bg);
}
.side {
  position: sticky;
  top: 0;
  height: 100vh;
  height: 100dvh;
}
.side :deep([data-ui='app-sidebar']) {
  height: 100%;
}
.brand {
  display: grid;
  gap: var(--space-3);
  padding: var(--space-4) var(--space-3) var(--space-3);
}
.main {
  min-width: 0;
  display: flex;
  flex-direction: column;
}
.topbar {
  position: sticky;
  top: 0;
  z-index: 5;
  display: flex;
  align-items: center;
  gap: var(--space-3);
  min-height: 56px;
  padding: 0 var(--space-6);
  background: var(--color-bg);
  border-bottom: var(--border);
}
.spacer {
  flex: 1;
}
.menu,
.phone-mark,
.scrim,
.pill-row {
  display: none;
}
.content {
  width: 100%;
  max-width: 1120px;
  padding: var(--space-6);
}
@media (max-width: 860px) {
  .shell {
    grid-template-columns: minmax(0, 1fr);
  }
  .side {
    position: fixed;
    inset: 0 auto 0 0;
    z-index: 20;
    width: min(300px, 86vw);
    --sidebar-item-h: 44px;
    transform: translateX(-100%);
    visibility: hidden;
    transition: transform 0.2s ease, visibility 0s linear 0.2s;
  }
  .open .side {
    transform: none;
    visibility: visible;
    transition: transform 0.2s ease;
  }
  .open .scrim {
    display: block;
    position: fixed;
    inset: 0;
    z-index: 15;
    background: color-mix(in srgb, var(--color-bg) 70%, transparent);
  }
  .menu {
    display: grid;
    place-items: center;
    width: 44px;
    height: 44px;
    margin-left: calc(var(--space-2) * -1);
    border: 0;
    border-radius: var(--r-2);
    background: transparent;
    color: var(--color-fg);
    cursor: pointer;
  }
  .menu:focus-visible {
    outline: 2px solid var(--color-accent);
  }
  .phone-mark {
    display: inline-flex;
  }
  .top-pill {
    display: none;
  }
  .pill-row {
    display: flex;
    padding: var(--space-3) var(--space-4) 0;
  }
  .topbar {
    padding: 0 var(--space-4);
  }
  .content {
    padding: var(--space-4);
  }
}
@media (prefers-reduced-motion: reduce) {
  .side,
  .open .side {
    transition: none;
  }
}
</style>
