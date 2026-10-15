<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { Alert, BrandMark, Button, EmptyState, Field, Skeleton, Textarea, TextField } from '@airy/ui'
import SuggestionCard from '@/components/SuggestionCard.vue'
import { readError, type Suggestion } from '@/lib/api'

const props = defineProps<{ code: string }>()

const list = ref<Suggestion[] | null>(null)
const invalid = ref(false)
const loadError = ref<string | null>(null)
const title = ref('')
const body = ref('')
const name = ref('')
const sending = ref(false)
const sendError = ref<string | null>(null)
const sent = ref<number | null>(null)

const base = `/api/public/suggestions/${encodeURIComponent(props.code)}`

async function load() {
  loadError.value = null
  try {
    const response = await fetch(base)
    if (response.status === 404) {
      invalid.value = true
      return
    }
    if (!response.ok) throw new Error(readError(await response.text(), response.status))
    list.value = ((await response.json()) as Suggestion[]).sort((a, b) => b.createdAt.localeCompare(a.createdAt))
  } catch (e) {
    loadError.value = e instanceof Error ? e.message : 'The ideas could not be loaded.'
  }
}

onMounted(load)

async function send() {
  sending.value = true
  sendError.value = null
  try {
    const response = await fetch(base, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ title: title.value.trim(), body: body.value.trim(), submittedBy: name.value.trim() || null }),
    })
    if (response.status === 429) throw new Error('That was a lot at once. Wait a minute and send again.')
    if (response.status === 404) {
      invalid.value = true
      return
    }
    if (!response.ok) throw new Error(readError(await response.text(), response.status))
    const created = (await response.json()) as Suggestion
    list.value = [created, ...(list.value ?? [])]
    sent.value = created.id
    title.value = ''
    body.value = ''
  } catch (e) {
    sendError.value = e instanceof Error ? e.message : 'Sending failed.'
  } finally {
    sending.value = false
  }
}
</script>

<template>
  <main class="public">
    <div class="column">
      <BrandMark name="Airy" :size="36" />
      <EmptyState v-if="invalid" title="This link no longer works" sub="Ask the server team for the new link to the ideas board." />
      <template v-else>
        <header>
          <h1>Ideas for the server</h1>
          <p class="lead">Tell the team what would make it better. Every idea is read.</p>
        </header>
        <form class="form" @submit.prevent="send">
          <Field label="Your idea in a few words"><TextField v-model="title" autocomplete="off" /></Field>
          <Field label="Tell a bit more"><Textarea v-model="body" :rows="4" /></Field>
          <Field label="Your name" hint="Optional"><TextField v-model="name" autocomplete="nickname" /></Field>
          <Alert v-if="sendError" variant="danger" :title="sendError" dense />
          <Alert v-if="sent" variant="ok" title="Thanks, your idea is in. It is at the top of the list below." dense />
          <Button type="submit" :loading="sending" :disabled="!title.trim() || !body.trim()" class="go">Send my idea</Button>
        </form>
        <section aria-labelledby="ideas-title" class="ideas">
          <h2 id="ideas-title">Ideas so far</h2>
          <Alert v-if="loadError" variant="danger" :title="loadError" dense>
            <template #actions><Button size="sm" variant="outline" @click="load">Try again</Button></template>
          </Alert>
          <div v-else-if="list === null" class="cards">
            <Skeleton v-for="i in 2" :key="i" variant="block" height="120px" />
          </div>
          <EmptyState v-else-if="!list.length" title="No ideas yet" sub="Yours can be the first." />
          <div v-else class="cards">
            <SuggestionCard v-for="s in list" :key="s.id" :suggestion="s" />
          </div>
        </section>
      </template>
    </div>
  </main>
</template>

<style scoped>
.public {
  min-height: 100vh;
  min-height: 100dvh;
  background: var(--color-bg);
  padding: var(--space-6) var(--space-4) var(--space-6);
}
.column {
  width: min(640px, 100%);
  margin: 0 auto;
  display: grid;
  gap: var(--space-5);
}
h1 {
  margin: 0 0 var(--space-2);
  font-family: var(--font-display);
  font-size: var(--font-size-xl);
  color: var(--color-fg);
}
h2 {
  margin: 0;
  font-size: var(--font-size-lg);
  color: var(--color-fg);
}
.lead {
  margin: 0;
  color: var(--color-text-dim);
}
.form {
  display: grid;
  gap: var(--space-4);
  padding: var(--space-5);
  border: var(--border);
  border-radius: var(--r-3);
  background: var(--color-surface);
}
.go {
  justify-self: start;
  min-height: 44px;
}
.ideas,
.cards {
  display: grid;
  gap: var(--space-3);
}
.column > *,
.ideas > *,
.cards > * {
  min-width: 0;
}
@media (max-width: 520px) {
  .go {
    justify-self: stretch;
  }
}
</style>
