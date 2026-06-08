<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { api, ApiError, type ChannelWebhook } from '../lib/api'
import { useBots } from '../lib/bots'

const { currentBot, currentBotId, loadBots } = useBots()

const links = ref<ChannelWebhook[]>([])
const loading = ref(true)
const error = ref('')

// null = not editing; a draft = adding (id 0) or editing an existing link.
const draft = ref<ChannelWebhook | null>(null)
const saving = ref(false)

function blank(): ChannelWebhook {
  return { id: 0, channelId: '', name: '', webhookUrl: '', hasSecret: false, secret: '', mode: 'sync', enabled: true }
}

async function load() {
  if (!currentBotId.value) { links.value = []; loading.value = false; return }
  loading.value = true
  error.value = ''
  try {
    links.value = await api.getChannelWebhooks(currentBotId.value)
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to load links.'
  } finally {
    loading.value = false
  }
}

function add() {
  draft.value = blank()
}
function edit(l: ChannelWebhook) {
  // Secret is write-only; start blank (leave blank to keep).
  draft.value = { ...l, secret: '' }
}
function cancel() {
  draft.value = null
}

async function save() {
  if (!draft.value || !currentBotId.value) return
  saving.value = true
  error.value = ''
  try {
    const d = draft.value
    if (d.id === 0) {
      await api.createChannelWebhook(currentBotId.value, d)
    } else {
      // Blank secret on edit → don't change it (send only if typed).
      const payload: Partial<ChannelWebhook> = { ...d }
      if (!d.secret) delete payload.secret
      await api.updateChannelWebhook(currentBotId.value, d.id, payload)
    }
    draft.value = null
    await load()
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to save.'
  } finally {
    saving.value = false
  }
}

async function remove(l: ChannelWebhook) {
  if (!currentBotId.value) return
  if (!window.confirm(`Delete the link for "${l.name}"?`)) return
  await api.deleteChannelWebhook(currentBotId.value, l.id)
  await load()
}

watch(currentBotId, load)
onMounted(async () => { await loadBots(); await load() })
</script>

<template>
  <div class="page">
    <header class="page-header">
      <div>
        <h1>Webhooks</h1>
        <p class="sub">
          <template v-if="currentBot">Link <strong>{{ currentBot.botName }}</strong>'s channels to webhooks (e.g. a Hive trigger). Messages in a linked channel are forwarded there.</template>
          <template v-else>Pick a bot from the menu to manage its channel links.</template>
        </p>
      </div>
      <button v-if="currentBotId" type="button" class="add-btn" @click="add">+ Add link</button>
    </header>

    <p v-if="error" class="error">{{ error }}</p>

    <!-- Editor -->
    <section v-if="draft" class="card editor">
      <h2>{{ draft.id === 0 ? 'New link' : 'Edit link' }}</h2>
      <div class="grid2">
        <label>Name<input v-model="draft.name" placeholder="e.g. Support flow" /></label>
        <label>Channel ID<input v-model="draft.channelId" inputmode="numeric" placeholder="Discord channel id" /></label>
      </div>
      <label>Webhook URL<input v-model="draft.webhookUrl" placeholder="https://…/hooks/{triggerId}" /></label>
      <div class="grid2">
        <label>Secret <small>(HMAC; {{ draft.id !== 0 && draft.hasSecret ? 'leave blank to keep' : 'optional' }})</small>
          <input v-model="draft.secret" type="password" autocomplete="off" placeholder="whsec_…" />
        </label>
        <label>Mode
          <select v-model="draft.mode"><option value="sync">sync</option><option value="async">async</option></select>
        </label>
      </div>
      <label class="check"><input type="checkbox" v-model="draft.enabled" /> <span>Enabled</span></label>
      <div class="actions">
        <button class="save-btn" :disabled="saving || !draft.channelId || !draft.webhookUrl" @click="save">{{ saving ? 'Saving…' : 'Save' }}</button>
        <button class="cancel-btn" @click="cancel">Cancel</button>
      </div>
    </section>

    <p v-if="!currentBotId" class="muted">No bot selected.</p>
    <p v-else-if="loading" class="muted">Loading…</p>
    <p v-else-if="!links.length && !draft" class="muted">No links yet — use <strong>+ Add link</strong>.</p>

    <div v-else class="list">
      <div v-for="l in links" :key="l.id" class="card link" :class="{ off: !l.enabled }">
        <div class="link-main">
          <h3>{{ l.name }}</h3>
          <code class="url">{{ l.webhookUrl }}</code>
          <div class="meta">
            <span>#{{ l.channelId }}</span>
            <span class="tag">{{ l.mode }}</span>
            <span v-if="l.hasSecret" class="tag">signed</span>
            <span class="tag" :class="l.enabled ? 'on' : 'offtag'">{{ l.enabled ? 'enabled' : 'disabled' }}</span>
          </div>
        </div>
        <div class="link-actions">
          <button class="mini" @click="edit(l)">Edit</button>
          <button class="mini danger" @click="remove(l)">Delete</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page { padding: 2rem; }
.page-header { display: flex; align-items: flex-start; justify-content: space-between; gap: 1rem; flex-wrap: wrap; margin-bottom: 1.5rem; }
h1 { margin: 0; font-size: 1.9rem; background: linear-gradient(90deg, var(--foxfire), var(--violet)); -webkit-background-clip: text; background-clip: text; color: transparent; }
.sub { margin: 0.35rem 0 0; color: var(--muted-color); max-width: 560px; }
.muted { color: var(--muted-color); }
.error { background: #fde8ef; border: 1px solid var(--border-color); color: var(--foxfire-deep); padding: 0.7rem 1rem; border-radius: 10px; margin-bottom: 1rem; }
.add-btn { background: linear-gradient(90deg, var(--foxfire), var(--foxfire-deep)); color: #fff; border: none; border-radius: 999px; padding: 0.55rem 1.2rem; font-weight: 600; cursor: pointer; white-space: nowrap; }

.card { background: var(--surface); border: 1px solid var(--border-color); border-radius: 14px; padding: 1.25rem; margin-bottom: 1rem; }
.editor h2 { margin: 0 0 1rem; font-size: 1.1rem; }
.editor label { display: flex; flex-direction: column; gap: 0.35rem; font-weight: 600; margin-bottom: 0.9rem; }
.editor small { font-weight: 400; color: var(--muted-color); }
.grid2 { display: grid; grid-template-columns: 1fr 1fr; gap: 0 1rem; }
.editor input, .editor select { padding: 0.55rem 0.7rem; border: 1px solid var(--input-border, #ecc0cf); border-radius: 8px; font: inherit; font-size: 16px; background: #fff; }
.check { flex-direction: row !important; align-items: center; gap: 0.5rem; }
.check input { width: 18px; height: 18px; }
.actions { display: flex; gap: 0.6rem; }
.save-btn { background: var(--foxfire); color: #fff; border: none; border-radius: 8px; padding: 0.6rem 1.3rem; font-weight: 600; cursor: pointer; }
.save-btn:disabled { opacity: 0.55; cursor: default; }
.cancel-btn { background: #fff; border: 1px solid var(--border-color); border-radius: 8px; padding: 0.6rem 1rem; cursor: pointer; }

.link { display: flex; align-items: center; justify-content: space-between; gap: 1rem; }
.link.off { opacity: 0.6; }
.link h3 { margin: 0 0 0.2rem; font-size: 1.05rem; }
.url { font-size: 0.78rem; color: var(--muted-color); word-break: break-all; }
.meta { display: flex; flex-wrap: wrap; gap: 0.4rem; margin-top: 0.4rem; color: var(--muted-color); font-size: 0.8rem; align-items: center; }
.tag { background: var(--surface-2); border: 1px solid var(--border-color); border-radius: 999px; padding: 0.05rem 0.5rem; font-size: 0.7rem; font-weight: 600; }
.tag.on { color: #1f9254; } .tag.offtag { color: var(--muted-color); }
.link-actions { display: flex; gap: 0.4rem; flex-shrink: 0; }
.mini { background: var(--surface-2); border: 1px solid var(--border-color); border-radius: 8px; padding: 0.4rem 0.8rem; cursor: pointer; font: inherit; }
.mini.danger { color: var(--danger-color); border-color: rgba(248,113,113,0.5); }

@media (max-width: 768px) {
  .page { padding: 1rem; }
  h1 { font-size: 1.5rem; }
  .grid2 { grid-template-columns: 1fr; }
  .link { flex-direction: column; align-items: stretch; }
}
</style>
