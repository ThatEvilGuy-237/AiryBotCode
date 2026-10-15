<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { Alert, Badge, Button, Collapsible, ConfirmDialog, EmptyState, Field, Icon, Modal, RadioGroup, StatusDot, TextField, Toggle, useToast } from '@airy/ui'
import ChannelPicker from '@/components/ChannelPicker.vue'
import { api, type ChannelWebhook, type DiscordEntity } from '@/lib/api'

const props = defineProps<{ botId: string; servers: DiscordEntity[] | null }>()
const toast = useToast()

const hooks = ref<ChannelWebhook[] | null>(null)
const error = ref<string | null>(null)
const open = ref(false)
const editing = ref<ChannelWebhook | null>(null)
const removing = ref<ChannelWebhook | null>(null)
const saving = ref(false)
const saveError = ref<string | null>(null)
const draft = ref({ channelId: '', name: '', webhookUrl: '', secret: '', mode: 'sync' as 'sync' | 'async', enabled: true })

const names = computed(() => new Map((props.servers ?? []).map(s => [s.id, s.name])))
const nameOf = (id: string) => names.value.get(id) ?? `channel ${id}`
const modes = [
  { label: 'Wait for the answer', value: 'sync', description: 'The webhook answers and Airy posts the answer.' },
  { label: 'Send and forget', value: 'async', description: 'Airy hands the message over and moves on.' },
]
const validUrl = computed(() => /^https?:\/\/\S+$/.test(draft.value.webhookUrl.trim()))

async function load() {
  error.value = null
  hooks.value = null
  try {
    hooks.value = await api.webhooks(props.botId)
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'The webhooks could not be loaded.'
  }
}

watch(() => props.botId, load, { immediate: true })

function start(hook?: ChannelWebhook) {
  editing.value = hook ?? null
  draft.value = hook
    ? { channelId: hook.channelId, name: hook.name, webhookUrl: hook.webhookUrl, secret: '', mode: hook.mode, enabled: hook.enabled }
    : { channelId: '', name: '', webhookUrl: '', secret: '', mode: 'sync', enabled: true }
  saveError.value = null
  open.value = true
}

async function save() {
  saving.value = true
  saveError.value = null
  const body = { ...draft.value, webhookUrl: draft.value.webhookUrl.trim(), secret: draft.value.secret.trim() || null }
  try {
    if (editing.value) await api.saveWebhook(props.botId, { ...body, id: editing.value.id })
    else await api.addWebhook(props.botId, body)
    open.value = false
    toast.success(editing.value ? 'Webhook saved' : `${nameOf(body.channelId)} now goes to the webhook`)
    await load()
  } catch (e) {
    saveError.value = e instanceof Error ? e.message : 'Saving failed.'
  } finally {
    saving.value = false
  }
}

async function remove() {
  const hook = removing.value
  removing.value = null
  if (!hook) return
  try {
    await api.dropWebhook(props.botId, hook.id)
    toast.success(`Webhook ${hook.name} removed`)
    await load()
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Removing failed.')
  }
}
</script>

<template>
  <Collapsible :title="`Webhooks${hooks ? ` (${hooks.length})` : ''}`" :default-open="false">
    <div class="hooks">
      <p class="lead">Send a channel's messages to an outside service instead of Airy's agent.</p>
      <Alert v-if="error" variant="danger" :title="error" dense>
        <template #actions><Button size="sm" variant="outline" @click="load">Try again</Button></template>
      </Alert>
      <EmptyState v-else-if="hooks && !hooks.length" title="No webhooks" sub="Most channels talk to Airy; a webhook is for an outside service." />
      <ul v-else-if="hooks" class="rows">
        <li v-for="h in hooks" :key="h.id" class="row">
          <StatusDot :variant="h.enabled ? 'ok' : 'off'" />
          <div class="names">
            <span class="name">{{ h.name }}</span>
            <span class="where">{{ nameOf(h.channelId) }}</span>
          </div>
          <span class="url" :title="h.webhookUrl">{{ h.webhookUrl }}</span>
          <Badge tone="neutral" outline>{{ h.mode === 'async' ? 'Send and forget' : 'Wait for answer' }}</Badge>
          <div class="end">
            <Button variant="ghost" size="sm" :aria-label="`Change ${h.name}`" @click="start(h)"><Icon name="pencil" :size="16" /></Button>
            <Button variant="ghost" size="sm" :aria-label="`Remove ${h.name}`" @click="removing = h"><Icon name="trash-2" :size="16" /></Button>
          </div>
        </li>
      </ul>
      <div><Button variant="outline" @click="start()"><Icon name="plus" :size="16" />Add a webhook</Button></div>
    </div>

    <Modal :open="open" :title="editing ? 'Change webhook' : 'Add a webhook'" size="md" @close="open = false">
      <form class="form" @submit.prevent="save">
        <Field label="Channel">
          <ChannelPicker :model-value="draft.channelId || null" :channels="servers ?? []" @update:model-value="v => (draft.channelId = v ?? '')" />
        </Field>
        <Field label="Name">
          <TextField v-model="draft.name" autocomplete="off" />
        </Field>
        <Field label="Webhook address" :error="draft.webhookUrl && !validUrl ? 'An http or https address' : undefined">
          <TextField v-model="draft.webhookUrl" type="url" autocomplete="off" />
        </Field>
        <Field label="Signing secret" :hint="editing?.hasSecret ? 'Leave empty to keep the stored secret' : 'Optional'">
          <TextField v-model="draft.secret" type="password" autocomplete="off" />
        </Field>
        <Field label="How Airy sends">
          <RadioGroup v-model="draft.mode" :options="modes" />
        </Field>
        <div class="toggle-row">
          <Toggle v-model="draft.enabled" labelledby="hook-on" />
          <span id="hook-on">On</span>
        </div>
        <Alert v-if="saveError" variant="danger" :title="saveError" dense />
      </form>
      <template #footer>
        <Button variant="ghost" @click="open = false">Cancel</Button>
        <Button :loading="saving" :disabled="!draft.channelId || !validUrl" @click="save">Save</Button>
      </template>
    </Modal>
    <ConfirmDialog
      :open="removing !== null"
      :title="`Remove ${removing?.name ?? ''}?`"
      message="Messages in that channel stop going to the webhook."
      confirm-text="Remove"
      danger
      @confirm="remove"
      @cancel="removing = null"
    />
  </Collapsible>
</template>

<style scoped>
.hooks {
  display: grid;
  gap: var(--space-3);
}
.lead {
  margin: 0;
  color: var(--color-text-dim);
}
.rows {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
}
.row {
  display: grid;
  grid-template-columns: 8px minmax(0, 1fr) minmax(0, 1.4fr) auto auto;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-2) 0;
  border-bottom: 1px solid var(--color-border-soft);
}
.row:last-child {
  border-bottom: 0;
}
.names {
  display: grid;
  min-width: 0;
}
.name {
  font-weight: 600;
  color: var(--color-fg);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.where {
  font-size: var(--font-size-sm);
  color: var(--color-muted);
}
.url {
  font-family: var(--font-mono);
  font-size: var(--font-size-xs);
  color: var(--color-muted);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.end {
  display: flex;
  gap: var(--space-1);
}
.form {
  display: grid;
  gap: var(--space-4);
}
.toggle-row {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  color: var(--color-fg);
}
@media (max-width: 860px) {
  .row {
    grid-template-columns: 8px minmax(0, 1fr) auto;
  }
  .url,
  .row > :nth-child(4) {
    grid-column: 2 / -1;
  }
}
</style>
