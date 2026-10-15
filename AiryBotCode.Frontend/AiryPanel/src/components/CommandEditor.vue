<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { Alert, Badge, Button, Drawer, Field } from '@airy/ui'
import SettingControl from '@/components/SettingControl.vue'
import type { DiscordEntity } from '@/lib/api'
import { changed, commandName, groups, settingName, type Command } from '@/lib/commands'

const props = defineProps<{
  command: Command | null
  saving: boolean
  error: string | null
  channels: DiscordEntity[]
  categories: DiscordEntity[]
  roles: DiscordEntity[]
}>()
const emit = defineEmits<{ close: []; save: [values: { key: string; value: string }[]] }>()

const draft = ref<Record<string, string>>({})
const open = computed(() => props.command !== null)
const sections = computed(() => (props.command ? groups(props.command.settings) : []))
const edits = computed(() => (props.command ? changed(props.command.settings, draft.value) : []))
const needsRestart = computed(() => edits.value.some(e => !props.command?.settings.find(s => s.key === e.key)?.isReloadable))

watch(() => props.command, c => {
  draft.value = Object.fromEntries((c?.settings ?? []).map(s => [s.key, s.value]))
}, { immediate: true })
</script>

<template>
  <Drawer :open="open" :title="command ? commandName(command.commandName) : ''" :width="560" @close="emit('close')">
    <form v-if="command" class="editor" @submit.prevent="emit('save', edits)">
      <p v-if="!command.settings.length" class="none">This command has nothing to set.</p>
      <section v-for="g in sections" :key="g.name" class="group" :aria-label="g.name">
        <h3 v-if="sections.length > 1">{{ g.name }}</h3>
        <Field v-for="s in g.settings" :key="s.key" :label="settingName(s.key)" :hint="s.description ?? undefined">
          <template v-if="!s.isReloadable" #aside>
            <Badge tone="muted" outline>after a restart</Badge>
          </template>
          <SettingControl
            v-model="draft[s.key]"
            :setting="s"
            :label="settingName(s.key)"
            :channels="channels"
            :categories="categories"
            :roles="roles"
          />
        </Field>
      </section>
      <Alert v-if="error" variant="danger" :title="error" dense />
    </form>
    <template #footer>
      <span class="state">{{ edits.length ? `${edits.length} changed${needsRestart ? ', a restart applies some' : ''}` : 'No changes' }}</span>
      <Button variant="ghost" @click="emit('close')">Cancel</Button>
      <Button :loading="saving" :disabled="!edits.length" @click="emit('save', edits)">Save</Button>
    </template>
  </Drawer>
</template>

<style scoped>
.editor {
  display: grid;
  gap: var(--space-6);
}
.group {
  display: grid;
  gap: var(--space-4);
}
h3 {
  margin: 0;
  padding-bottom: var(--space-2);
  border-bottom: var(--border);
  font-size: var(--font-size-base);
  color: var(--color-fg);
}
.none {
  margin: 0;
  color: var(--color-text-dim);
}
.state {
  flex: 1;
  color: var(--color-muted);
  font-size: var(--font-size-sm);
}
</style>
