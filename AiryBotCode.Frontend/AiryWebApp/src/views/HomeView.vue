<script setup lang="ts">
import { ref } from 'vue'
import { token, clearToken } from '../lib/auth'
import { DISCORD_AUTH_URL } from '../lib/config'
import { api } from '../lib/api'

const apiResponse = ref('')

function login(): void {
  window.location.href = DISCORD_AUTH_URL
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
      <h1>Airy Bot Control Panel</h1>
      <p>Manage your Discord bots from one place.</p>
    </header>

    <section class="card">
      <template v-if="token">
        <p class="status logged-in">You are logged in.</p>
        <button class="btn" @click="logout">Log out</button>
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

.page-header p {
  color: var(--muted-color);
  margin-top: 0.5rem;
}

.card {
  background-color: var(--card-background);
  border: 1px solid var(--border-color);
  border-radius: 12px;
  padding: 1.5rem;
  margin-bottom: 1.25rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.04);
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
  background-color: var(--primary-color);
  color: #fff;
  border: none;
  padding: 0.7rem 1.5rem;
  border-radius: 8px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: background-color 0.2s;
}

.btn:hover:not(:disabled) {
  background-color: var(--primary-hover);
}

.btn:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.response {
  margin-top: 1rem;
}

.muted {
  color: var(--muted-color);
}
</style>
