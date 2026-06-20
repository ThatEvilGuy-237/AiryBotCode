<script setup lang="ts">
// Read-only feature status for the selected bot: counting-game progress per
// channel + the XP leaderboard. Purely surfaces what the running bot has written
// to the DB (CountingState / LevelUser) — nothing here mutates anything.
import { ref, watch, onMounted } from 'vue'
import { PageHeader, Card, Badge, Button } from '@hive/ui'
import { api, ApiError, type CountingChannelStatus, type LeaderboardEntry } from '../lib/api'
import { useBots } from '../lib/bots'

const { currentBot, currentBotId, loadBots } = useBots()

const counting = ref<CountingChannelStatus[]>([])
const board = ref<LeaderboardEntry[]>([])
const boardTotal = ref(0)
const loading = ref(true)
const error = ref('')

const nf = new Intl.NumberFormat()

async function load() {
  if (!currentBotId.value) {
    counting.value = []
    board.value = []
    boardTotal.value = 0
    loading.value = false
    return
  }
  loading.value = true
  error.value = ''
  try {
    // Independent reads — surface whichever succeeds even if the other is empty.
    const [c, b] = await Promise.all([
      api.getCountingStatus(currentBotId.value),
      api.getLevelingBoard(currentBotId.value, 0, 25),
    ])
    counting.value = c
    board.value = b.users
    boardTotal.value = b.total
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to load feature status.'
  } finally {
    loading.value = false
  }
}

function channelLabel(c: CountingChannelStatus): string {
  return c.channelName ? `#${c.channelName}` : `channel ${c.channelId}`
}

watch(currentBotId, load)
onMounted(async () => {
  await loadBots()
  await load()
})
</script>

<template>
  <div class="page">
    <PageHeader
      title="Status"
      :subtitle="currentBot
        ? `Live feature status for ${currentBot.botName} — counting games and the XP leaderboard. Read-only.`
        : 'Pick a bot from the menu to see its feature status.'"
    >
      <template #actions>
        <Button v-if="currentBotId" variant="outline" :disabled="loading" @click="load">
          {{ loading ? 'Refreshing…' : '⟳ Refresh' }}
        </Button>
      </template>
    </PageHeader>

    <p v-if="error" class="banner err">{{ error }}</p>

    <p v-if="!currentBotId" class="muted">No bot selected — pick one in the bot menu (top-left).</p>
    <p v-else-if="loading" class="muted">Loading feature status…</p>

    <template v-else>
      <!-- Counting game -->
      <section class="section">
        <div class="section-head">
          <h2 class="section-title">Counting game</h2>
          <span class="mono muted">{{ counting.length }} channel{{ counting.length === 1 ? '' : 's' }}</span>
        </div>
        <p v-if="!counting.length" class="muted">No counting games running yet.</p>
        <div v-else class="grid">
          <Card v-for="c in counting" :key="c.channelId" class="stat-card">
            <template #head>
              <h3 class="name">{{ channelLabel(c) }}</h3>
              <div style="flex: 1" />
              <Badge v-if="c.bossActive" variant="active">boss active</Badge>
            </template>
            <div class="stats">
              <div class="stat">
                <span class="stat-num">{{ nf.format(c.currentCount) }}</span>
                <span class="stat-lbl">current</span>
              </div>
              <div class="stat">
                <span class="stat-num">{{ nf.format(c.highScore) }}</span>
                <span class="stat-lbl">high score</span>
              </div>
            </div>
          </Card>
        </div>
      </section>

      <!-- Leaderboard -->
      <section class="section">
        <div class="section-head">
          <h2 class="section-title">XP leaderboard</h2>
          <span class="mono muted">{{ nf.format(boardTotal) }} ranked</span>
        </div>
        <p v-if="!board.length" class="muted">No one has earned XP yet.</p>
        <Card v-else class="board">
          <ol class="rows">
            <li v-for="u in board" :key="u.userId" class="row">
              <span class="rank" :class="{ top: u.rank <= 3 }">#{{ u.rank }}</span>
              <span class="uname">{{ u.userName || u.userId }}</span>
              <Badge variant="inactive">lvl {{ u.level }}</Badge>
              <span class="xp mono">{{ nf.format(u.xp) }} XP</span>
            </li>
          </ol>
          <p v-if="boardTotal > board.length" class="more muted mono">
            showing top {{ board.length }} of {{ nf.format(boardTotal) }}
          </p>
        </Card>
      </section>
    </template>
  </div>
</template>

<style scoped>
.page { padding: 2rem; display: flex; flex-direction: column; gap: 1.5rem; }
.muted { color: var(--color-muted); }
.mono { font-family: var(--font-mono); }
.banner { border-radius: var(--radius); padding: .7rem 1rem; font-weight: 500; border: var(--border); }
.banner.err { color: var(--color-danger); background: var(--color-surface-mute); }

.section { display: flex; flex-direction: column; gap: var(--space-3); }
.section-head { display: flex; align-items: baseline; gap: var(--space-3); }
.section-title { margin: 0; font-size: var(--font-size-lg); }

.grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(220px, 1fr)); gap: var(--space-3); }
.stat-card { transition: border-color .15s; }
.name { margin: 0; font-size: var(--font-size-base); overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.stats { display: flex; gap: var(--space-5); margin-top: var(--space-2); }
.stat { display: flex; flex-direction: column; }
.stat-num { font-family: var(--font-mono); font-size: var(--font-size-xl); font-weight: 700; color: var(--color-fg); }
.stat-lbl { font-size: var(--font-size-xs); letter-spacing: .06em; text-transform: uppercase; color: var(--color-text-ghost); }

.board { padding: 0; }
.rows { list-style: none; margin: 0; padding: 0; }
.row { display: flex; align-items: center; gap: var(--space-3); padding: var(--space-3) var(--space-4); border-bottom: var(--border); }
.row:last-child { border-bottom: none; }
.rank { font-family: var(--font-mono); font-size: var(--font-size-sm); color: var(--color-muted); min-width: 2.5rem; }
.rank.top { color: var(--color-accent); font-weight: 700; }
.uname { flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.xp { color: var(--color-muted); font-size: var(--font-size-sm); }
.more { padding: var(--space-2) var(--space-4); border-top: var(--border); }
@media (max-width: 768px) { .page { padding: 1rem; } .grid { grid-template-columns: 1fr; } }
</style>
