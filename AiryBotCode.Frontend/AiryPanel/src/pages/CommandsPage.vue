<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { Alert, Button, ConfirmDialog, EmptyState, Icon, PageHeader, SearchField, Skeleton, Toggle, useToast } from '@airy/ui'
import CommandEditor from '@/components/CommandEditor.vue'
import { api, type DiscordEntity } from '@/lib/api'
import { botId } from '@/lib/bots'
import { commandName, settingName, type Command } from '@/lib/commands'

const toast = useToast()

const commands = ref<Command[] | null>(null)
const error = ref<string | null>(null)
const search = ref('')
const editing = ref<Command | null>(null)
const saving = ref(false)
const saveError = ref<string | null>(null)
const restartAsk = ref(false)
const restarting = ref(false)
const channels = ref<DiscordEntity[]>([])
const categories = ref<DiscordEntity[]>([])
const roles = ref<DiscordEntity[]>([])

const shown = computed(() => {
  const q = search.value.trim().toLowerCase()
  const list = [...(commands.value ?? [])].sort((a, b) => Number(b.enabled) - Number(a.enabled) || commandName(a.commandName).localeCompare(commandName(b.commandName)))
  if (!q) return list
  return list.filter(c => commandName(c.commandName).toLowerCase().includes(q) || c.settings.some(s => settingName(s.key).toLowerCase().includes(q) || (s.description ?? '').toLowerCase().includes(q)))
})
const onCount = computed(() => commands.value?.filter(c => c.enabled).length ?? 0)

async function load(id: string) {
  error.value = null
  commands.value = null
  try {
    const list = await api.commandsFull(id)
    if (botId.value === id) commands.value = list
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'The commands could not be loaded.'
  }
}

async function loadLists(id: string) {
  const [c, k, r] = await Promise.allSettled([api.discordChannels(id), api.discordCategories(id), api.discordRoles(id)])
  if (botId.value !== id) return
  channels.value = c.status === 'fulfilled' ? c.value : []
  categories.value = k.status === 'fulfilled' ? k.value : []
  roles.value = r.status === 'fulfilled' ? r.value : []
}

watch(botId, id => { if (id) load(id) }, { immediate: true })

function edit(c: Command) {
  saveError.value = null
  editing.value = c
  if (botId.value && !channels.value.length) loadLists(botId.value)
}

async function setOn(c: Command, enabled: boolean, undo = true) {
  if (!botId.value) return
  const before = c.enabled
  c.enabled = enabled
  try {
    await api.setCommandOn(botId.value, c.commandName, enabled)
    const said = `${commandName(c.commandName)} is ${enabled ? 'on' : 'off'}`
    if (undo) toast.action(said, { label: 'Undo', run: () => setOn(c, before, false) }, 'success')
    else toast.success(said)
  } catch (e) {
    c.enabled = before
    toast.error(e instanceof Error ? e.message : 'Switching failed.')
  }
}

async function save(values: { key: string; value: string }[]) {
  const c = editing.value
  if (!botId.value || !c || !values.length) return
  saving.value = true
  saveError.value = null
  try {
    await api.saveCommand(botId.value, c.commandName, values)
    const restart = values.some(v => !c.settings.find(s => s.key === v.key)?.isReloadable)
    for (const v of values) {
      const s = c.settings.find(x => x.key === v.key)
      if (s) s.value = v.value
    }
    editing.value = null
    if (restart) toast.action(`${commandName(c.commandName)} saved; some changes apply after a restart`, { label: 'Restart bots', run: () => { restartAsk.value = true } }, 'success')
    else toast.success(`${commandName(c.commandName)} saved`)
  } catch (e) {
    saveError.value = e instanceof Error ? e.message : 'Saving failed.'
  } finally {
    saving.value = false
  }
}

async function restart() {
  restartAsk.value = false
  restarting.value = true
  try {
    await api.restartBots()
    toast.success('The bots are restarting; they are back in about a minute')
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'The restart could not be asked.')
  } finally {
    restarting.value = false
  }
}
</script>

<template>
  <div class="page">
    <PageHeader title="Commands" :subtitle="commands ? `// ${onCount} of ${commands.length} on` : '// what Airy does in its servers'">
      <template #actions>
        <Button variant="outline" :loading="restarting" @click="restartAsk = true"><Icon name="refresh-cw" :size="16" />Restart bots</Button>
      </template>
    </PageHeader>

    <SearchField v-if="commands && commands.length > 6" v-model="search" label="Find a command or setting" placeholder="Find a command or setting" />

    <Alert v-if="error" variant="danger" :title="error">
      <template #actions><Button size="sm" variant="outline" @click="botId && load(botId)">Try again</Button></template>
    </Alert>
    <div v-else-if="commands === null" class="list">
      <Skeleton v-for="i in 6" :key="i" variant="block" height="64px" />
    </div>
    <EmptyState v-else-if="!commands.length" title="No commands" sub="This bot has no commands to set." />
    <EmptyState v-else-if="!shown.length" :title="`Nothing matches ${search}`" sub="Try another word." />
    <ul v-else class="list">
      <li v-for="c in shown" :key="c.commandName" class="item" :class="{ off: !c.enabled }">
        <button type="button" class="open" @click="edit(c)">
          <span class="name">{{ commandName(c.commandName) }}</span>
          <span class="meta">{{ c.settings.length ? `${c.settings.length} setting${c.settings.length === 1 ? '' : 's'}` : 'Nothing to set' }}</span>
          <Icon name="chevron-right" :size="16" class="chev" />
        </button>
        <Toggle :model-value="c.enabled" :label="`${commandName(c.commandName)} on`" @update:model-value="v => setOn(c, v)" />
      </li>
    </ul>

    <CommandEditor
      :command="editing"
      :saving="saving"
      :error="saveError"
      :channels="channels"
      :categories="categories"
      :roles="roles"
      @close="editing = null"
      @save="save"
    />
    <ConfirmDialog
      :open="restartAsk"
      title="Restart every bot?"
      message="All of Airy's bots sign out and back in, so changes marked 'after a restart' apply. They are back in about a minute."
      confirm-text="Restart"
      @confirm="restart"
      @cancel="restartAsk = false"
    />
  </div>
</template>

<style scoped>
.page {
  display: grid;
  gap: var(--space-5);
}
.list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  gap: var(--space-2);
}
.item {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto;
  align-items: center;
  gap: var(--space-4);
  padding-right: var(--space-4);
  border: var(--border);
  border-radius: var(--r-2);
  background: var(--color-surface);
}
.open {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto auto;
  align-items: center;
  gap: var(--space-3);
  min-height: 56px;
  padding: var(--space-2) var(--space-4);
  border: 0;
  background: transparent;
  color: inherit;
  font: inherit;
  text-align: left;
  cursor: pointer;
  border-radius: var(--r-2) 0 0 var(--r-2);
}
.open:hover {
  background: var(--color-surface-hi);
}
.open:focus-visible {
  outline: 2px solid var(--color-accent);
  outline-offset: -2px;
}
.name {
  font-weight: 600;
  color: var(--color-fg);
}
.off .name {
  color: var(--color-text-dim);
  font-weight: 500;
}
.meta {
  color: var(--color-muted);
  font-size: var(--font-size-sm);
}
.chev {
  color: var(--color-muted);
}
</style>
