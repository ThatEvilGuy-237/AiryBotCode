<script setup lang="ts">
import { ref } from 'vue'
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
    const message = await api.ping()
    apiResponse.value = `Success! API says: "${message}"`
  } catch (e) {
    apiResponse.value = e instanceof Error ? e.message : 'Ping failed.'
  }
}
</script>

<template>
  <div class="page">
    <header class="page-header">
      <h1>Airy Control Panel</h1>
      <p>Tend to your kitsune spirits of the digital realm — all from one den. ✦</p>
    </header>

    <section class="card">
      <template v-if="token">
        <p class="status logged-in">You are logged in.</p>
        <div class="btn-row">
          <button class="btn hive" @click="openHive">⬡ Open The Hive</button>
          <button class="btn ghost" @click="logout">Log out</button>
        </div>
      </template>
      <template v-else>
        <p class="status">You are not logged in.</p>
        <button class="btn" @click="login">Login with Discord</button>
      </template>
    </section>

    <section class="card">
      <h2>API Connection</h2>
      <button class="btn" :disabled="!token" @click="pingApi">Ping API</button>
      <p class="response">
        <span class="muted">Response:</span> <code>{{ apiResponse || '—' }}</code>
      </p>
    </section>
  </div>
</template>

<style scoped>
.page {
  max-width: 640px;
  margin: 0 auto;
  padding: 2.5rem 1.5rem;
}

.page-header {
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2rem;
  background: linear-gradient(90deg, var(--foxfire), var(--violet));
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
}

.page-header p {
  color: var(--muted-color);
  margin-top: 0.5rem;
}

.card {
  background-color: var(--surface);
  border: 1px solid var(--border-color);
  border-radius: 14px;
  padding: 1.5rem;
  margin-bottom: 1.25rem;
  box-shadow: 0 4px 16px var(--shadow);
}

.card h2 {
  font-size: 1.15rem;
  margin-bottom: 1rem;
}

.status {
  margin-bottom: 1rem;
}

.status.logged-in {
  color: var(--success-color);
  font-weight: 500;
}

.btn {
  background: linear-gradient(90deg, var(--foxfire), var(--foxfire-deep));
  color: #fff;
  border: none;
  padding: 0.7rem 1.6rem;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  box-shadow: 0 2px 8px var(--shadow);
  transition: transform 0.15s ease, filter 0.2s ease;
}

.btn:hover:not(:disabled) {
  filter: brightness(1.05);
  transform: translateY(-1px);
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-row {
  display: flex;
  gap: 0.6rem;
  flex-wrap: wrap;
}
.btn.hive {
  background: linear-gradient(90deg, #f6b73c, var(--foxfire));
}
.btn.ghost {
  background: #fff;
  color: var(--text-color);
  border: 1px solid var(--border-color);
  box-shadow: none;
}
.btn.ghost:hover:not(:disabled) {
  background: var(--surface-hover);
  filter: none;
}

.response {
  margin-top: 1rem;
}

.response code {
  background: var(--surface-2);
  border: 1px solid var(--border-color);
  border-radius: 6px;
  padding: 0.15rem 0.45rem;
  color: var(--foxfire);
}

.muted {
  color: var(--muted-color);
}
</style>
