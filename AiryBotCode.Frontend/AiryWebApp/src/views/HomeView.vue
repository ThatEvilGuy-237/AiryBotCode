<script setup lang="ts">
// Migrated to @hive/ui (Item E) — the first real panel page on the shared design
// system, so it re-themes with the image-derived theme. Functionality unchanged.
import { ref } from 'vue'
import { PageHeader, Card, Button, StatusDot, Badge } from '@hive/ui'
import { token, clearToken } from '../lib/auth'
import { DISCORD_AUTH_URL, HIVE_URL } from '../lib/config'
import { api } from '../lib/api'

const apiResponse = ref('')

function login(): void {
  window.location.href = DISCORD_AUTH_URL
}

// Jump to The Hive, carrying the shared token across origins via the fragment.
function openHive(): void {
  window.location.href = `${HIVE_URL}/#token=${encodeURIComponent(token.value ?? '')}`
}

function logout(): void {
  clearToken()
  apiResponse.value = 'Logged out.'
}

async function pingApi(): Promise<void> {
  apiResponse.value = 'Pinging…'
  try {
    apiResponse.value = `API says: "${await api.ping()}"`
  } catch (e) {
    apiResponse.value = e instanceof Error ? e.message : 'Ping failed.'
  }
}
</script>

<template>
  <div class="home">
    <PageHeader
      title="Airy Control Panel"
      subtitle="Tend to your kitsune spirits of the digital realm — all from one den. ✦"
    />

    <Card>
      <template #head>
        <StatusDot :variant="token ? 'ok' : 'off'" />
        <h3>Session</h3>
        <div style="flex: 1" />
        <Badge :variant="token ? 'active' : 'inactive'">{{ token ? 'logged in' : 'logged out' }}</Badge>
      </template>

      <p v-if="token">You're signed in. Jump into The Hive or sign out.</p>
      <p v-else>You are not logged in.</p>

      <template #foot>
        <template v-if="token">
          <Button @click="openHive">⬡ Open The Hive</Button>
          <Button variant="ghost" @click="logout">Log out</Button>
        </template>
        <Button v-else @click="login">Login with Discord</Button>
      </template>
    </Card>

    <Card>
      <template #head><h3>API Connection</h3></template>
      <p><Button :disabled="!token" @click="pingApi">Ping API</Button></p>
      <p class="resp"><span class="mono">Response:</span> <code>{{ apiResponse || '—' }}</code></p>
    </Card>
  </div>
</template>

<style scoped>
.home { max-width: 720px; margin: 0 auto; padding: 2rem 1.5rem; display: flex; flex-direction: column; gap: 1rem; }
.resp { margin-top: .75rem; }
.mono { font-family: var(--font-mono); color: var(--color-muted); }
.home code { font-family: var(--font-mono); color: var(--color-accent); }
</style>
