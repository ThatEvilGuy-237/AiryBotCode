<script setup lang="ts">
// Migrated to @hive/ui (Item E — full panel on the shared design system). Logic unchanged.
import { ref, watch, onMounted } from 'vue'
import { PageHeader, Card, Button, Badge, TextField, Toggle, ToolRow } from '@hive/ui'
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
      // Adding: the Channel ID field accepts several ids (comma / space / newline
      // separated) so one webhook can be linked to many channels at once — each
      // becomes its own link sharing the name, URL, secret and mode.
      const channelIds = String(d.channelId).split(/[\s,]+/).map(s => s.trim()).filter(Boolean)
      if (!channelIds.length) { error.value = 'Enter at least one channel id.'; saving.value = false; return }
      for (const channelId of channelIds) {
        await api.createChannelWebhook(currentBotId.value, { ...d, channelId })
      }
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
    <PageHeader
      title="Webhooks"
      :subtitle="currentBot
        ? `Link ${currentBot.botName}'s channels to webhooks (e.g. a Hive trigger). Messages in a linked channel are forwarded there.`
        : 'Pick a bot from the menu to manage its channel links.'"
    >
      <template #actions>
        <Button v-if="currentBotId" @click="add">+ Add link</Button>
      </template>
    </PageHeader>

    <p v-if="error" class="err">{{ error }}</p>

    <!-- Editor -->
    <Card v-if="draft">
      <template #head><h3>{{ draft.id === 0 ? 'New link' : 'Edit link' }}</h3></template>

      <div class="grid2">
        <TextField v-model="draft.name" label="Name" placeholder="e.g. Support flow" />
        <TextField
          v-model="draft.channelId"
          :label="draft.id === 0 ? 'Channel ID(s)' : 'Channel ID'"
          :placeholder="draft.id === 0 ? 'Discord channel id(s) — comma or space separated' : 'Discord channel id'"
        />
      </div>
      <TextField v-model="draft.webhookUrl" label="Webhook URL" placeholder="https://…/hooks/{triggerId}" />

      <div class="grid2">
        <label class="fld">
          <span class="lbl">Secret <span class="hint">(HMAC; {{ draft.id !== 0 && draft.hasSecret ? 'leave blank to keep' : 'optional' }})</span></span>
          <input class="in" v-model="draft.secret" type="password" autocomplete="off" placeholder="whsec_…" />
        </label>
        <label class="fld">
          <span class="lbl">Mode</span>
          <select class="in" v-model="draft.mode"><option value="sync">sync</option><option value="async">async</option></select>
        </label>
      </div>

      <label class="check"><Toggle v-model="draft.enabled" /> <span>Enabled</span></label>

      <template #foot>
        <Button :disabled="saving || !draft.channelId || !draft.webhookUrl" @click="save">{{ saving ? 'Saving…' : 'Save' }}</Button>
        <Button variant="ghost" @click="cancel">Cancel</Button>
      </template>
    </Card>

    <p v-if="!currentBotId" class="muted">No bot selected.</p>
    <p v-else-if="loading" class="muted">Loading…</p>
    <p v-else-if="!links.length && !draft" class="muted">No links yet — use <strong>+ Add link</strong>.</p>

    <Card v-else-if="links.length">
      <template #head>
        <h3>Links</h3>
        <div style="flex: 1" />
        <Badge variant="inactive">{{ links.length }}</Badge>
      </template>
      <ToolRow v-for="l in links" :key="l.id" :name="l.name" :description="l.webhookUrl" :dim="!l.enabled">
        <template #actions>
          <Badge variant="inactive">#{{ l.channelId }}</Badge>
          <Badge variant="inactive">{{ l.mode }}</Badge>
          <Badge v-if="l.hasSecret" variant="inactive">signed</Badge>
          <Badge :variant="l.enabled ? 'active' : 'inactive'">{{ l.enabled ? 'enabled' : 'disabled' }}</Badge>
          <Button variant="ghost" @click="edit(l)">Edit</Button>
          <Button variant="ghost" @click="remove(l)">Delete</Button>
        </template>
      </ToolRow>
    </Card>
  </div>
</template>

<style scoped>
.page { padding: 2rem; display: flex; flex-direction: column; gap: 1rem; max-width: 900px; }
.muted { color: var(--color-muted); }
.err { color: var(--color-danger); background: var(--color-surface-mute); border: var(--border); border-radius: var(--radius); padding: .7rem 1rem; }
.grid2 { display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-3) var(--space-4); margin-bottom: var(--space-3); }
.fld { display: flex; flex-direction: column; gap: var(--space-2); }
.lbl { font-size: var(--font-size-sm); font-weight: 600; color: var(--color-text-dim); }
.hint { font-weight: 400; color: var(--color-muted); }
.in { padding: var(--space-2) var(--space-3); border: var(--border); border-radius: var(--r-2); font: inherit; font-size: 16px;
  background: var(--color-surface-mute); color: var(--color-fg); }
.in:focus { outline: none; border-color: var(--color-accent-edge); }
.check { display: flex; flex-direction: row; align-items: center; gap: var(--space-2); color: var(--color-fg); }
@media (max-width: 768px) { .page { padding: 1rem; } .grid2 { grid-template-columns: 1fr; } }
</style>
