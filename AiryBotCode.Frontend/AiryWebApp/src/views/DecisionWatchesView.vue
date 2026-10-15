<script setup lang="ts">
import { computed, ref, watch, onMounted } from 'vue'
import { PageHeader, Card, Button, Badge, TextField, Toggle, Select } from '@hive/ui'
import { api, ApiError, type DecisionWatch } from '../lib/api'
import { useBots } from '../lib/bots'

const { currentBot, currentBotId, loadBots } = useBots()

interface ActionRow { answer: string; kind: string; emoji: string }
interface Draft { id: number; name: string; channelId: string; listenerUrl: string; secret: string; hasSecret: boolean; questionKey: string; rows: ActionRow[]; unsureKind: string; unsureEmoji: string; alertChannelId: string; enabled: boolean }

const KINDS = [
  { label: 'Do nothing', value: 'ignore' },
  { label: 'Answer in the channel', value: 'answer' },
  { label: 'Alert the mods', value: 'alert' },
  { label: 'React with an emoji', value: 'react' },
]
const KIND_WORD: Record<string, string> = { ignore: 'nothing', answer: 'answers', alert: 'alerts the mods', react: 'reacts' }

const watches = ref<DecisionWatch[]>([])
const loading = ref(true)
const error = ref('')
const draft = ref<Draft | null>(null)
const saving = ref(false)
const tried = ref(false)

function split(action: string): { kind: string; emoji: string } {
  return action.startsWith('react:') ? { kind: 'react', emoji: action.slice(6) } : { kind: action || 'ignore', emoji: '' }
}

function join(kind: string, emoji: string): string {
  return kind === 'react' ? `react:${emoji.trim()}` : kind
}

function describe(action: string): string {
  const { kind, emoji } = split(action)
  return kind === 'react' ? `reacts ${emoji}` : KIND_WORD[kind] ?? kind
}

function blank(): Draft {
  return { id: 0, name: '', channelId: '', listenerUrl: '', secret: '', hasSecret: false, questionKey: '', rows: [{ answer: 'yes', kind: 'answer', emoji: '' }, { answer: 'no', kind: 'ignore', emoji: '' }], unsureKind: 'ignore', unsureEmoji: '', alertChannelId: '', enabled: true }
}

function toDraft(w: DecisionWatch): Draft {
  const unsure = split(w.unsureAction)
  return {
    id: w.id, name: w.name, channelId: w.channelId, listenerUrl: w.listenerUrl, secret: '', hasSecret: w.hasSecret,
    questionKey: w.questionKey ?? '',
    rows: Object.entries(w.actions).map(([answer, action]) => ({ answer, ...split(action) })),
    unsureKind: unsure.kind, unsureEmoji: unsure.emoji, alertChannelId: w.alertChannelId ?? '', enabled: w.enabled,
  }
}

const problems = computed(() => {
  const d = draft.value
  if (!d) return {}
  return {
    channelId: /^\d{5,25}$/.test(d.channelId.trim()) ? '' : 'A Discord channel id (digits).',
    listenerUrl: /^https?:\/\/\S+$/.test(d.listenerUrl.trim()) ? '' : 'The Hive listener’s address (http or https).',
    secret: d.id === 0 && !d.secret.trim() ? 'The listener’s secret (whsec_…).' : '',
    rows: d.rows.some(r => !r.answer.trim()) ? 'Every row names an answer.' : d.rows.some(r => r.kind === 'react' && !r.emoji.trim()) ? 'A reaction needs its emoji.' : '',
    alertChannelId: d.alertChannelId.trim() && !/^\d{5,25}$/.test(d.alertChannelId.trim()) ? 'A channel id, or empty for the bot’s log channel.' : '',
  }
})
const valid = computed(() => Object.values(problems.value).every(p => !p))
const shown = (key: keyof typeof problems.value) => (tried.value ? problems.value[key] ?? '' : '')

async function load() {
  if (!currentBotId.value) { watches.value = []; loading.value = false; return }
  loading.value = true
  error.value = ''
  try {
    watches.value = await api.getDecisionWatches(currentBotId.value)
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Could not load the watches.'
  } finally {
    loading.value = false
  }
}

function add() {
  tried.value = false
  draft.value = blank()
}

function edit(w: DecisionWatch) {
  tried.value = false
  draft.value = toDraft(w)
}

async function save() {
  const d = draft.value
  if (!d || !currentBotId.value) return
  tried.value = true
  if (!valid.value) return
  saving.value = true
  error.value = ''
  const body: Partial<DecisionWatch> = {
    name: d.name.trim() || 'Decision watch',
    channelId: d.channelId.trim(),
    listenerUrl: d.listenerUrl.trim(),
    secret: d.secret.trim() || undefined,
    questionKey: d.questionKey.trim() || null,
    actions: Object.fromEntries(d.rows.map(r => [r.answer.trim(), join(r.kind, r.emoji)])),
    unsureAction: join(d.unsureKind, d.unsureEmoji),
    alertChannelId: d.alertChannelId.trim() || null,
    enabled: d.enabled,
  }
  try {
    if (d.id === 0) await api.createDecisionWatch(currentBotId.value, body)
    else await api.updateDecisionWatch(currentBotId.value, d.id, body)
    draft.value = null
    await load()
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Could not save the watch.'
  } finally {
    saving.value = false
  }
}

async function remove(w: DecisionWatch) {
  if (!currentBotId.value) return
  if (!window.confirm(`Delete the watch "${w.name}"? Messages in its channel stop going to the Hive listener.`)) return
  try {
    await api.deleteDecisionWatch(currentBotId.value, w.id)
    await load()
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Could not delete the watch.'
  }
}

watch(currentBotId, load)
onMounted(async () => { await loadBots(); await load() })
</script>

<template>
  <div class="page">
    <PageHeader
      title="Decision watches"
      :subtitle="currentBot
        ? `Each message in a watched channel goes to a Hive listener; ${currentBot.botName} acts on the answer.`
        : 'Pick a bot from the menu to manage its decision watches.'"
    >
      <template #actions>
        <Button v-if="currentBotId && !draft" @click="add">+ Add watch</Button>
      </template>
    </PageHeader>

    <p v-if="error" class="err" role="alert">{{ error }}</p>

    <Card v-if="draft">
      <template #head><h3>{{ draft.id === 0 ? 'New watch' : `Change ${draft.name}` }}</h3></template>
      <form class="form" @submit.prevent="save">
        <div class="grid2">
          <TextField v-model="draft.name" label="Name" placeholder="Talks to Airy" />
          <TextField v-model="draft.channelId" label="Channel id" placeholder="1236693895581143162" :error="shown('channelId')" />
        </div>
        <TextField v-model="draft.listenerUrl" label="Hive listener address" placeholder="https://thehive.thatevilguy.com/hive/api/spine/hooks/decisions/…" :error="shown('listenerUrl')" />
        <div class="grid2">
          <TextField v-model="draft.secret" type="password" :label="draft.id !== 0 && draft.hasSecret ? 'Listener secret (empty keeps it)' : 'Listener secret'" placeholder="whsec_…" :error="shown('secret')" />
          <TextField v-model="draft.questionKey" label="Question it acts on (empty: the first)" placeholder="forAiry" />
        </div>

        <div class="fld" role="group" aria-labelledby="watch-answers">
          <span id="watch-answers" class="lbl">When the answer is</span>
          <div class="rows">
            <div v-for="(r, i) in draft.rows" :key="i" class="row">
              <TextField v-model="r.answer" :aria-label="`Answer ${i + 1}`" placeholder="spam" />
              <Select v-model="r.kind" :options="KINDS" :aria-label="`What to do for ${r.answer || `answer ${i + 1}`}`" />
              <TextField v-if="r.kind === 'react'" v-model="r.emoji" :aria-label="`Emoji for ${r.answer || `answer ${i + 1}`}`" placeholder="👀" />
              <span v-else />
              <Button variant="ghost" :aria-label="`Remove ${r.answer || `answer ${i + 1}`}`" @click="draft.rows.splice(i, 1)">Remove</Button>
            </div>
            <Button class="add" variant="outline" @click="draft.rows.push({ answer: '', kind: 'ignore', emoji: '' })">+ Answer</Button>
          </div>
          <span v-if="shown('rows')" class="field-err">{{ shown('rows') }}</span>
        </div>

        <div class="fld" role="group" aria-labelledby="watch-unsure">
          <span id="watch-unsure" class="lbl">When the Hive is not sure</span>
          <div class="row unsure">
            <Select v-model="draft.unsureKind" :options="KINDS" aria-label="When the Hive is not sure" />
            <TextField v-if="draft.unsureKind === 'react'" v-model="draft.unsureEmoji" aria-label="Emoji when unsure" placeholder="🤔" />
          </div>
        </div>

        <TextField v-model="draft.alertChannelId" label="Alerts go to (empty: the bot’s log channel)" placeholder="channel id" :error="shown('alertChannelId')" />
        <p class="hint">A mention or a reply to the bot always gets an answer. Make the Hive listener keep answers only when people in this channel did not agree to have their messages kept.</p>
        <label class="check"><Toggle v-model="draft.enabled" aria-label="Watch on" /> <span>On</span></label>
        <div class="actions">
          <span v-if="tried && !valid" class="why">Fix the fields marked above first.</span>
          <Button variant="ghost" type="button" @click="draft = null">Cancel</Button>
          <Button type="submit" :loading="saving">{{ draft.id === 0 ? 'Add watch' : 'Save changes' }}</Button>
        </div>
      </form>
    </Card>

    <p v-if="!currentBotId" class="muted">No bot selected.</p>
    <p v-else-if="loading" class="muted">Loading…</p>
    <p v-else-if="!watches.length && !draft" class="muted">No watch yet. Make a listener in the Hive portal (Flows, Listeners), then use <strong>+ Add watch</strong>.</p>

    <Card v-else-if="watches.length">
      <template #head>
        <h3>Watches</h3>
        <div style="flex: 1" />
        <Badge variant="inactive">{{ watches.length }}</Badge>
      </template>
      <div class="watches">
        <div v-for="w in watches" :key="w.id" class="watch" :class="{ off: !w.enabled }">
          <div class="info">
            <div class="top">
              <span class="name">{{ w.name }}</span>
              <Badge :variant="w.enabled ? 'active' : 'inactive'">{{ w.enabled ? 'on' : 'off' }}</Badge>
            </div>
            <p class="line"><span class="mono">#{{ w.channelId }}</span> · acts on <span class="mono">{{ w.questionKey || 'the first question' }}</span></p>
            <p class="line">
              <template v-for="(action, answer, i) in w.actions" :key="answer"><template v-if="i">, </template><span class="mono">{{ answer }}</span> {{ describe(action) }}</template><template v-if="Object.keys(w.actions).length">; </template>unsure: {{ describe(w.unsureAction) }}
            </p>
            <code class="url">{{ w.listenerUrl }}</code>
          </div>
          <div class="watch-actions">
            <Button variant="outline" @click="edit(w)">Change</Button>
            <Button variant="ghost" @click="remove(w)">Delete</Button>
          </div>
        </div>
      </div>
    </Card>
  </div>
</template>

<style scoped>
.page { padding: 2rem; display: flex; flex-direction: column; gap: 1rem; max-width: 900px; }
.muted { color: var(--color-muted); }
.err { color: var(--color-danger); background: var(--color-surface-mute); border: var(--border); border-radius: var(--radius); padding: .7rem 1rem; }
.form { display: flex; flex-direction: column; gap: var(--space-3); }
.grid2 { display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-3) var(--space-4); }
.fld { display: flex; flex-direction: column; gap: var(--space-2); }
.lbl { font-size: var(--font-size-sm); font-weight: 600; color: var(--color-text-dim); }
.field-err { font-size: var(--font-size-xs); color: var(--color-danger); }
.rows { display: flex; flex-direction: column; gap: var(--space-2); }
.row { display: grid; grid-template-columns: minmax(0, 1fr) minmax(0, 1.2fr) minmax(0, 0.6fr) auto; gap: var(--space-2); align-items: start; }
.row.unsure { grid-template-columns: minmax(0, 1.2fr) minmax(0, 0.6fr); }
.add { align-self: flex-start; }
.hint { margin: 0; font-size: var(--font-size-sm); color: var(--color-muted); }
.check { display: flex; align-items: center; gap: var(--space-2); color: var(--color-fg); }
.actions { display: flex; align-items: center; justify-content: flex-end; gap: var(--space-2); flex-wrap: wrap; }
.why { margin-right: auto; font-size: var(--font-size-sm); color: var(--color-danger); }
.mono { font-family: var(--font-mono); font-size: var(--font-size-sm); }
.watches { display: flex; flex-direction: column; }
.watch { display: flex; align-items: center; gap: var(--space-4); padding: var(--space-3) 0; border-top: 1px solid var(--color-border-soft); }
.watch:first-child { border-top: none; }
.watch.off { opacity: .55; }
.info { min-width: 0; flex: 1; display: flex; flex-direction: column; gap: var(--space-1); }
.top { display: flex; align-items: center; gap: var(--space-2); }
.name { font-weight: 600; color: var(--color-fg); }
.line { margin: 0; font-size: var(--font-size-sm); color: var(--color-text-dim); }
.url { font-family: var(--font-mono); font-size: var(--font-size-xs); color: var(--color-muted); word-break: break-all; }
.watch-actions { display: flex; gap: var(--space-2); flex-shrink: 0; }

@media (max-width: 768px) {
  .page { padding: 1rem; }
  .grid2 { grid-template-columns: 1fr; }
  .row { grid-template-columns: minmax(0, 1fr) auto; }
  .row > :nth-child(2), .row > :nth-child(3) { grid-column: 1 / -1; }
  .watch { flex-direction: column; align-items: stretch; }
  .watch-actions { justify-content: flex-end; }
}
</style>
