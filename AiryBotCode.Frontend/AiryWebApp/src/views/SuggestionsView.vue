<script setup lang="ts">
// Team feature-suggestions board. Anyone logged in can submit an idea; the
// maintainer later adds a review (why it's good + a time estimate) and the team
// marks it approved/rejected. The review text is written from the API/script,
// shown here read-only; the buttons drive approve/reject/delete.
import { ref, computed, onMounted } from 'vue'
import { PageHeader, Card, Button, Badge } from '@hive/ui'
import { api, ApiError, type Suggestion } from '../lib/api'
import { relativeTime } from '../lib/relativeTime'

const items = ref<Suggestion[]>([])
const loading = ref(true)
const error = ref('')

const title = ref('')
const body = ref('')
const submitting = ref(false)
const submitMsg = ref('')

const now = ref(Date.now())

const sorted = computed(() =>
  [...items.value].sort((a, b) => b.createdAt.localeCompare(a.createdAt)),
)
const openCount = computed(() => items.value.filter((s) => s.status === 'new').length)

function statusVariant(s: Suggestion): 'active' | 'inactive' {
  return s.status === 'approved' ? 'active' : 'inactive'
}

async function load() {
  loading.value = true
  error.value = ''
  try {
    items.value = await api.getSuggestions()
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Failed to load suggestions.'
  } finally {
    loading.value = false
  }
}

async function submit() {
  if (!title.value.trim() || !body.value.trim()) return
  submitting.value = true
  submitMsg.value = ''
  try {
    const created = await api.createSuggestion(title.value.trim(), body.value.trim())
    items.value.unshift(created)
    title.value = ''
    body.value = ''
    submitMsg.value = 'Thanks — your idea was added. 🎉'
    setTimeout(() => (submitMsg.value = ''), 4000)
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Could not submit.'
  } finally {
    submitting.value = false
  }
}

async function approve(s: Suggestion) {
  try { Object.assign(s, await api.approveSuggestion(s.id)) }
  catch (e) { error.value = e instanceof ApiError ? e.message : 'Approve failed.' }
}
async function reject(s: Suggestion) {
  try { Object.assign(s, await api.rejectSuggestion(s.id)) }
  catch (e) { error.value = e instanceof ApiError ? e.message : 'Reject failed.' }
}
async function remove(s: Suggestion) {
  if (!confirm(`Delete "${s.title}"? This can't be undone.`)) return
  try {
    await api.deleteSuggestion(s.id)
    items.value = items.value.filter((x) => x.id !== s.id)
  } catch (e) { error.value = e instanceof ApiError ? e.message : 'Delete failed.' }
}

// ── Public share link (capability URL) ───────────────────────────────────────
const shareUrl = ref('')
const shareLoading = ref(true)
const shareMsg = ref('')

async function loadShareLink() {
  shareLoading.value = true
  try {
    const { sharePath } = await api.getSuggestionShareLink()
    shareUrl.value = window.location.origin + sharePath
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Could not load the share link.'
  } finally {
    shareLoading.value = false
  }
}

async function copyShareLink() {
  try {
    await navigator.clipboard.writeText(shareUrl.value)
    shareMsg.value = 'Copied!'
  } catch {
    shareMsg.value = 'Copy failed — select the link and copy manually.'
  }
  setTimeout(() => (shareMsg.value = ''), 2500)
}

async function regenerateShareLink() {
  if (!confirm('Regenerate the share link? The current link stops working immediately.')) return
  try {
    const { sharePath } = await api.regenerateSuggestionShareLink()
    shareUrl.value = window.location.origin + sharePath
    shareMsg.value = 'New link generated — the old one is now dead.'
    setTimeout(() => (shareMsg.value = ''), 3500)
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Could not regenerate the link.'
  }
}

onMounted(() => { load(); loadShareLink() })
</script>

<template>
  <div class="page">
    <PageHeader
      title="Suggestions"
      subtitle="Drop feature ideas for the project. The maintainer reviews each one with a why + an estimate, then the team approves what ships."
    >
      <template #actions>
        <Badge variant="inactive">{{ openCount }} new</Badge>
        <Button variant="outline" :disabled="loading" @click="load">{{ loading ? 'Refreshing…' : '⟳ Refresh' }}</Button>
      </template>
    </PageHeader>

    <p v-if="error" class="banner err">{{ error }}</p>

    <!-- Public share link -->
    <Card>
      <template #head><h3>Public share link</h3></template>
      <p class="muted share-hint">Anyone with this link can view the board and submit ideas — no login. Approval stays here.</p>
      <div class="share-row">
        <input
          class="share-input mono"
          :value="shareLoading ? 'Loading…' : shareUrl"
          readonly
          @focus="($event.target as HTMLInputElement).select()"
        />
        <Button variant="outline" :disabled="shareLoading || !shareUrl" @click="copyShareLink">Copy</Button>
        <Button variant="outline" :disabled="shareLoading" @click="regenerateShareLink">Regenerate</Button>
      </div>
      <span v-if="shareMsg" class="ok">{{ shareMsg }}</span>
    </Card>

    <!-- Submit form -->
    <Card>
      <template #head><h3>New idea</h3></template>
      <div class="form">
        <input v-model="title" type="text" maxlength="200" placeholder="Short title (e.g. 'Welcome message per channel')" />
        <textarea v-model="body" rows="4" placeholder="Describe the idea — what it does and why it'd help."></textarea>
        <div class="form-foot">
          <span v-if="submitMsg" class="ok">{{ submitMsg }}</span>
          <div style="flex: 1" />
          <Button :disabled="submitting || !title.trim() || !body.trim()" @click="submit">
            {{ submitting ? 'Submitting…' : 'Submit idea' }}
          </Button>
        </div>
      </div>
    </Card>

    <!-- List -->
    <p v-if="loading" class="muted">Loading…</p>
    <p v-else-if="!items.length" class="muted">No suggestions yet — be the first to add one above.</p>
    <div v-else class="list">
      <Card v-for="s in sorted" :key="s.id" class="item" :class="{ approved: s.status === 'approved', rejected: s.status === 'rejected' }">
        <template #head>
          <h3 class="title">{{ s.title }}</h3>
          <div style="flex: 1" />
          <Badge :variant="statusVariant(s)">{{ s.status }}</Badge>
        </template>

        <p class="body">{{ s.body }}</p>
        <p class="meta mono">by {{ s.submittedBy || 'anonymous' }} · {{ relativeTime(s.createdAt, now) || 'just now' }}</p>

        <!-- Maintainer review (filled in later) -->
        <div v-if="s.responseWhy || s.responseEstimate" class="review">
          <p v-if="s.responseWhy" class="why"><span class="rlabel">Why it's good</span>{{ s.responseWhy }}</p>
          <p v-if="s.responseEstimate" class="est"><span class="rlabel">Estimate</span><span class="mono">{{ s.responseEstimate }}</span></p>
        </div>
        <p v-else class="pending muted">Awaiting review…</p>

        <template #foot>
          <Button v-if="s.status !== 'approved'" @click="approve(s)">Approve</Button>
          <Button v-if="s.status !== 'rejected'" variant="ghost" @click="reject(s)">Reject</Button>
          <div style="flex: 1" />
          <Button variant="ghost" @click="remove(s)">Delete</Button>
        </template>
      </Card>
    </div>
  </div>
</template>

<style scoped>
.page { padding: 2rem; display: flex; flex-direction: column; gap: 1rem; max-width: 860px; }
.muted { color: var(--color-muted); }
.mono { font-family: var(--font-mono); }
.banner { border-radius: var(--radius); padding: .7rem 1rem; font-weight: 500; border: var(--border); }
.banner.err { color: var(--color-danger); background: var(--color-surface-mute); }
.ok { color: var(--color-ok); font-weight: 500; }

.share-hint { margin: 0 0 0.6rem; font-size: 0.85rem; }
.share-row { display: flex; gap: 0.5rem; align-items: center; flex-wrap: wrap; }
.share-input {
  flex: 1; min-width: 12rem; box-sizing: border-box; padding: 0.55rem; font-size: 0.85rem;
  border: 1px solid var(--color-border); border-radius: var(--radius); background: var(--color-bg-2);
  color: var(--color-fg);
}
.form { display: flex; flex-direction: column; gap: 0.6rem; }
.form input, .form textarea {
  width: 100%; box-sizing: border-box; padding: 0.6rem; border: 1px solid var(--color-border);
  border-radius: 8px; font-size: 16px; background: var(--color-surface); color: var(--color-fg);
}
.form textarea { resize: vertical; min-height: 80px; }
.form-foot { display: flex; align-items: center; gap: 0.5rem; }

.list { display: flex; flex-direction: column; gap: 0.75rem; }
.item.approved { border-color: var(--color-accent-edge); }
.item.rejected { opacity: 0.62; }
.title { margin: 0; font-size: var(--font-size-base); }
.body { margin: 0.25rem 0; white-space: pre-wrap; }
.meta { font-size: var(--font-size-sm); color: var(--color-muted); margin: 0.25rem 0 0; }

.review { margin-top: 0.75rem; padding: 0.6rem 0.75rem; border: var(--border);
  border-radius: 8px; background: var(--color-surface-mute); display: flex; flex-direction: column; gap: 0.4rem; }
.rlabel { display: block; font-size: var(--font-size-xs); letter-spacing: .06em; text-transform: uppercase;
  color: var(--color-text-ghost); margin-bottom: 0.15rem; }
.why, .est { margin: 0; }
.est .mono { color: var(--color-accent); }
.pending { margin: 0.6rem 0 0; font-style: italic; font-size: var(--font-size-sm); }
@media (max-width: 768px) { .page { padding: 1rem; } }
</style>
