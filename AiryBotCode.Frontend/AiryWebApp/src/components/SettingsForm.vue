<script setup lang="ts">
// Migrated to @hive/ui (Item E). Logic unchanged; sections are Cards, fields use
// TextField/Toggle/Button (native styled inputs for password + datalist).
import { ref, watch } from 'vue'
import { Card, TextField, Toggle, Button } from '@hive/ui'
import { api, ApiError } from '../lib/api'
import type { BotSetting } from '../types/botSetting'

const props = defineProps<{
  bot: BotSetting
  saving: boolean
  creating?: boolean
  databases?: string[]
}>()

const emit = defineEmits<{ (e: 'save'): void }>()

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
watch(() => props.bot.botId, () => {
  revealedToken.value = null
  revealOpen.value = false
  revealPassword.value = ''
  revealError.value = ''
  copied.value = false
})
</script>

<template>
  <form class="settings-form" @submit.prevent="emit('save')">
    <Card>
      <template #head><h3>General</h3></template>
      <div class="fields">
        <div>
          <TextField v-model="bot.botId" label="Bot ID" />
          <small class="hint">{{ creating ? 'The Discord application / bot user ID.' : 'Changing this re-creates the bot under the new ID.' }}</small>
        </div>
        <TextField v-model="bot.botName" label="Bot Name" />
        <label class="check"><Toggle v-model="bot.enabled" /> <span>Enabled</span></label>
      </div>
    </Card>

    <Card>
      <template #head><h3>Channels</h3></template>
      <div class="fields">
        <p class="hint">Discord channel IDs.</p>
        <TextField v-model="bot.logChannelId" label="Log Channel" />
        <TextField v-model="bot.evilLogChannelId" label="Evil Log Channel" />
      </div>
    </Card>

    <Card>
      <template #head><h3>Roles</h3></template>
      <div class="fields">
        <TextField v-model="bot.adminRoleIds" label="Admin Roles" />
        <small class="hint">Comma-separated role IDs allowed to run admin commands.</small>
      </div>
    </Card>

    <Card>
      <template #head><h3>Identity</h3></template>
      <TextField v-model="bot.evilId" label="Evil ID" />
    </Card>

    <Card>
      <template #head><h3>Data</h3></template>
      <div class="fields">
        <label class="fld">
          <span class="lbl">Database</span>
          <input class="in" list="db-options" v-model="bot.databaseName" placeholder="Shared (default)" autocomplete="off" />
          <datalist id="db-options"><option v-for="d in props.databases || []" :key="d" :value="d" /></datalist>
        </label>
        <small class="hint">Which database this bot's data uses — leave blank for the shared default. Applies after a restart.</small>
      </div>
    </Card>

    <Card>
      <template #head><h3>Token</h3></template>
      <div class="fields">
        <label class="fld">
          <span class="lbl">{{ creating ? 'Bot token' : 'Set new token' }}</span>
          <input class="in" type="password" autocomplete="off" v-model="bot.token"
            :placeholder="creating ? 'Paste the bot token' : (bot.hasToken ? '•••••••• (stored — leave blank to keep)' : 'No token stored')" />
        </label>
        <small class="hint">{{ creating ? 'The Discord bot token used to log in.' : 'Leave blank to keep the current token. Changing it needs a bot reload.' }}</small>

        <div v-if="!creating" class="token-block">
          <span class="lbl">Current token</span>
          <div v-if="revealedToken !== null" class="token-row">
            <input type="text" class="in mono" :value="revealedToken" readonly />
            <Button variant="outline" @click="copyToken">{{ copied ? 'Copied' : 'Copy' }}</Button>
            <Button variant="ghost" @click="hideToken">Hide</Button>
          </div>
          <div v-else-if="revealOpen" class="token-row">
            <input class="in" type="password" v-model="revealPassword" placeholder="Re-enter access password" autocomplete="off" @keydown.enter.prevent="submitReveal" />
            <Button :disabled="revealBusy || !revealPassword" @click="submitReveal">{{ revealBusy ? '…' : 'Show' }}</Button>
            <Button variant="ghost" @click="cancelReveal">Cancel</Button>
          </div>
          <Button v-else variant="outline" :disabled="!bot.hasToken" @click="revealOpen = true">{{ bot.hasToken ? '🔒 View current token' : 'No token stored' }}</Button>
          <p v-if="revealError" class="err">{{ revealError }}</p>
          <small class="hint">Viewing the token requires re-entering your access password.</small>
        </div>
      </div>
    </Card>

    <div class="form-actions">
      <Button type="submit" :disabled="saving">{{ saving ? 'Saving…' : creating ? 'Create bot' : 'Save Settings' }}</Button>
    </div>
  </form>
</template>

<style scoped>
.settings-form { display: flex; flex-direction: column; gap: var(--space-4); }
.fields { display: flex; flex-direction: column; gap: var(--space-3); }
.hint { color: var(--color-muted); font-size: var(--font-size-sm); display: block; margin-top: var(--space-1); }
.check { display: flex; align-items: center; gap: var(--space-2); color: var(--color-fg); }
.fld { display: flex; flex-direction: column; gap: var(--space-2); }
.lbl { font-size: var(--font-size-sm); font-weight: 600; color: var(--color-text-dim); }
.in { padding: var(--space-2) var(--space-3); border: var(--border); border-radius: var(--r-2); font: inherit; font-size: 16px;
  background: var(--color-surface-mute); color: var(--color-fg); width: 100%; }
.in:focus { outline: none; border-color: var(--color-accent-edge); }
.in.mono { font-family: var(--font-mono); }
.token-block { display: flex; flex-direction: column; gap: var(--space-2); margin-top: var(--space-2); }
.token-row { display: flex; gap: var(--space-2); align-items: center; flex-wrap: wrap; }
.token-row .in { flex: 1 1 12rem; min-width: 0; }
.err { color: var(--color-danger); font-size: var(--font-size-sm); margin: var(--space-1) 0 0; }
.form-actions { position: sticky; bottom: 0; }
@media (max-width: 768px) { .form-actions { background: var(--color-bg-2); padding: var(--space-3) 0; border-top: var(--border); } }
</style>
