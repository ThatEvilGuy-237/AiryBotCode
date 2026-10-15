<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { Alert, Button, Card, ConfirmDialog, EmptyState, Field, Icon, PageHeader, RadioGroup, Skeleton, StatusDot, Toggle, useToast } from '@airy/ui'
import ChannelPicker from '@/components/ChannelPicker.vue'
import HiveConnectionCard from '@/components/HiveConnectionCard.vue'
import WebhooksSection from '@/components/WebhooksSection.vue'
import { api, type AiryChannel, type AnswerMode, type DiscordEntity, type HiveCheck } from '@/lib/api'
import { botId } from '@/lib/bots'
import { channelName } from '@/lib/home'
import { ruleHelp, ruleLabels } from '@/lib/rules'
import { ago } from '@/lib/time'

const toast = useToast()

const channels = ref<AiryChannel[] | null>(null)
const servers = ref<DiscordEntity[] | null>(null)
const check = ref<HiveCheck | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)

const pick = ref<string | null>(null)
const rule = ref<AnswerMode>('mention')
const adding = ref(false)
const removing = ref<AiryChannel | null>(null)

const ruleOptions = (Object.keys(ruleLabels) as AnswerMode[]).map(m => ({ label: ruleLabels[m], value: m }))
const ruleTiles = (Object.keys(ruleLabels) as AnswerMode[]).map(m => ({ label: ruleLabels[m], value: m, description: ruleHelp[m] }))

const taken = computed(() => new Set((channels.value ?? []).map(c => c.channelId)))
const manyServers = computed(() => new Set((servers.value ?? []).map(s => s.guild)).size > 1)
const pickOptions = computed(() => (servers.value ?? [])
  .filter(s => !taken.value.has(s.id))
  .map(s => ({ label: manyServers.value ? `${s.name} · ${s.guild}` : s.name, value: s.id })))
const cannotDecide = computed(() => check.value !== null && !(check.value.canDecide && check.value.deciders.length > 0))

async function load(id: string) {
  loading.value = true
  error.value = null
  channels.value = null
  servers.value = null
  const [c, s] = await Promise.allSettled([api.channels(id), api.discordChannels(id)])
  if (botId.value !== id) return
  if (c.status === 'fulfilled') channels.value = c.value
  else error.value = c.reason instanceof Error ? c.reason.message : 'The channels could not be loaded.'
  servers.value = s.status === 'fulfilled' ? s.value : []
  loading.value = false
}

watch(botId, id => { if (id) load(id) }, { immediate: true })

async function add() {
  if (!botId.value || !pick.value) return
  adding.value = true
  try {
    const created = await api.addChannel(botId.value, pick.value, rule.value)
    channels.value = [created, ...(channels.value ?? [])]
    pick.value = null
    rule.value = 'mention'
    toast.success(`Airy talks in ${channelName(created)}`)
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Adding failed.')
  } finally {
    adding.value = false
  }
}

async function save(c: AiryChannel, change: Partial<Pick<AiryChannel, 'answerMode' | 'enabled' | 'listens'>>, undo = true) {
  if (!botId.value) return
  const before = { answerMode: c.answerMode, enabled: c.enabled, listens: c.listens }
  Object.assign(c, change)
  try {
    await api.saveChannel(botId.value, c)
    const said = change.listens !== undefined
      ? `Airy ${c.listens ? 'reads along in' : 'no longer reads along in'} ${channelName(c)}`
      : change.enabled !== undefined
        ? `Airy ${c.enabled ? 'answers in' : 'stays quiet in'} ${channelName(c)}`
        : `${channelName(c)}: ${ruleLabels[c.answerMode]}`
    if (undo) toast.action(said, { label: 'Undo', run: () => save(c, before, false) }, 'success')
    else toast.success(said)
  } catch (e) {
    Object.assign(c, before)
    toast.error(e instanceof Error ? e.message : 'Saving failed.')
  }
}

async function remove() {
  const c = removing.value
  removing.value = null
  if (!botId.value || !c) return
  try {
    await api.dropChannel(botId.value, c.id)
    channels.value = (channels.value ?? []).filter(x => x.id !== c.id)
    const id = botId.value
    toast.action(`Airy left ${channelName(c)}`, {
      label: 'Undo',
      run: async () => {
        const back = await api.addChannel(id, c.channelId, c.answerMode)
        if (!c.enabled || !c.listens) await api.saveChannel(id, { ...back, enabled: c.enabled, listens: c.listens })
        if (botId.value === id) channels.value = [{ ...back, enabled: c.enabled, listens: c.listens }, ...(channels.value ?? [])]
      },
    }, 'success')
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Removing failed.')
  }
}
</script>

<template>
  <div class="page">
    <PageHeader title="Channels" subtitle="// where Airy talks and when it answers" :loading="loading" />

    <template v-if="botId">
      <HiveConnectionCard :bot-id="botId" @changed="check = $event" />

      <Card class="talks">
        <template #head>
          <div class="head">
            <h2>Where Airy talks</h2>
            <span v-if="channels" class="count">{{ channels.length }}</span>
          </div>
        </template>

        <p class="listens">Airy reads along in these channels. When it answers, it knows what was said since it last spoke.</p>
        <form class="add" @submit.prevent="add">
          <div class="add-top">
            <Field label="Channel">
              <ChannelPicker v-model="pick" :channels="servers ?? []" :exclude="[...taken]" :placeholder="servers === null ? 'Reading the servers…' : pickOptions.length ? 'Find a channel' : servers.length ? 'Airy is in every channel already' : 'No channels to pick'" />
            </Field>
            <Button type="submit" :loading="adding" :disabled="!pick"><Icon name="plus" :size="16" />Add channel</Button>
          </div>
          <Field label="When Airy answers there">
            <RadioGroup v-model="rule" :options="ruleTiles" orientation="horizontal" class="tiles" />
          </Field>
        </form>
        <Alert v-if="servers && !servers.length && !loading" variant="warn" title="Airy cannot read this bot's servers, so there is nothing to pick." dense>
          <template #actions><Button size="sm" variant="outline" @click="$router.push('/settings')">Check the token</Button></template>
        </Alert>
        <Alert v-if="cannotDecide && channels?.some(c => c.answerMode === 'decide')" variant="warn" title="The Hive cannot decide yet, so the &quot;When it's worth it&quot; channels answer only when mentioned for now." dense />

        <Alert v-if="error" variant="danger" :title="error" dense>
          <template #actions><Button size="sm" variant="outline" @click="botId && load(botId)">Try again</Button></template>
        </Alert>
        <div v-else-if="channels === null" class="rows">
          <Skeleton v-for="i in 3" :key="i" variant="block" height="56px" />
        </div>
        <EmptyState v-else-if="!channels.length" title="Airy talks nowhere yet" sub="Pick a channel above and choose when Airy answers there." />
        <ul v-else class="rows">
          <li v-for="c in channels" :key="c.id" class="row" :class="{ off: !c.enabled && !c.listens }">
            <div class="who">
              <StatusDot :variant="!c.enabled && !c.listens ? 'off' : (c.channelName === null && c.namesKnown) || c.lastDecisionError ? 'warn' : 'ok'" />
              <div class="names">
                <span class="name" :title="channelName(c)">{{ c.channelName ?? 'Unknown channel' }}</span>
                <span v-if="c.channelName" class="guild">{{ c.guild }}</span>
                <span v-else-if="c.namesKnown" class="id">{{ c.channelId }} · gone from its server, or the bot cannot see it</span>
                <span v-else class="id quiet">{{ c.channelId }} · name unknown right now</span>
              </div>
            </div>
            <Field class="rule" :label="`When Airy answers in ${channelName(c)}`" hide-label>
              <RadioGroup
                :model-value="c.answerMode"
                :options="ruleOptions"
                orientation="horizontal"
                variant="plain"
                :disabled="!c.enabled"
                @update:model-value="v => save(c, { answerMode: v as AnswerMode })"
              />
            </Field>
            <div class="end">
              <span class="switch">
                <Toggle :model-value="c.listens" :labelledby="`listens-${c.id}`" @update:model-value="v => save(c, { listens: v })" />
                <span :id="`listens-${c.id}`">Listens<span class="sr-only"> in {{ channelName(c) }}</span></span>
              </span>
              <span class="switch">
                <Toggle :model-value="c.enabled" :labelledby="`answers-${c.id}`" @update:model-value="v => save(c, { enabled: v })" />
                <span :id="`answers-${c.id}`">Answers<span class="sr-only"> in {{ channelName(c) }}</span></span>
              </span>
              <Button variant="ghost" size="sm" :aria-label="`Remove ${channelName(c)}`" @click="removing = c"><Icon name="trash-2" :size="16" /></Button>
            </div>
            <p v-if="!c.enabled && c.listens" class="note">Reads along but stays quiet.</p>
            <p v-else-if="c.enabled && !c.listens" class="note">Answers without reading along: only the message it answers.</p>
            <p v-else-if="c.enabled && c.answerMode === 'decide' && cannotDecide" class="note attention">Answers only when mentioned until the Hive can decide.</p>
            <p v-else-if="c.lastDecisionError" class="note warn">Last check failed: {{ c.lastDecisionError }}</p>
            <p v-else-if="c.lastDecision" class="note">Last check: <span class="mono">{{ c.lastDecision }}</span> · {{ ago(c.lastDecisionAt) }}</p>
          </li>
        </ul>
      </Card>

      <WebhooksSection :bot-id="botId" :servers="servers" />
    </template>

    <ConfirmDialog
      :open="removing !== null"
      :title="`Remove ${removing ? channelName(removing) : ''}?`"
      message="Airy stops answering there. Its rule is forgotten; you can add the channel again."
      confirm-text="Remove"
      danger
      @confirm="remove"
      @cancel="removing = null"
    />
  </div>
</template>

<style scoped>
.page {
  display: grid;
  gap: var(--space-5);
}
.head {
  display: flex;
  align-items: baseline;
  gap: var(--space-2);
}
h2 {
  margin: 0;
  font-size: var(--font-size-lg);
  color: var(--color-fg);
}
.count {
  font-family: var(--font-mono);
  font-size: var(--font-size-sm);
  color: var(--color-muted);
}
.talks :deep([data-ui='card']) {
  display: grid;
}
.listens {
  margin: 0 0 var(--space-4);
  color: var(--color-text-dim);
}
.add {
  display: grid;
  gap: var(--space-4);
  padding-bottom: var(--space-4);
  margin-bottom: var(--space-3);
  border-bottom: var(--border);
}
.add-top {
  display: grid;
  grid-template-columns: minmax(0, 420px) auto;
  justify-content: start;
  align-items: end;
  gap: var(--space-3);
}
.tiles :deep(label) {
  flex: 1 1 200px;
}
.rows {
  list-style: none;
  margin: var(--space-3) 0 0;
  padding: 0;
  display: grid;
  gap: var(--space-2);
}
.row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto auto;
  align-items: center;
  gap: var(--space-2) var(--space-4);
  padding: var(--space-3) 0;
  border-bottom: 1px solid var(--color-border-soft);
}
.row:last-child {
  border-bottom: 0;
}
.who {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  min-width: 0;
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
.off .name {
  color: var(--color-text-dim);
  font-weight: 500;
}
.guild,
.id {
  font-size: var(--font-size-sm);
  color: var(--color-muted);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.id {
  font-family: var(--font-mono);
  font-size: var(--font-size-xs);
  color: var(--color-warn);
  white-space: normal;
}
.id.quiet {
  color: var(--color-muted);
}
.end {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: var(--space-2) var(--space-4);
}
.switch {
  display: inline-flex;
  align-items: center;
  gap: var(--space-2);
  color: var(--color-text-dim);
  font-size: var(--font-size-sm);
}
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  overflow: hidden;
  clip: rect(0 0 0 0);
  white-space: nowrap;
}
.note {
  grid-column: 1 / -1;
  margin: 0;
  padding-left: calc(8px + var(--space-3));
  font-size: var(--font-size-sm);
  color: var(--color-muted);
}
.note.attention {
  color: var(--color-warn);
}
.note.warn {
  color: var(--color-danger);
}
.mono {
  font-family: var(--font-mono);
  font-size: var(--font-size-xs);
}
@media (max-width: 760px) {
  .add-top {
    grid-template-columns: minmax(0, 1fr);
  }
}
@media (max-width: 1100px) {
  .row {
    grid-template-columns: minmax(0, 1fr) auto;
  }
  .rule {
    grid-column: 1 / -1;
    grid-row: 2;
    padding-left: calc(8px + var(--space-3));
  }
}
</style>
