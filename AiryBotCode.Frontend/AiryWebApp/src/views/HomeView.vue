<script setup lang="ts">
// Migrated to @hive/ui (Item E) — the first real panel page on the shared design
// system, so it re-themes with the image-derived theme. Functionality unchanged.
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { PageHeader, Card, Button, StatusDot, Badge } from '@hive/ui'
import { token, clearToken } from '../lib/auth'
import { DISCORD_AUTH_URL, HIVE_URL } from '../lib/config'
import { api } from '../lib/api'
import { useBots } from '../lib/bots'

const apiResponse = ref('')

const router = useRouter()
const { bots, loadBots, loadError, selectBot } = useBots()

onMounted(() => {
  if (token.value) loadBots()
})

// Select a bot and jump to its read-only status overview.
function openStatus(botId: string): void {
  selectBot(botId)
  router.push('/status')
}

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

    <Card v-if="token">
      <template #head>
        <h3>Your bots</h3>
        <div style="flex: 1" />
        <Badge variant="inactive">{{ bots.length }}</Badge>
      </template>

      <p v-if="loadError" class="muted">{{ loadError }}</p>
      <p v-else-if="!bots.length" class="muted">No bots yet — add one from the Settings page.</p>
      <ul v-else class="roster">
        <li v-for="b in bots" :key="b.botId" class="bot" @click="openStatus(b.botId)">
          <StatusDot :variant="b.enabled ? 'ok' : 'off'" />
          <span class="bname">{{ b.botName || 'Unnamed bot' }}</span>
          <span class="bid mono">{{ b.botId }}</span>
          <Badge :variant="b.enabled ? 'active' : 'inactive'">{{ b.enabled ? 'enabled' : 'disabled' }}</Badge>
          <span class="go mono">status →</span>
        </li>
      </ul>
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
.muted { color: var(--color-muted); }

.roster { list-style: none; margin: 0; padding: 0; display: flex; flex-direction: column; gap: 2px; }
.bot { display: flex; align-items: center; gap: var(--space-3); padding: var(--space-2) var(--space-3);
  border: 1px solid transparent; border-radius: var(--radius); cursor: pointer;
  transition: background .12s, border-color .12s; }
.bot:hover { background: var(--color-surface-mute); border-color: var(--color-accent-edge); }
.bname { font-weight: 500; }
.bid { flex: 1; font-size: var(--font-size-sm); overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.go { font-size: var(--font-size-sm); color: var(--color-accent); opacity: 0; transition: opacity .12s; }
.bot:hover .go { opacity: 1; }
@media (max-width: 600px) { .bid { display: none; } .go { opacity: 1; } }
</style>
