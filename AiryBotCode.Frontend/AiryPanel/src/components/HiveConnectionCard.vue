<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { Alert, Button, Card, ConfirmDialog, Field, Icon, Modal, Select, StatusDot, Textarea, TextField, useToast } from '@airy/ui'
import { api, type HiveAgent, type HiveCheck, type HiveConnection, type HiveTry } from '@/lib/api'

const props = defineProps<{ botId: string }>()
const emit = defineEmits<{ changed: [check: HiveCheck | null] }>()
const toast = useToast()

const hive = ref<HiveConnection | null>(null)
const check = ref<HiveCheck | null>(null)
const loading = ref(false)
const checking = ref(false)
const error = ref<string | null>(null)

const open = ref(false)
const url = ref('')
const key = ref('')
const agentId = ref('')
const agents = ref<HiveAgent[] | null>(null)
const finding = ref(false)
const findError = ref<string | null>(null)
const saving = ref(false)
const saveError = ref<string | null>(null)
const confirmDrop = ref(false)

const trying = ref(false)
const tryOpen = ref(false)
const tryMessage = ref('')
const tryChannel = ref('general')
const tryResult = ref<HiveTry | null>(null)
const tryError = ref<string | null>(null)
const canTry = computed(() => connected.value && !!check.value?.canDecide && (check.value?.deciders.length ?? 0) > 0)

async function ask() {
  if (!tryMessage.value.trim()) return
  trying.value = true
  tryError.value = null
  tryResult.value = null
  try {
    tryResult.value = await api.tryHive(props.botId, tryMessage.value, tryChannel.value)
  } catch (e) {
    tryError.value = e instanceof Error ? e.message : 'The Hive did not answer.'
  } finally {
    trying.value = false
  }
}

const connected = computed(() => !!hive.value?.url && !!hive.value.hasKey && !!hive.value.agentId)
const agentName = computed(() => check.value?.agents.find(a => a.id === hive.value?.agentId)?.name ?? null)
const dot = computed(() => (!connected.value ? 'off' : !check.value ? 'off' : !check.value.reachable || !check.value.canRun ? 'err' : check.value.problems.length ? 'warn' : 'ok'))
const agentOptions = computed(() => (agents.value ?? []).map(a => ({ label: a.name, value: a.id })))

async function load() {
  loading.value = true
  error.value = null
  check.value = null
  try {
    hive.value = await api.hive(props.botId)
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'The Hive connection could not be read.'
  } finally {
    loading.value = false
  }
  await recheck()
}

async function recheck() {
  if (!connected.value) {
    emit('changed', null)
    return
  }
  checking.value = true
  try {
    check.value = await api.hiveCheck(props.botId)
  } catch (e) {
    check.value = null
    error.value = e instanceof Error ? e.message : 'The check failed.'
  } finally {
    checking.value = false
    emit('changed', check.value)
  }
}

function edit() {
  url.value = hive.value?.url ?? 'http://spine:7000'
  key.value = ''
  agentId.value = hive.value?.agentId ?? ''
  agents.value = check.value?.agents ?? null
  findError.value = null
  saveError.value = null
  open.value = true
}

async function findAgents() {
  finding.value = true
  findError.value = null
  try {
    agents.value = await api.hiveAgents(props.botId, url.value.trim(), key.value.trim())
    if (!agents.value.some(a => a.id === agentId.value)) agentId.value = agents.value[0]?.id ?? ''
    if (!agents.value.length) findError.value = 'This key may not run any agent yet.'
  } catch (e) {
    agents.value = null
    findError.value = e instanceof Error ? e.message : 'No agents found.'
  } finally {
    finding.value = false
  }
}

async function save() {
  saving.value = true
  saveError.value = null
  try {
    await api.saveHive(props.botId, { url: url.value.trim(), agentId: agentId.value, appKey: key.value.trim() || undefined })
    open.value = false
    toast.success('Hive connection saved')
    await load()
  } catch (e) {
    saveError.value = e instanceof Error ? e.message : 'Saving failed.'
  } finally {
    saving.value = false
  }
}

async function drop() {
  confirmDrop.value = false
  try {
    await api.dropHive(props.botId)
    open.value = false
    toast.success('Airy is no longer connected to the Hive')
    await load()
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Disconnecting failed.')
  }
}

watch(() => props.botId, load, { immediate: true })
defineExpose({ reload: load })
</script>

<template>
  <Card class="hive">
    <template #head>
      <div class="head">
        <StatusDot :variant="dot" />
        <h2>Hive connection</h2>
        <span class="spacer" />
        <Button v-if="canTry" size="sm" variant="ghost" @click="tryOpen = true; tryResult = null">Try a message</Button>
        <Button v-if="connected" size="sm" variant="outline" @click="edit">Change</Button>
      </div>
    </template>
    <Alert v-if="error" variant="danger" :title="error" dense>
      <template #actions><Button size="sm" variant="outline" @click="load">Try again</Button></template>
    </Alert>
    <div v-else-if="loading" class="line muted">Reading the connection…</div>
    <div v-else-if="!connected" class="first">
      <p>Airy answers through one Hive agent with one app key, in every channel below.</p>
      <Button @click="edit"><Icon name="plug" :size="16" />Connect the Hive</Button>
    </div>
    <div v-else class="facts">
      <p class="line">
        Airy speaks as <strong>{{ agentName ?? 'its agent' }}</strong>
        <span class="mono">{{ hive!.url }}</span>
        <span class="mono">{{ hive!.keyHint }}</span>
      </p>
      <p v-if="checking" class="line muted">Checking what the key may do…</p>
      <ul v-else-if="check" class="checks">
        <li :class="check.canRun && check.agentFound ? 'ok' : 'bad'">
          <Icon :name="check.canRun && check.agentFound ? 'check' : 'x'" :size="14" />
          {{ check.canRun && check.agentFound ? 'Runs the agent' : 'Cannot run the agent' }}
        </li>
        <li :class="check.canDecide && check.deciders.length ? 'ok' : 'warn'">
          <Icon :name="check.canDecide && check.deciders.length ? 'check' : 'triangle-alert'" :size="14" />
          {{ check.canDecide && check.deciders.length ? `Decides with ${check.deciders[0]}` : 'Cannot decide yet' }}
        </li>
      </ul>
      <p v-for="p in check?.problems ?? []" :key="p" class="problem">{{ p }}</p>
    </div>

    <Modal :open="open" title="Hive connection" size="md" @close="open = false">
      <form class="form" @submit.prevent="save">
        <Field label="Hive address" hint="Inside the server it is http://spine:7000">
          <TextField v-model="url" type="url" autocomplete="off" />
        </Field>
        <Field label="App key" :hint="hive?.hasKey ? 'Leave empty to keep the stored key' : 'From the Hive: Agent API, App keys'">
          <TextField v-model="key" type="password" autocomplete="off" />
        </Field>
        <div class="agents">
          <Field label="Agent Airy speaks as" :error="findError ?? undefined">
            <Select v-model="agentId" :options="agentOptions" :placeholder="agents ? 'Pick an agent' : 'Find the agents first'" :disabled="!agents?.length" />
          </Field>
          <Button variant="outline" :loading="finding" :disabled="!url || (!key && !hive?.hasKey)" @click="findAgents">Find agents</Button>
        </div>
        <Alert v-if="saveError" variant="danger" :title="saveError" dense />
      </form>
      <template #footer>
        <Button v-if="connected" variant="ghost" class="drop" @click="confirmDrop = true">Disconnect</Button>
        <span class="spacer" />
        <Button variant="ghost" @click="open = false">Cancel</Button>
        <Button :loading="saving" :disabled="!url || !agentId || (!key && !hive?.hasKey)" @click="save">Save</Button>
      </template>
    </Modal>
    <Modal :open="tryOpen" title="Would Airy answer this?" size="md" @close="tryOpen = false">
      <form class="form" @submit.prevent="ask">
        <Field label="Message">
          <Textarea v-model="tryMessage" :rows="3" />
        </Field>
        <Field label="Channel name" hint="The Hive reads it as part of the message">
          <TextField v-model="tryChannel" autocomplete="off" />
        </Field>
        <Alert v-if="tryError" variant="danger" :title="tryError" dense />
        <div v-if="tryResult" class="verdict" :class="tryResult.error ? 'bad' : tryResult.answers ? 'yes' : 'no'" role="status">
          <Icon :name="tryResult.error ? 'x' : tryResult.answers ? 'check' : 'minus'" :size="18" />
          <span v-if="tryResult.error">{{ tryResult.error }}</span>
          <span v-else-if="tryResult.answers">Airy would answer</span>
          <span v-else>Airy would stay quiet{{ tryResult.yes && !tryResult.sure ? ' (a yes, but not sure enough)' : '' }}</span>
          <span v-if="!tryResult.error" class="mono">{{ tryResult.summary }}</span>
        </div>
      </form>
      <template #footer>
        <Button variant="ghost" @click="tryOpen = false">Close</Button>
        <Button :loading="trying" :disabled="!tryMessage.trim()" @click="ask">Ask the Hive</Button>
      </template>
    </Modal>
    <ConfirmDialog
      :open="confirmDrop"
      title="Disconnect Airy from the Hive?"
      message="Airy stops answering in every channel until it is connected again. The channels and their rules stay."
      confirm-text="Disconnect"
      danger
      @confirm="drop"
      @cancel="confirmDrop = false"
    />
  </Card>
</template>

<style scoped>
.head {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  width: 100%;
}
h2 {
  margin: 0;
  font-size: var(--font-size-lg);
  color: var(--color-fg);
}
.spacer {
  flex: 1;
}
.first {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-3);
}
.first p {
  margin: 0;
  color: var(--color-text-dim);
}
.facts {
  display: grid;
  gap: var(--space-2);
}
.line {
  margin: 0;
  display: flex;
  flex-wrap: wrap;
  align-items: baseline;
  gap: var(--space-2);
  color: var(--color-text-dim);
}
.line strong {
  color: var(--color-fg);
}
.muted {
  color: var(--color-muted);
}
.mono {
  font-family: var(--font-mono);
  font-size: var(--font-size-xs);
  color: var(--color-muted);
}
.checks {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-wrap: wrap;
  gap: var(--space-4);
}
.checks li {
  display: inline-flex;
  align-items: center;
  gap: var(--space-1);
  font-size: var(--font-size-sm);
}
.checks .ok {
  color: var(--color-ok);
}
.checks .warn {
  color: var(--color-warn);
}
.checks .bad {
  color: var(--color-danger);
}
.problem {
  margin: 0;
  font-size: var(--font-size-sm);
  color: var(--color-text-dim);
}
.form {
  display: grid;
  gap: var(--space-4);
}
.agents {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto;
  align-items: end;
  gap: var(--space-2);
}
.drop {
  color: var(--color-danger);
}
.verdict {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-3);
  border: var(--border);
  border-radius: var(--r-2);
  color: var(--color-fg);
}
.verdict.yes {
  box-shadow: inset 2px 0 0 var(--color-ok);
}
.verdict.no {
  box-shadow: inset 2px 0 0 var(--color-muted);
}
.verdict.bad {
  box-shadow: inset 2px 0 0 var(--color-danger);
  color: var(--color-danger);
}
@media (max-width: 520px) {
  .agents {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
