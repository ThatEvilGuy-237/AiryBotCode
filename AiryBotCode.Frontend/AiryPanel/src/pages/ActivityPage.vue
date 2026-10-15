<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { Alert, Badge, Button, Card, EmptyState, PageHeader, Skeleton, Stat, StatStrip, Table, TableColumn } from '@airy/ui'
import { api, type CountingStatus, type LevelUser } from '@/lib/api'
import { botId } from '@/lib/bots'
import { ago } from '@/lib/time'

const counting = ref<CountingStatus[] | null>(null)
const countingError = ref<string | null>(null)
const levels = ref<LevelUser[]>([])
const total = ref(0)
const levelsLoaded = ref(false)
const levelsError = ref<string | null>(null)
const more = ref(false)

const record = computed(() => Math.max(0, ...(counting.value ?? []).map(c => c.highScore)))
const bosses = computed(() => (counting.value ?? []).filter(c => c.bossActive).length)
const rows = computed(() => levels.value.map(u => ({ ...u, name: u.userName ?? u.userId, last: ago(u.lastXpAt) })))

async function load(id: string) {
  counting.value = null
  countingError.value = null
  levels.value = []
  total.value = 0
  levelsLoaded.value = false
  levelsError.value = null
  const [c, l] = await Promise.allSettled([api.countingFull(id), api.levels(id, 0)])
  if (botId.value !== id) return
  if (c.status === 'fulfilled') counting.value = c.value
  else countingError.value = c.reason instanceof Error ? c.reason.message : 'The counting game could not be loaded.'
  if (l.status === 'fulfilled') {
    levels.value = l.value.users
    total.value = l.value.total
  } else levelsError.value = l.reason instanceof Error ? l.reason.message : 'The level board could not be loaded.'
  levelsLoaded.value = true
}

async function loadMore() {
  if (!botId.value) return
  more.value = true
  try {
    const page = await api.levels(botId.value, levels.value.length)
    levels.value = [...levels.value, ...page.users]
    total.value = page.total
  } catch (e) {
    levelsError.value = e instanceof Error ? e.message : 'More could not be loaded.'
  } finally {
    more.value = false
  }
}

watch(botId, id => { if (id) load(id) }, { immediate: true })
</script>

<template>
  <div class="page">
    <PageHeader title="Activity" subtitle="// the counting game and the level board" />

    <Card>
      <template #head><h2>Counting</h2></template>
      <Alert v-if="countingError" variant="danger" :title="countingError" dense>
        <template #actions><Button size="sm" variant="outline" @click="botId && load(botId)">Try again</Button></template>
      </Alert>
      <Skeleton v-else-if="counting === null" variant="block" height="120px" />
      <EmptyState v-else-if="!counting.length" title="No counting yet" sub="Turn on Counting in Commands and pick its channel." />
      <template v-else>
        <StatStrip>
          <Stat label="Channels" :value="String(counting.length)" />
          <Stat label="Record" :value="String(record)" tone="accent" />
          <Stat label="Bosses out" :value="String(bosses)" :tone="bosses ? 'warn' : 'default'" />
        </StatStrip>
        <ul class="rows">
          <li v-for="c in counting" :key="c.channelId" class="row">
            <div class="names">
              <span class="name">{{ c.channelName ?? `channel ${c.channelId}` }}</span>
              <span class="sub">{{ c.lastUserName ? `last: ${c.lastUserName}` : 'nobody counted yet' }}{{ c.updatedAt ? ` · ${ago(c.updatedAt)}` : '' }}</span>
            </div>
            <span class="num"><span class="label">now</span>{{ c.currentCount }}</span>
            <span class="num"><span class="label">record</span>{{ c.highScore }}</span>
            <Badge v-if="c.bossActive" tone="warn">Boss out</Badge>
          </li>
        </ul>
      </template>
    </Card>

    <Card>
      <template #head>
        <div class="head">
          <h2>Level board</h2>
          <span v-if="total" class="count">{{ total }} members</span>
        </div>
      </template>
      <Alert v-if="levelsError" variant="danger" :title="levelsError" dense />
      <Table
        v-else
        :data="rows"
        row-key="userId"
        label="Level board"
        density="compact"
        :loading="!levelsLoaded"
        :skeleton-rows="5"
        empty="Nobody has XP yet"
        empty-hint="Turn on Leveling in Commands; members earn XP by talking"
      >
        <TableColumn field="rank" header="#" type="num" width="3rem" />
        <TableColumn field="name" header="Member" type="prose" />
        <TableColumn field="level" header="Level" type="num" />
        <TableColumn field="xp" header="XP" type="num" />
        <TableColumn field="last" header="Last XP" type="prose" />
      </Table>
      <div v-if="levels.length < total" class="more">
        <Button variant="outline" :loading="more" @click="loadMore">Show {{ Math.min(25, total - levels.length) }} more</Button>
      </div>
    </Card>
  </div>
</template>

<style scoped>
.page {
  display: grid;
  gap: var(--space-5);
}
h2 {
  margin: 0;
  font-size: var(--font-size-lg);
  color: var(--color-fg);
}
.head {
  display: flex;
  align-items: baseline;
  gap: var(--space-2);
}
.count {
  color: var(--color-muted);
  font-size: var(--font-size-sm);
}
.rows {
  list-style: none;
  margin: var(--space-3) 0 0;
  padding: 0;
  display: grid;
}
.row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 6rem 6rem auto;
  align-items: center;
  gap: var(--space-3);
  min-height: 52px;
  border-top: 1px solid var(--color-border-soft);
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
.sub {
  font-size: var(--font-size-sm);
  color: var(--color-muted);
}
.num {
  display: grid;
  justify-items: end;
  font-family: var(--font-mono);
  color: var(--color-fg);
}
.label {
  font-family: var(--font-family);
  font-size: var(--font-size-xs);
  color: var(--color-muted);
}
.more {
  display: flex;
  justify-content: center;
  margin-top: var(--space-3);
}
@media (max-width: 520px) {
  .row {
    grid-template-columns: minmax(0, 1fr) 4rem 4rem;
  }
  .row > :nth-child(4) {
    grid-column: 1 / -1;
    justify-self: start;
    margin-bottom: var(--space-2);
  }
}
</style>
