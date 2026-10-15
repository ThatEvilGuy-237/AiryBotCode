<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Alert, Button, Card, ConfirmDialog, CopyButton, Field, Icon, Modal, MultiSelect, PageHeader, Select, TextField, Toggle, useToast } from '@airy/ui'
import ChannelPicker from '@/components/ChannelPicker.vue'
import { api, ApiError, type Bot, type DiscordEntity } from '@/lib/api'
import { bot, botId, loadBots, pickBot } from '@/lib/bots'
import { parseIdList } from '@/lib/discordIds'

const toast = useToast()
const route = useRoute()
const router = useRouter()

const draft = ref<Bot | null>(null)
const token = ref('')
const saving = ref(false)
const saveError = ref<string | null>(null)
const channels = ref<DiscordEntity[]>([])
const roles = ref<DiscordEntity[]>([])
const databases = ref<string[]>([])

const revealOpen = ref(false)
const revealPassword = ref('')
const revealing = ref(false)
const revealError = ref<string | null>(null)
const revealed = ref<string | null>(null)

const dropAsk = ref(false)
const addOpen = ref(route.query.new === '1')
const fresh = ref({ botId: '', botName: '', token: '' })
const adding = ref(false)
const addError = ref<string | null>(null)

const dirty = computed(() => {
  if (!draft.value || !bot.value) return false
  const a = draft.value
  const b = bot.value
  return token.value.trim() !== '' || a.botName !== b.botName || a.enabled !== b.enabled || a.logChannelId !== b.logChannelId
    || a.evilLogChannelId !== b.evilLogChannelId || a.evilId !== b.evilId || (a.adminRoleIds ?? '') !== (b.adminRoleIds ?? '')
    || (a.databaseName ?? '') !== (b.databaseName ?? '')
})

const many = (list: DiscordEntity[]) => new Set(list.map(e => e.guild)).size > 1
const options = (list: DiscordEntity[], keep: string[]) => [
  ...list.map(e => ({ label: many(list) ? `${e.name} · ${e.guild}` : e.name, value: e.id })),
  ...keep.filter(id => id && id !== '0' && !list.some(e => e.id === id)).map(id => ({ label: `Unknown (${id})`, value: id })),
]
const adminIds = computed({
  get: () => parseIdList(draft.value?.adminRoleIds ?? ''),
  set: (ids: string[]) => { if (draft.value) draft.value.adminRoleIds = ids.join(',') },
})
const roleOptions = computed(() => options(roles.value, adminIds.value))
const databaseOptions = computed(() => [{ label: 'The shared database', value: '' }, ...databases.value.map(d => ({ label: d, value: d }))])

function reset() {
  draft.value = bot.value ? { ...bot.value, logChannelId: bot.value.logChannelId || '0', evilLogChannelId: bot.value.evilLogChannelId || '0', databaseName: bot.value.databaseName ?? '' } : null
  token.value = ''
  saveError.value = null
  revealed.value = null
}

watch(bot, reset, { immediate: true })
watch(botId, async id => {
  if (!id) return
  const [c, r, d] = await Promise.allSettled([api.discordChannels(id), api.discordRoles(id), api.databases()])
  if (botId.value !== id) return
  channels.value = c.status === 'fulfilled' ? c.value : []
  roles.value = r.status === 'fulfilled' ? r.value : []
  databases.value = d.status === 'fulfilled' ? d.value : []
}, { immediate: true })

async function save() {
  if (!draft.value) return
  saving.value = true
  saveError.value = null
  try {
    await api.saveBot({ ...draft.value, token: token.value.trim() || undefined })
    await loadBots(true)
    toast.success(`${draft.value.botName} saved`)
  } catch (e) {
    saveError.value = e instanceof Error ? e.message : 'Saving failed.'
  } finally {
    saving.value = false
  }
}

async function reveal() {
  if (!botId.value) return
  revealing.value = true
  revealError.value = null
  try {
    revealed.value = (await api.revealToken(botId.value, revealPassword.value)).token
    revealOpen.value = false
    revealPassword.value = ''
  } catch (e) {
    revealError.value = e instanceof ApiError && e.status === 403 ? 'That password is not right.' : e instanceof Error ? e.message : 'The token could not be shown.'
  } finally {
    revealing.value = false
  }
}

async function drop() {
  dropAsk.value = false
  const gone = bot.value
  if (!gone) return
  try {
    await api.dropBot(gone.botId)
    localStorage.removeItem('airy.bot')
    await loadBots(true)
    toast.success(`${gone.botName} removed`)
    router.push('/')
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Removing failed.')
  }
}

async function add() {
  adding.value = true
  addError.value = null
  try {
    const created = await api.addBot({
      botId: fresh.value.botId.trim(), botName: fresh.value.botName.trim(), token: fresh.value.token.trim(), enabled: true,
      adminRoleIds: '', evilId: '0', logChannelId: '0', evilLogChannelId: '0', hasToken: true, databaseName: null,
    })
    await loadBots(true)
    pickBot(created.botId)
    addOpen.value = false
    fresh.value = { botId: '', botName: '', token: '' }
    toast.success(`${created.botName} added; restart the bots to sign it in`)
    router.replace('/settings')
  } catch (e) {
    addError.value = e instanceof ApiError && e.status === 409 ? 'A bot with this id exists already.' : e instanceof Error ? e.message : 'Adding failed.'
  } finally {
    adding.value = false
  }
}
</script>

<template>
  <div class="page">
    <PageHeader title="Settings" :subtitle="bot ? `// ${bot.botName}` : '// the bot'">
      <template #actions>
        <Button variant="outline" @click="addOpen = true"><Icon name="plus" :size="16" />Add a bot</Button>
      </template>
    </PageHeader>

    <form v-if="draft" class="form" @submit.prevent="save">
      <Card>
        <template #head><h2>Bot</h2></template>
        <div class="fields">
          <Field label="Name"><TextField v-model="draft.botName" autocomplete="off" /></Field>
          <div class="toggle-row">
            <Toggle v-model="draft.enabled" labelledby="bot-on" />
            <span id="bot-on">{{ draft.enabled ? 'On: the bot signs in to Discord' : 'Off: the bot stays signed out' }}</span>
          </div>
          <div class="id-row">
            <span class="label">Discord application id</span>
            <span class="mono">{{ draft.botId }}</span>
            <CopyButton :value="draft.botId" label="Copy" size="sm" />
          </div>
        </div>
      </Card>

      <Card>
        <template #head><h2>Discord token</h2></template>
        <div class="fields">
          <Field label="New token" :hint="bot?.hasToken ? 'Leave empty to keep the stored token' : 'The bot has no token yet'">
            <TextField v-model="token" type="password" autocomplete="off" />
          </Field>
          <div v-if="revealed" class="revealed">
            <span class="mono secret">{{ revealed }}</span>
            <CopyButton :value="revealed" label="Copy" size="sm" />
            <Button variant="ghost" size="sm" @click="revealed = null">Hide</Button>
          </div>
          <div v-else-if="bot?.hasToken"><Button variant="ghost" size="sm" @click="revealOpen = true"><Icon name="eye" :size="16" />Show the stored token</Button></div>
        </div>
      </Card>

      <Card>
        <template #head><h2>Logs and admins</h2></template>
        <div class="fields two">
          <Field label="Log channel" hint="Where the bot reports what it did">
            <ChannelPicker :model-value="draft.logChannelId" :channels="channels" none @update:model-value="v => draft && (draft.logChannelId = v ?? '0')" />
          </Field>
          <Field label="Owner log channel" hint="Where the owner gets the private reports">
            <ChannelPicker :model-value="draft.evilLogChannelId" :channels="channels" none @update:model-value="v => draft && (draft.evilLogChannelId = v ?? '0')" />
          </Field>
          <Field label="Admin roles" hint="Members with these roles may use the admin commands">
            <MultiSelect v-model="adminIds" :options="roleOptions" placeholder="No admin roles" clearable />
          </Field>
          <Field label="Owner's Discord user id" hint="The account that gets the owner reports">
            <TextField v-model="draft.evilId" autocomplete="off" />
          </Field>
          <Field label="Database" hint="Where this bot keeps its own data">
            <Select :model-value="draft.databaseName ?? ''" :options="databaseOptions" @update:model-value="v => draft && (draft.databaseName = v || null)" />
          </Field>
        </div>
      </Card>

      <div class="bar">
        <Alert v-if="saveError" variant="danger" :title="saveError" dense />
        <span class="spacer" />
        <Button variant="ghost" :disabled="!dirty" @click="reset">Undo changes</Button>
        <Button type="submit" :loading="saving" :disabled="!dirty">Save</Button>
      </div>

      <Card class="danger">
        <template #head><h2>Remove this bot</h2></template>
        <div class="drop">
          <p>The bot leaves the panel with its commands, channels and look. Its Discord application stays.</p>
          <Button variant="danger" @click="dropAsk = true">Remove {{ bot?.botName }}</Button>
        </div>
      </Card>
    </form>

    <Modal :open="revealOpen" title="Show the stored token" size="sm" @close="revealOpen = false">
      <form class="modal-form" @submit.prevent="reveal">
        <Field label="Access password" :error="revealError ?? undefined">
          <TextField v-model="revealPassword" type="password" autocomplete="current-password" />
        </Field>
      </form>
      <template #footer>
        <Button variant="ghost" @click="revealOpen = false">Cancel</Button>
        <Button :loading="revealing" :disabled="!revealPassword" @click="reveal">Show</Button>
      </template>
    </Modal>

    <Modal :open="addOpen" title="Add a bot" size="md" @close="addOpen = false; router.replace('/settings')">
      <form class="modal-form" @submit.prevent="add">
        <Field label="Discord application id" hint="From the Discord developer portal, General information">
          <TextField v-model="fresh.botId" autocomplete="off" />
        </Field>
        <Field label="Name"><TextField v-model="fresh.botName" autocomplete="off" /></Field>
        <Field label="Token" hint="From the developer portal, Bot, Reset token">
          <TextField v-model="fresh.token" type="password" autocomplete="off" />
        </Field>
        <Alert v-if="addError" variant="danger" :title="addError" dense />
      </form>
      <template #footer>
        <Button variant="ghost" @click="addOpen = false; router.replace('/settings')">Cancel</Button>
        <Button :loading="adding" :disabled="!/^\d{5,25}$/.test(fresh.botId.trim()) || !fresh.botName.trim() || !fresh.token.trim()" @click="add">Add bot</Button>
      </template>
    </Modal>

    <ConfirmDialog
      :open="dropAsk"
      :title="`Remove ${bot?.botName ?? 'this bot'}?`"
      message="Its commands, channels, Hive connection and look are deleted from the panel. This cannot be undone here."
      confirm-text="Remove"
      danger
      @confirm="drop"
      @cancel="dropAsk = false"
    />
  </div>
</template>

<style scoped>
.page,
.form {
  display: grid;
  gap: var(--space-5);
}
h2 {
  margin: 0;
  font-size: var(--font-size-lg);
  color: var(--color-fg);
}
.fields {
  display: grid;
  gap: var(--space-4);
}
.fields.two {
  grid-template-columns: repeat(2, minmax(0, 1fr));
}
.toggle-row,
.id-row,
.revealed {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: var(--space-2);
  color: var(--color-fg);
}
.label {
  color: var(--color-text-dim);
  font-size: var(--font-size-sm);
}
.mono {
  font-family: var(--font-mono);
  font-size: var(--font-size-sm);
  color: var(--color-fg);
}
.secret {
  overflow-wrap: anywhere;
}
.bar {
  position: sticky;
  bottom: 0;
  z-index: 2;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-3) 0;
  background: var(--color-bg);
  border-top: var(--border);
}
.spacer {
  flex: 1;
}
.drop {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-3);
}
.drop p {
  margin: 0;
  color: var(--color-text-dim);
}
.modal-form {
  display: grid;
  gap: var(--space-4);
}
@media (max-width: 760px) {
  .fields.two {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
