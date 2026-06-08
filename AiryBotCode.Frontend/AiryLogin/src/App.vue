<script setup lang="ts">
import { onMounted, ref, nextTick } from 'vue'
import { API_BASE_URL, APP_URL, discordAuthUrl } from './lib/config'
import { isAuthenticated } from './lib/auth'

type Step = 'key' | 'identity'

const step = ref<Step>('key')
const password = ref('')
const gateToken = ref('')
const error = ref('')
const busy = ref(false)
const keyInput = ref<HTMLInputElement | null>(null)

onMounted(() => {
  // Already holding a valid session? Don't show the gate — go straight in.
  if (isAuthenticated()) {
    window.location.replace(APP_URL)
    return
  }
  keyInput.value?.focus()
})

async function submitKey() {
  if (busy.value || !password.value) return
  busy.value = true
  error.value = ''
  try {
    const res = await fetch(`${API_BASE_URL}/api/auth/gate`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ password: password.value }),
    })
    if (res.status === 401) {
      error.value = 'That key was not recognised.'
      return
    }
    if (!res.ok) {
      error.value = 'Something went wrong. Try again.'
      return
    }
    const data = (await res.json()) as { token: string }
    gateToken.value = data.token
    password.value = ''
    step.value = 'identity'
  } catch {
    error.value = 'Could not reach the server. Try again.'
  } finally {
    busy.value = false
  }
}

async function back() {
  step.value = 'key'
  error.value = ''
  await nextTick()
  keyInput.value?.focus()
}

function continueWithIdentity() {
  if (!gateToken.value) return
  busy.value = true
  window.location.href = discordAuthUrl(gateToken.value)
}
</script>

<template>
  <div class="stage">
    <div class="aurora" aria-hidden="true"></div>

    <main class="card" role="dialog" aria-modal="true">
      <div class="mark" aria-hidden="true"></div>

      <transition name="fade" mode="out-in">
        <!-- Step 1 — access key -->
        <form v-if="step === 'key'" key="key" class="panel" @submit.prevent="submitKey">
          <h1>Welcome back</h1>
          <p class="hint">Enter your access key to continue.</p>

          <input
            ref="keyInput"
            v-model="password"
            type="password"
            class="field"
            placeholder="Access key"
            autocomplete="current-password"
            :disabled="busy"
          />

          <p v-if="error" class="error">{{ error }}</p>

          <button type="submit" class="primary" :disabled="busy || !password">
            {{ busy ? 'Checking…' : 'Continue' }}
          </button>
        </form>

        <!-- Step 2 — identity -->
        <div v-else key="identity" class="panel">
          <h1>One more step</h1>
          <p class="hint">Confirm it's you to finish signing in.</p>

          <p v-if="error" class="error">{{ error }}</p>

          <button class="discord" :disabled="busy" @click="continueWithIdentity">
            <svg viewBox="0 0 24 24" width="20" height="20" fill="currentColor" aria-hidden="true">
              <path d="M20.317 4.369A19.79 19.79 0 0 0 15.432 3a13.7 13.7 0 0 0-.617 1.27 18.27 18.27 0 0 0-5.486 0A13.7 13.7 0 0 0 8.71 3a19.74 19.74 0 0 0-4.885 1.37C.73 9.045-.12 13.6.3 18.09a19.9 19.9 0 0 0 6.063 3.058c.488-.665.922-1.37 1.296-2.111a12.9 12.9 0 0 1-2.04-.978c.171-.125.339-.255.5-.389a14.2 14.2 0 0 0 12.087 0c.164.137.331.267.5.389-.652.387-1.336.716-2.043.979.375.74.81 1.445 1.297 2.11a19.84 19.84 0 0 0 6.066-3.058c.5-5.21-.838-9.726-3.51-13.72ZM8.02 15.33c-1.183 0-2.157-1.086-2.157-2.42 0-1.333.955-2.42 2.157-2.42 1.21 0 2.176 1.096 2.157 2.42 0 1.334-.955 2.42-2.157 2.42Zm7.975 0c-1.183 0-2.157-1.086-2.157-2.42 0-1.333.955-2.42 2.157-2.42 1.21 0 2.176 1.096 2.157 2.42 0 1.334-.946 2.42-2.157 2.42Z"/>
            </svg>
            <span>{{ busy ? 'Redirecting…' : 'Continue with Discord' }}</span>
          </button>

          <button class="ghost" type="button" :disabled="busy" @click="back">Back</button>
        </div>
      </transition>
    </main>

    <p class="footnote">protected area · authorised access only</p>
  </div>
</template>

<style scoped>
.stage {
  position: relative;
  min-height: 100dvh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1.5rem;
  padding: 1.5rem;
  overflow: hidden;
  background:
    radial-gradient(1200px 600px at 50% -10%, #2a1d33, transparent),
    linear-gradient(160deg, #140d1a, #0c0910 60%);
}

/* slow drifting glow behind the card */
.aurora {
  position: absolute;
  inset: -30% -10% auto -10%;
  height: 70%;
  background:
    radial-gradient(40% 60% at 30% 40%, rgba(232, 70, 122, 0.35), transparent 70%),
    radial-gradient(45% 55% at 70% 35%, rgba(138, 99, 210, 0.32), transparent 70%);
  filter: blur(40px);
  animation: drift 16s ease-in-out infinite alternate;
  pointer-events: none;
}
@keyframes drift {
  from { transform: translate3d(-4%, -2%, 0) scale(1); }
  to   { transform: translate3d(4%, 3%, 0) scale(1.1); }
}

.card {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 380px;
  padding: 2.25rem 2rem 2rem;
  border-radius: 22px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.12);
  box-shadow: 0 30px 80px rgba(0, 0, 0, 0.55);
  backdrop-filter: blur(18px);
  -webkit-backdrop-filter: blur(18px);
  text-align: center;
}

.mark {
  width: 56px;
  height: 56px;
  margin: 0 auto 1.4rem;
  border-radius: 50%;
  background: conic-gradient(from 0deg, #e8467a, #8a63d2, #e8467a);
  box-shadow: 0 0 28px rgba(232, 70, 122, 0.55);
  animation: spin 14s linear infinite;
  position: relative;
}
.mark::after {
  content: '';
  position: absolute;
  inset: 12px;
  border-radius: 50%;
  background: #140d1a;
}
@keyframes spin { to { transform: rotate(360deg); } }

.panel { display: flex; flex-direction: column; gap: 0.55rem; }

h1 {
  margin: 0;
  font-size: 1.4rem;
  font-weight: 650;
  color: #fff;
  letter-spacing: 0.2px;
}
.hint {
  margin: 0 0 0.6rem;
  font-size: 0.86rem;
  color: rgba(255, 255, 255, 0.55);
}

.field {
  width: 100%;
  box-sizing: border-box;
  padding: 0.85rem 1rem;
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.16);
  background: rgba(0, 0, 0, 0.25);
  color: #fff;
  font-size: 16px;
  outline: none;
  transition: border-color 0.18s ease, box-shadow 0.18s ease;
}
.field::placeholder { color: rgba(255, 255, 255, 0.4); }
.field:focus {
  border-color: #e8467a;
  box-shadow: 0 0 0 3px rgba(232, 70, 122, 0.25);
}

.error {
  margin: 0.1rem 0 0;
  font-size: 0.82rem;
  color: #ff9bb8;
}

button {
  cursor: pointer;
  font: inherit;
  border-radius: 12px;
  transition: transform 0.12s ease, opacity 0.18s ease, background 0.18s ease;
}
button:disabled { opacity: 0.55; cursor: default; }
button:not(:disabled):active { transform: translateY(1px); }

.primary {
  margin-top: 0.7rem;
  padding: 0.85rem 1rem;
  border: none;
  color: #fff;
  font-weight: 650;
  background: linear-gradient(90deg, #e8467a, #8a63d2);
}
.primary:not(:disabled):hover { filter: brightness(1.08); }

.discord {
  margin-top: 0.4rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.6rem;
  padding: 0.85rem 1rem;
  border: none;
  color: #fff;
  font-weight: 600;
  background: #5865f2;
}
.discord:not(:disabled):hover { background: #4a56d6; }

.ghost {
  margin-top: 0.55rem;
  padding: 0.55rem;
  background: transparent;
  border: none;
  color: rgba(255, 255, 255, 0.55);
  font-size: 0.85rem;
}
.ghost:not(:disabled):hover { color: #fff; }

.footnote {
  position: relative;
  z-index: 1;
  margin: 0;
  font-size: 0.72rem;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.28);
}

.fade-enter-active, .fade-leave-active { transition: opacity 0.18s ease, transform 0.18s ease; }
.fade-enter-from { opacity: 0; transform: translateY(6px); }
.fade-leave-to { opacity: 0; transform: translateY(-6px); }
</style>
