<script setup lang="ts">
import { ref, watch } from 'vue'
import { api, ApiError } from '../lib/api'
import type { BotSetting } from '../types/botSetting'

// `bot` is the parent's working draft. Inputs bind straight onto its fields with
// v-model; the parent owns persistence and reacts to the `save` event.
const props = defineProps<{
  bot: BotSetting
  saving: boolean
  creating?: boolean
  databases?: string[]
}>()

const emit = defineEmits<{
  (e: 'save'): void
}>()

// --- Password-gated token reveal ---
const revealOpen = ref(false)
const revealPassword = ref('')
const revealedToken = ref<string | null>(null)
const revealBusy = ref(false)
const revealError = ref('')
const copied = ref(false)

async function submitReveal(): Promise<void> {
  if (revealBusy.value || !revealPassword.value) return
  revealBusy.value = true
  revealError.value = ''
  try {
    revealedToken.value = await api.revealBotToken(props.bot.botId, revealPassword.value)
    revealOpen.value = false
    revealPassword.value = ''
  } catch (e) {
    revealError.value = e instanceof ApiError ? e.message : 'Could not reveal token.'
  } finally {
    revealBusy.value = false
  }
}

function cancelReveal(): void {
  revealOpen.value = false
  revealPassword.value = ''
  revealError.value = ''
}

function hideToken(): void {
  revealedToken.value = null
  copied.value = false
}

async function copyToken(): Promise<void> {
  if (revealedToken.value == null) return
  try {
    await navigator.clipboard.writeText(revealedToken.value)
    copied.value = true
    setTimeout(() => (copied.value = false), 1500)
  } catch {
    /* clipboard unavailable — ignore */
  }
}

// Switching bots must never leave a previous bot's token on screen.
watch(
  () => props.bot.botId,
  () => {
    revealedToken.value = null
    revealOpen.value = false
    revealPassword.value = ''
    revealError.value = ''
    copied.value = false
  },
)
</script>

<template>
  <form class="settings-form" @submit.prevent="emit('save')">
    <section class="group">
      <h3>General</h3>
      <div class="form-group">
        <label for="botId">Bot ID</label>
        <input id="botId" type="text" inputmode="numeric" v-model="bot.botId" />
        <small v-if="creating">The Discord application / bot user ID.</small>
        <small v-else>Changing this re-creates the bot under the new ID.</small>
      </div>
      <div class="form-group">
        <label for="botName">Bot Name</label>
        <input id="botName" type="text" v-model="bot.botName" />
      </div>
      <label class="check">
        <input type="checkbox" v-model="bot.enabled" />
        <span>Enabled</span>
      </label>
    </section>

    <section class="group">
      <h3>Channels</h3>
      <p class="hint">Discord channel IDs.</p>
      <div class="form-group">
        <label for="logChannelId">Log Channel</label>
        <input id="logChannelId" type="text" inputmode="numeric" v-model="bot.logChannelId" />
      </div>
      <div class="form-group">
        <label for="evilLogChannelId">Evil Log Channel</label>
        <input id="evilLogChannelId" type="text" inputmode="numeric" v-model="bot.evilLogChannelId" />
      </div>
    </section>

    <section class="group">
      <h3>Roles</h3>
      <div class="form-group">
        <label for="adminRoleIds">Admin Roles</label>
        <input id="adminRoleIds" type="text" v-model="bot.adminRoleIds" />
        <small>Comma-separated role IDs allowed to run admin commands.</small>
      </div>
    </section>

    <section class="group">
      <h3>Identity</h3>
      <div class="form-group">
        <label for="evilId">Evil ID</label>
        <input id="evilId" type="text" inputmode="numeric" v-model="bot.evilId" />
      </div>
    </section>

    <section class="group">
      <h3>Data</h3>
      <div class="form-group">
        <label for="databaseName">Database</label>
        <input
          id="databaseName"
          list="db-options"
          v-model="bot.databaseName"
          placeholder="Shared (default)"
          autocomplete="off"
        />
        <datalist id="db-options">
          <option v-for="d in props.databases || []" :key="d" :value="d" />
        </datalist>
        <small>Which database this bot's data uses — leave blank for the shared default. Create new ones on the Database page. Applies after a restart.</small>
      </div>
    </section>

    <section class="group">
      <h3>Token</h3>

      <div class="form-group">
        <label for="token">{{ creating ? 'Bot token' : 'Set new token' }}</label>
        <input
          id="token"
          type="password"
          autocomplete="off"
          v-model="bot.token"
          :placeholder="creating ? 'Paste the bot token' : (bot.hasToken ? '•••••••• (stored — leave blank to keep)' : 'No token stored')"
        />
        <small v-if="creating">The Discord bot token used to log in.</small>
        <small v-else>Leave blank to keep the current token. Changing it needs a bot reload.</small>
      </div>

      <div v-if="!creating" class="form-group">
        <label>Current token</label>

        <!-- Revealed value -->
        <div v-if="revealedToken !== null" class="token-row">
          <input type="text" class="readonly" :value="revealedToken" readonly />
          <button type="button" class="mini-btn" @click="copyToken">{{ copied ? 'Copied' : 'Copy' }}</button>
          <button type="button" class="mini-btn" @click="hideToken">Hide</button>
        </div>

        <!-- Password prompt -->
        <div v-else-if="revealOpen" class="token-row">
          <input
            type="password"
            v-model="revealPassword"
            placeholder="Re-enter access password"
            autocomplete="off"
            @keydown.enter.prevent="submitReveal"
          />
          <button type="button" class="mini-btn primary" :disabled="revealBusy || !revealPassword" @click="submitReveal">
            {{ revealBusy ? '…' : 'Show' }}
          </button>
          <button type="button" class="mini-btn" @click="cancelReveal">Cancel</button>
        </div>

        <!-- Trigger -->
        <button
          v-else
          type="button"
          class="reveal-trigger"
          :disabled="!bot.hasToken"
          @click="revealOpen = true"
        >
          {{ bot.hasToken ? '🔒 View current token' : 'No token stored' }}
        </button>

        <p v-if="revealError" class="reveal-error">{{ revealError }}</p>
        <small>Viewing the token requires re-entering your access password.</small>
      </div>
    </section>

    <div class="form-actions">
      <button type="submit" class="save-btn" :disabled="saving">
        {{ saving ? 'Saving…' : creating ? 'Create bot' : 'Save Settings' }}
      </button>
    </div>
  </form>
</template>

<style scoped>
.settings-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.group {
  background-color: var(--surface);
  border: 1px solid var(--border-color);
  border-radius: 14px;
  padding: 1.5rem;
  box-shadow: 0 4px 16px var(--shadow);
}

.group h3 {
  margin-bottom: 1rem;
  font-size: 1.15rem;
  color: var(--foxfire);
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.group h3::before {
  content: '';
  width: 6px;
  height: 18px;
  border-radius: 3px;
  background: linear-gradient(180deg, var(--foxfire), var(--violet));
}

.hint {
  color: var(--muted-color);
  font-size: 0.85rem;
  margin: -0.5rem 0 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  margin-bottom: 1rem;
}

.form-group:last-child {
  margin-bottom: 0;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

label {
  font-weight: 500;
  color: var(--text-color);
}

input[type='text'],
input[type='number'],
input[type='password'],
textarea {
  width: 100%;
  padding: 0.6rem 0.7rem;
  border: 1px solid var(--input-border);
  border-radius: 8px;
  font: inherit;
  color: var(--text-color);
  background: var(--surface-2);
  transition: border-color 0.15s ease, box-shadow 0.15s ease;
}

input::placeholder {
  color: var(--muted-color);
}

input:focus,
textarea:focus {
  outline: none;
  border-color: var(--foxfire);
  box-shadow: 0 0 0 3px rgba(232, 70, 122, 0.16);
}

textarea {
  resize: vertical;
  min-height: 90px;
}

small {
  color: var(--muted-color);
  font-size: 0.8rem;
}

.check {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 500;
}

.check input {
  width: 18px;
  height: 18px;
}

.form-actions {
  position: sticky;
  bottom: 0;
}

.save-btn {
  background: linear-gradient(90deg, var(--foxfire), var(--foxfire-deep));
  color: #fff;
  border: none;
  border-radius: 10px;
  padding: 0.75rem 1.85rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 2px 8px var(--shadow);
  transition: transform 0.15s ease, filter 0.2s ease;
}

.save-btn:hover:not(:disabled) {
  filter: brightness(1.05);
  transform: translateY(-1px);
}

.save-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.readonly {
  opacity: 0.7;
  cursor: default;
  font-family: ui-monospace, Menlo, monospace;
}

.token-row {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  flex-wrap: wrap;
}
.token-row input {
  flex: 1 1 12rem;
  min-width: 0;
}

.mini-btn {
  border: 1px solid var(--border-color);
  background: var(--surface-2);
  color: var(--text-color);
  border-radius: 8px;
  padding: 0.5rem 0.9rem;
  font: inherit;
  cursor: pointer;
  white-space: nowrap;
  transition: all 0.15s ease;
}
.mini-btn:hover:not(:disabled) {
  border-color: var(--violet);
}
.mini-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
.mini-btn.primary {
  background: var(--foxfire);
  border-color: var(--foxfire);
  color: #fff;
}
.mini-btn.primary:hover:not(:disabled) {
  filter: brightness(1.05);
}

.reveal-trigger {
  align-self: flex-start;
  border: 1px dashed var(--input-border);
  background: var(--surface-2);
  color: var(--foxfire);
  border-radius: 8px;
  padding: 0.55rem 1rem;
  font: inherit;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.15s ease;
}
.reveal-trigger:hover:not(:disabled) {
  border-color: var(--foxfire);
  background: var(--surface-hover);
}
.reveal-trigger:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  color: var(--muted-color);
}

.reveal-error {
  color: var(--danger-color);
  font-size: 0.8rem;
  margin: 0.3rem 0 0;
}

@media (max-width: 768px) {
  .settings-form {
    gap: 1rem;
  }
  .group {
    padding: 1.1rem;
  }
  /* 16px keeps iOS from zooming the viewport when a field is focused. */
  input[type='text'],
  input[type='number'],
  input[type='password'],
  textarea {
    font-size: 16px;
  }
  /* Keep the save button reachable above the content as it scrolls. */
  .form-actions {
    background: var(--surface-2);
    padding: 0.75rem 0 calc(0.75rem + env(safe-area-inset-bottom));
    border-top: 1px solid var(--border-color);
  }
  .save-btn {
    width: 100%;
    min-height: 48px;
  }
}
</style>
