<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { RouterLink } from 'vue-router'
import { Alert, Badge, Button, Card, EmptyState, LoadingBar, Skeleton, Stat, StatStrip, StatusDot } from '@airy/ui'
import { api, type AiryChannel, type CommandConfig, type CountingChannel, type HiveCheck, type HiveConnection, type Suggestion } from '@/lib/api'
import { bot, botId, botImage, bots, botsError, botsLoaded, loadBots } from '@/lib/bots'
import { channelName, concerns, grouped } from '@/lib/home'
import { ruleLabels } from '@/lib/rules'
import { ago } from '@/lib/time'

const loading = ref(false)
const failed = ref<string[]>([])
const channels = ref<AiryChannel[] | null>(null)
const hive = ref<HiveConnection | null>(null)
const check = ref<HiveCheck | null>(null)
const commands = ref<CommandConfig[] | null>(null)
const suggestions = ref<Suggestion[] | null>(null)
const counting = ref<CountingChannel[] | null>(null)

async function load(id: string) {
  loading.value = true
  failed.value = []
  channels.value = hive.value = check.value = commands.value = suggestions.value = counting.value = null
  const [c, h, k, s, n] = await Promise.allSettled([api.channels(id), api.hive(id), api.commands(id), api.suggestions(), api.counting(id)])
  if (botId.value !== id) return
  const read = <T,>(r: PromiseSettledResult<T>, what: string): T | null => {
    if (r.status === 'fulfilled') return r.value
    failed.value.push(`${what}: ${r.reason instanceof Error ? r.reason.message : 'failed'}`)
    return null
  }
  channels.value = read(c, 'Channels')
  hive.value = read(h, 'Hive connection')
  commands.value = read(k, 'Commands')
  suggestions.value = read(s, 'Suggestions')
  counting.value = read(n, 'Counting')
  check.value = null
  loading.value = false
  if (hive.value?.url && hive.value.hasKey) {
    try {
      const result = await api.hiveCheck(id)
      if (botId.value === id) check.value = result
    } catch (e) {
      failed.value.push(`Hive check: ${e instanceof Error ? e.message : 'failed'}`)
    }
  }
}

watch(botId, id => { if (id) load(id) }, { immediate: true })

const found = computed(() => grouped(concerns({ bot: bot.value, hive: hive.value, check: check.value, channels: channels.value })))
const commandsOn = computed(() => commands.value?.filter(c => c.enabled).length ?? 0)
const waiting = computed(() => suggestions.value?.filter(s => s.status === 'new').length ?? 0)
const record = computed(() => Math.max(0, ...(counting.value ?? []).map(c => c.highScore)))
const tone = (c: AiryChannel) => (!c.enabled ? 'off' : (c.channelName === null && c.namesKnown) || c.lastDecisionError ? 'warn' : 'ok')
</script>

<template>
  <div class="home">
    <Alert v-if="botsError" variant="danger" :title="botsError">
      <template #actions><Button size="sm" variant="outline" @click="loadBots(true)">Try again</Button></template>
    </Alert>
    <EmptyState v-else-if="botsLoaded && !bots.length" title="No bots yet" sub="Add the first bot with its Discord token in Settings.">
      <Button @click="$router.push('/settings')">Add a bot</Button>
    </EmptyState>
    <template v-else-if="bot">
      <header class="bot">
        <span class="avatar" aria-hidden="true">
          <img v-if="botImage" :src="botImage" alt="" />
          <span v-else>{{ bot.botName.charAt(0).toUpperCase() }}</span>
        </span>
        <div class="who">
          <h1>{{ bot.botName }}</h1>
          <span class="id">// {{ bot.botId }}</span>
        </div>
        <Badge :tone="bot.enabled ? 'ok' : 'muted'">{{ bot.enabled ? 'On' : 'Off' }}</Badge>
      </header>
      <LoadingBar v-if="loading" />

      <section v-if="found.length || failed.length" class="looks" aria-label="Needs a look">
        <Alert v-for="f in failed" :key="f" variant="danger" :title="f" dense>
          <template #actions><Button size="sm" variant="outline" @click="load(bot.botId)">Try again</Button></template>
        </Alert>
        <Alert v-for="g in found" :key="g.fix.to" :variant="g.tone" :title="g.items.length === 1 ? g.items[0] : `${g.items.length} things need a look`" dense>
          <ul v-if="g.items.length > 1" class="items">
            <li v-for="item in g.items" :key="item">{{ item }}</li>
          </ul>
          <template #actions><Button size="sm" variant="outline" @click="$router.push(g.fix.to)">{{ g.fix.label }}</Button></template>
        </Alert>
      </section>

      <Skeleton v-if="loading && commands === null" variant="block" height="92px" class="stats-wait" />
      <StatStrip v-else class="stats">
        <RouterLink to="/channels" class="stat-link"><Stat label="Channels" :value="channels ? String(channels.length) : '…'" sub="where Airy talks" /></RouterLink>
        <RouterLink to="/commands" class="stat-link"><Stat label="Commands on" :value="commands ? `${commandsOn} of ${commands.length}` : '…'" /></RouterLink>
        <RouterLink to="/suggestions" class="stat-link"><Stat label="Suggestions" :value="suggestions ? String(waiting) : '…'" sub="waiting" :tone="waiting ? 'accent' : 'default'" /></RouterLink>
        <RouterLink to="/activity" class="stat-link"><Stat label="Counting record" :value="counting ? String(record) : '…'" /></RouterLink>
      </StatStrip>

      <Card class="talks">
        <template #head>
          <div class="head">
            <h2>Where Airy talks</h2>
            <RouterLink to="/channels" class="more">Change</RouterLink>
          </div>
        </template>
        <div v-if="channels === null && loading" class="rows">
          <Skeleton v-for="i in 3" :key="i" variant="text" />
        </div>
        <EmptyState v-else-if="channels && !channels.length" title="Airy talks nowhere yet" sub="Pick a channel and when Airy should answer there.">
          <Button @click="$router.push('/channels')">Pick a channel</Button>
        </EmptyState>
        <ul v-else-if="channels" class="rows">
          <li v-for="c in channels" :key="c.id" class="row" :class="{ off: !c.enabled }">
            <StatusDot :variant="tone(c)" />
            <span class="name" :title="channelName(c)">{{ channelName(c) }}</span>
            <span class="guild">{{ c.guild ?? '' }}</span>
            <Badge tone="neutral" outline>{{ c.enabled ? ruleLabels[c.answerMode] : 'Paused' }}</Badge>
            <span class="last" :class="{ failed: c.lastDecisionError }">{{ c.lastDecision ? `${c.lastDecision} · ${ago(c.lastDecisionAt)}` : '' }}</span>
          </li>
        </ul>
      </Card>
    </template>
  </div>
</template>

<style scoped>
.home {
  display: grid;
  gap: var(--space-5);
}
.bot {
  display: grid;
  grid-template-columns: 56px minmax(0, 1fr) auto;
  align-items: center;
  gap: var(--space-4);
}
.who {
  min-width: 0;
  display: grid;
  gap: var(--space-1);
}
h1 {
  margin: 0;
  font-family: var(--font-display);
  font-size: var(--font-size-xl);
  color: var(--color-fg);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.id {
  font-family: var(--font-mono);
  font-size: var(--font-size-xs);
  color: var(--color-muted);
}
.avatar {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  overflow: hidden;
  display: grid;
  place-items: center;
  background: var(--color-surface-hi);
  color: var(--color-fg);
  font-size: var(--font-size-xl);
  font-weight: 700;
}
.avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.looks {
  display: grid;
  gap: var(--space-2);
}
.items {
  margin: 0;
  padding-left: var(--space-4);
  display: grid;
  gap: var(--space-1);
  color: var(--color-text-dim);
}
.stat-link {
  display: block;
  color: inherit;
  text-decoration: none;
  border-radius: var(--r-2);
}
.stat-link:hover :deep([data-ui='stat']) {
  border-color: var(--color-border-hi);
}
.stat-link:focus-visible {
  outline: 2px solid var(--color-accent);
  outline-offset: 2px;
}
.head {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  gap: var(--space-3);
  width: 100%;
}
h2 {
  margin: 0;
  font-size: var(--font-size-lg);
  color: var(--color-fg);
}
.more {
  display: inline-flex;
  align-items: center;
  min-height: 32px;
  padding: 0 var(--space-2);
  margin-right: calc(var(--space-2) * -1);
  border-radius: var(--r-2);
  color: var(--color-accent);
  font-size: var(--font-size-sm);
}
.more:focus-visible {
  outline: 2px solid var(--color-accent);
}
.rows {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  gap: var(--space-2);
}
.row {
  display: grid;
  grid-template-columns: 8px minmax(0, 1.3fr) minmax(0, 1fr) 184px 148px;
  align-items: center;
  gap: var(--space-3);
  min-height: 44px;
  padding: var(--space-2) 0;
  border-bottom: 1px solid var(--color-border-soft);
}
.row:last-child {
  border-bottom: 0;
}
.row > :nth-child(4) {
  justify-self: start;
}
.row.off .name {
  color: var(--color-text-dim);
  font-weight: 500;
}
.name {
  color: var(--color-fg);
  font-weight: 600;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.guild {
  color: var(--color-muted);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.last {
  justify-self: end;
  font-family: var(--font-mono);
  font-size: var(--font-size-xs);
  color: var(--color-muted);
  white-space: nowrap;
}
.last.failed {
  color: var(--color-danger);
}
@media (max-width: 860px) {
  .more {
    min-height: 44px;
  }
}
@media (max-width: 640px) {
  .row {
    grid-template-columns: 8px minmax(0, 1fr) auto;
    grid-template-areas: 'dot name rule' 'dot guild last';
    row-gap: 2px;
  }
  .row > :nth-child(1) { grid-area: dot; }
  .name { grid-area: name; }
  .guild { grid-area: guild; font-size: var(--font-size-sm); }
  .row > :nth-child(4) { grid-area: rule; }
  .last { grid-area: last; }
}
</style>
