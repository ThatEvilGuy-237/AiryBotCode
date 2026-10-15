<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { Alert, Button, Card, ConfirmDialog, CopyButton, EmptyState, Field, Icon, Modal, PageHeader, Skeleton, Textarea, TextField, useToast } from '@airy/ui'
import SuggestionCard from '@/components/SuggestionCard.vue'
import { api, type ShareLink, type Suggestion } from '@/lib/api'

const toast = useToast()

const list = ref<Suggestion[] | null>(null)
const error = ref<string | null>(null)
const link = ref<ShareLink | null>(null)
const renewAsk = ref(false)
const dropping = ref<Suggestion | null>(null)
const writing = ref(false)
const title = ref('')
const body = ref('')
const sending = ref(false)
const sendError = ref<string | null>(null)
const showDone = ref(false)

const waiting = computed(() => (list.value ?? []).filter(s => s.status === 'new' || s.status === 'reviewed'))
const done = computed(() => (list.value ?? []).filter(s => s.status === 'approved' || s.status === 'rejected'))
const url = computed(() => (link.value ? `${location.origin}${link.value.sharePath}` : ''))

const newest = (a: Suggestion, b: Suggestion) => b.createdAt.localeCompare(a.createdAt)

async function load() {
  error.value = null
  try {
    const [items, share] = await Promise.all([api.suggestions(), api.shareLink().catch(() => null)])
    list.value = items.sort(newest)
    link.value = share
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'The suggestions could not be loaded.'
  }
}

onMounted(load)

async function decide(s: Suggestion, approve: boolean) {
  const before = s.status
  s.status = approve ? 'approved' : 'rejected'
  try {
    if (approve) await api.approveSuggestion(s.id)
    else await api.rejectSuggestion(s.id)
    toast.success(approve ? `"${s.title}" approved` : `"${s.title}" set to not now`)
  } catch (e) {
    s.status = before
    toast.error(e instanceof Error ? e.message : 'That did not work.')
  }
}

async function drop() {
  const s = dropping.value
  dropping.value = null
  if (!s) return
  try {
    await api.dropSuggestion(s.id)
    list.value = (list.value ?? []).filter(x => x.id !== s.id)
    toast.success(`"${s.title}" deleted`)
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Deleting failed.')
  }
}

async function renew() {
  renewAsk.value = false
  try {
    link.value = await api.renewShareLink()
    toast.success('New link made; the old one no longer works')
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'A new link could not be made.')
  }
}

async function send() {
  sending.value = true
  sendError.value = null
  try {
    const created = await api.addSuggestion(title.value.trim(), body.value.trim())
    list.value = [created, ...(list.value ?? [])]
    writing.value = false
    title.value = ''
    body.value = ''
    toast.success('Suggestion added')
  } catch (e) {
    sendError.value = e instanceof Error ? e.message : 'Sending failed.'
  } finally {
    sending.value = false
  }
}
</script>

<template>
  <div class="page">
    <PageHeader title="Suggestions" :subtitle="list ? `// ${waiting.length} waiting` : '// ideas from the community'">
      <template #actions>
        <Button variant="outline" @click="writing = true"><Icon name="plus" :size="16" />Write one</Button>
      </template>
    </PageHeader>

    <Card v-if="link">
      <template #head><h2>Public board</h2></template>
      <div class="share">
        <p>Members suggest ideas at this link, no sign-in needed.</p>
        <div class="link-row">
          <span class="url" :title="url">{{ url }}</span>
          <CopyButton :value="url" label="Copy" size="sm" />
          <Button variant="ghost" size="sm" @click="renewAsk = true">New link</Button>
        </div>
      </div>
    </Card>

    <Alert v-if="error" variant="danger" :title="error">
      <template #actions><Button size="sm" variant="outline" @click="load">Try again</Button></template>
    </Alert>
    <div v-else-if="list === null" class="cards">
      <Skeleton v-for="i in 3" :key="i" variant="block" height="140px" />
    </div>
    <template v-else>
      <section aria-labelledby="waiting-title" class="group">
        <h2 id="waiting-title">Waiting <span class="count">{{ waiting.length }}</span></h2>
        <EmptyState v-if="!waiting.length" title="Nothing waiting" sub="New ideas from the public board and from Airy's chats land here." />
        <div v-else class="cards">
          <SuggestionCard v-for="s in waiting" :key="s.id" :suggestion="s">
            <Button size="sm" @click="decide(s, true)"><Icon name="check" :size="16" />Approve</Button>
            <Button size="sm" variant="outline" @click="decide(s, false)">Not now</Button>
            <Button size="sm" variant="ghost" :aria-label="`Delete ${s.title}`" @click="dropping = s"><Icon name="trash-2" :size="16" /></Button>
          </SuggestionCard>
        </div>
      </section>

      <section v-if="done.length" aria-labelledby="done-title" class="group">
        <div class="group-head">
          <h2 id="done-title">Decided <span class="count">{{ done.length }}</span></h2>
          <Button variant="ghost" size="sm" :aria-expanded="showDone" @click="showDone = !showDone">{{ showDone ? 'Hide' : 'Show' }}</Button>
        </div>
        <div v-if="showDone" class="cards">
          <SuggestionCard v-for="s in done" :key="s.id" :suggestion="s">
            <Button size="sm" variant="ghost" :aria-label="`Delete ${s.title}`" @click="dropping = s"><Icon name="trash-2" :size="16" /></Button>
          </SuggestionCard>
        </div>
      </section>
    </template>

    <Modal :open="writing" title="Write a suggestion" size="md" @close="writing = false">
      <form class="form" @submit.prevent="send">
        <Field label="Title"><TextField v-model="title" autocomplete="off" /></Field>
        <Field label="What should change"><Textarea v-model="body" :rows="4" /></Field>
        <Alert v-if="sendError" variant="danger" :title="sendError" dense />
      </form>
      <template #footer>
        <Button variant="ghost" @click="writing = false">Cancel</Button>
        <Button :loading="sending" :disabled="!title.trim() || !body.trim()" @click="send">Add</Button>
      </template>
    </Modal>
    <ConfirmDialog
      :open="renewAsk"
      title="Make a new public link?"
      message="The current link stops working at once; share the new one where the old one was."
      confirm-text="Make a new link"
      @confirm="renew"
      @cancel="renewAsk = false"
    />
    <ConfirmDialog
      :open="dropping !== null"
      :title="`Delete &quot;${dropping?.title ?? ''}&quot;?`"
      message="It disappears from the panel and the public board."
      confirm-text="Delete"
      danger
      @confirm="drop"
      @cancel="dropping = null"
    />
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
.count {
  font-family: var(--font-mono);
  font-size: var(--font-size-sm);
  color: var(--color-muted);
}
.share {
  display: grid;
  gap: var(--space-2);
}
.share > * {
  min-width: 0;
}
.share p {
  margin: 0;
  color: var(--color-text-dim);
}
.link-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: var(--space-2);
}
.url {
  flex: 1 1 200px;
  min-width: 0;
  max-width: 100%;
  font-family: var(--font-mono);
  font-size: var(--font-size-sm);
  color: var(--color-fg);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.group {
  display: grid;
  gap: var(--space-3);
}
.group-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.cards > *,
.group > * {
  min-width: 0;
}
.cards {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(min(320px, 100%), 1fr));
  gap: var(--space-3);
}
.form {
  display: grid;
  gap: var(--space-4);
}
@media (max-width: 520px) {
  .cards {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
