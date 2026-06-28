<script setup lang="ts">
// PUBLIC suggestions board, reached via a capability link (/suggest/:code). No
// login — the code in the URL is the only credential, and the server validates it
// on every request: a bad/expired code returns 404 and we render nothing but an
// "invalid link" notice (the SPA route is UX only, never the security boundary).
// Calls the [AllowAnonymous] api/public/suggestions/{code} endpoints directly with
// plain fetch (NOT the dashboard `api`, which adds the JWT + redirects on 401).
import { ref, onMounted } from 'vue'
import { API_BASE_URL } from '../lib/config'
import { relativeTime } from '../lib/relativeTime'

const props = defineProps<{ code: string }>()

interface PublicSuggestion {
  id: number
  title: string
  body: string
  submittedBy: string
  createdAt: string
  status: string
  responseWhy?: string | null
  responseEstimate?: string | null
  approved: boolean
}

const items = ref<PublicSuggestion[]>([])
const loading = ref(true)
const invalid = ref(false) // server said the link is bad/expired (404)
const error = ref('')

const title = ref('')
const body = ref('')
const submittedBy = ref('')
const submitting = ref(false)
const submitMsg = ref('')

const endpoint = () =>
  `${API_BASE_URL}/api/public/suggestions/${encodeURIComponent(props.code)}`

async function load() {
  loading.value = true
  error.value = ''
  try {
    const res = await fetch(endpoint(), { headers: { accept: 'application/json' } })
    if (res.status === 404) { invalid.value = true; return }
    if (!res.ok) { error.value = `Couldn't load the board (${res.status}).`; return }
    items.value = (await res.json()) as PublicSuggestion[]
  } catch {
    error.value = 'Network error loading the board.'
  } finally {
    loading.value = false
  }
}

async function submit() {
  if (!title.value.trim() || !body.value.trim() || submitting.value) return
  submitting.value = true
  submitMsg.value = ''
  error.value = ''
  try {
    const res = await fetch(endpoint(), {
      method: 'POST',
      headers: { 'content-type': 'application/json' },
      body: JSON.stringify({
        title: title.value.trim(),
        body: body.value.trim(),
        submittedBy: submittedBy.value.trim() || undefined,
      }),
    })
    if (res.status === 404) { invalid.value = true; return }
    if (res.status === 429) { error.value = 'Too many submissions — please wait a minute and try again.'; return }
    if (!res.ok) { error.value = `Could not submit (${res.status}).`; return }
    const created = (await res.json()) as PublicSuggestion
    items.value.unshift(created)
    title.value = ''
    body.value = ''
    submitMsg.value = 'Thanks — your idea was added. 🎉'
    setTimeout(() => (submitMsg.value = ''), 4000)
  } catch {
    error.value = 'Network error — your idea was not submitted.'
  } finally {
    submitting.value = false
  }
}

onMounted(load)
</script>

<template>
  <!-- Bad/expired link: render NOTHING but the notice (the code is the only gate). -->
  <div v-if="invalid" class="invalid">
    <h1>Link not valid</h1>
    <p>This suggestions link is invalid or has expired. Ask whoever shared it for a fresh one.</p>
  </div>

  <div v-else class="board">
    <header class="head">
      <h1>Suggestions</h1>
      <p class="sub">Share an idea — no login needed.</p>
    </header>

    <section class="submit">
      <h2>Suggest an idea</h2>
      <label>Title
        <input v-model="title" maxlength="200" placeholder="Short summary" />
      </label>
      <label>Details
        <textarea v-model="body" maxlength="4000" rows="4" placeholder="What would you like?"></textarea>
      </label>
      <label>Your name (optional)
        <input v-model="submittedBy" maxlength="80" placeholder="anonymous" />
      </label>
      <button class="btn" :disabled="submitting || !title.trim() || !body.trim()" @click="submit">
        {{ submitting ? 'Submitting…' : 'Submit idea' }}
      </button>
      <p v-if="submitMsg" class="ok">{{ submitMsg }}</p>
      <p v-if="error" class="err">{{ error }}</p>
    </section>

    <p v-if="loading" class="muted">Loading…</p>
    <section v-else class="list">
      <article v-for="s in items" :key="s.id" class="item">
        <div class="item-head">
          <strong>{{ s.title }}</strong>
          <span class="status" :class="s.status">{{ s.status }}</span>
        </div>
        <p class="text">{{ s.body }}</p>
        <p v-if="s.responseWhy" class="resp">
          <em>Response:</em> {{ s.responseWhy }}
          <span v-if="s.responseEstimate"> · est. {{ s.responseEstimate }}</span>
        </p>
        <p class="meta">{{ s.submittedBy || 'anonymous' }} · {{ relativeTime(s.createdAt) }}</p>
      </article>
      <p v-if="!items.length" class="muted">No suggestions yet — be the first!</p>
    </section>
  </div>
</template>

<style scoped>
.board { max-width: 720px; margin: 0 auto; padding: 1.25rem; }
.head h1 { margin: 0; }
.sub { opacity: .65; margin: .25rem 0 1.25rem; }
.submit { display: flex; flex-direction: column; gap: .5rem; margin-bottom: 1.5rem; }
.submit h2 { margin: 0 0 .25rem; font-size: 1.1rem; }
.submit label { display: flex; flex-direction: column; gap: .25rem; font-size: .9rem; opacity: .9; }
.submit input, .submit textarea {
  padding: .55rem; border: 1px solid var(--color-border, #ccc); border-radius: 6px;
  background: var(--color-bg-2, #fff); color: inherit; font: inherit;
}
.btn {
  align-self: flex-start; padding: .55rem 1rem; border: none; border-radius: 6px; cursor: pointer;
  background: var(--color-accent, #5865f2); color: #fff; font: inherit;
}
.btn:disabled { opacity: .5; cursor: not-allowed; }
.list { display: flex; flex-direction: column; gap: .75rem; }
.item { padding: .85rem 1rem; border: 1px solid var(--color-border, #ddd); border-radius: 8px; background: var(--color-bg-2, #fff); }
.item-head { display: flex; justify-content: space-between; align-items: center; gap: .5rem; }
.status { font-size: .72rem; opacity: .7; text-transform: capitalize; }
.text { white-space: pre-wrap; margin: .4rem 0; }
.resp { font-size: .9rem; opacity: .85; }
.meta { font-size: .78rem; opacity: .6; margin: .3rem 0 0; }
.muted { opacity: .6; }
.ok { color: var(--color-success, green); }
.err { color: var(--color-danger, crimson); }
.invalid { max-width: 480px; margin: 4rem auto; text-align: center; padding: 1rem; }
</style>
