<script setup lang="ts">
import { ref } from 'vue'
import type { CommandConfig } from '../lib/api'

const props = defineProps<{ command: CommandConfig }>()
const emit = defineEmits<{
  (e: 'save', settings: { key: string; value: string }[]): void
  (e: 'close'): void
}>()

// Local editable clone so edits don't mutate the parent until saved.
const draft = ref<CommandConfig>(JSON.parse(JSON.stringify(props.command)))
const saving = ref(false)

function save() {
  saving.value = true
  emit(
    'save',
    draft.value.settings.map((s) => ({ key: s.key, value: s.value })),
  )
}
</script>

<template>
  <div class="overlay" @click.self="emit('close')">
    <div class="modal" role="dialog" aria-modal="true">
      <header class="modal-header">
        <h2>{{ draft.commandName }}</h2>
        <button class="close-btn" aria-label="Close" @click="emit('close')">✕</button>
      </header>

      <div class="modal-body">
        <div v-for="setting in draft.settings" :key="setting.key" class="field">
          <div class="label-row">
            <label :for="`f-${setting.key}`">{{ setting.key }}</label>
            <span class="badge" :class="{ reloadable: setting.isReloadable }">
              {{ setting.isReloadable ? 'auto-applies' : 'needs reload' }}
            </span>
          </div>

          <textarea
            v-if="setting.uiHint === 'textarea' || setting.uiHint === 'json'"
            :id="`f-${setting.key}`"
            v-model="setting.value"
            rows="4"
            :class="{ mono: setting.uiHint === 'json' }"
          ></textarea>
          <input
            v-else-if="setting.uiHint === 'number'"
            :id="`f-${setting.key}`"
            v-model="setting.value"
            type="number"
          />
          <label v-else-if="setting.uiHint === 'boolean'" class="switch">
            <input
              type="checkbox"
              :checked="setting.value === 'true'"
              @change="setting.value = ($event.target as HTMLInputElement).checked ? 'true' : 'false'"
            />
            <span>{{ setting.value === 'true' ? 'Enabled' : 'Disabled' }}</span>
          </label>
          <input v-else :id="`f-${setting.key}`" v-model="setting.value" type="text" />

          <p class="help">{{ setting.description }}</p>
        </div>

        <p v-if="draft.settings.length === 0" class="empty">No configurable settings.</p>
      </div>

      <footer class="modal-footer">
        <button class="cancel-btn" @click="emit('close')">Cancel</button>
        <button class="save-btn" :disabled="saving" @click="save">
          {{ saving ? 'Saving…' : 'Save' }}
        </button>
      </footer>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  background: rgba(58, 44, 54, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1.5rem;
  z-index: 100;
}
.modal {
  background: var(--surface-2, #fff);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  width: 100%;
  max-width: 560px;
  max-height: 85vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 24px 60px rgba(0, 0, 0, 0.22);
}
.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1.5rem 0.75rem;
}
.modal-header h2 {
  margin: 0;
  font-size: 1.25rem;
  color: var(--foxfire-deep);
}
.close-btn {
  background: transparent;
  border: none;
  font-size: 1.1rem;
  cursor: pointer;
  color: var(--muted-color);
  padding: 0.25rem 0.5rem;
  border-radius: 8px;
}
.close-btn:hover { background: var(--surface-hover); }
.modal-body {
  padding: 0.5rem 1.5rem 1rem;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 1.1rem;
}
.field { display: flex; flex-direction: column; gap: 0.4rem; }
.label-row { display: flex; align-items: center; justify-content: space-between; gap: 0.5rem; }
label { font-weight: 600; color: var(--text-color); }
.badge {
  font-size: 0.68rem;
  font-weight: 700;
  padding: 0.12rem 0.5rem;
  border-radius: 999px;
  background: var(--surface-hover);
  color: var(--muted-color);
  white-space: nowrap;
}
.badge.reloadable { background: #e7f6ec; color: #1f9254; }
input[type='text'],
input[type='number'],
textarea {
  width: 100%;
  box-sizing: border-box;
  padding: 0.6rem;
  border: 1px solid var(--input-border, #ecc0cf);
  border-radius: 8px;
  font-size: 16px;
  background: #fff;
}
textarea { resize: vertical; min-height: 70px; }
textarea.mono { font-family: ui-monospace, Menlo, monospace; font-size: 0.85rem; }
.switch { display: flex; align-items: center; gap: 0.5rem; font-weight: 400; }
.switch input { width: 20px; height: 20px; }
.help { margin: 0; color: var(--muted-color); font-size: 0.8rem; }
.empty { color: var(--muted-color); font-style: italic; }
.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid var(--border-color);
}
.cancel-btn {
  background: #fff;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 0.6rem 1.1rem;
  cursor: pointer;
  color: var(--text-color);
}
.save-btn {
  background: var(--foxfire);
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 0.6rem 1.4rem;
  font-weight: 600;
  cursor: pointer;
}
.save-btn:hover { background: var(--foxfire-deep); }
.save-btn:disabled { opacity: 0.6; cursor: default; }

@media (max-width: 768px) {
  .overlay { padding: 0; align-items: stretch; }
  .modal { max-width: 100%; height: 100dvh; max-height: 100dvh; border-radius: 0; }
  .modal-footer {
    position: sticky;
    bottom: 0;
    background: var(--surface-2, #fff);
    padding-bottom: calc(1rem + env(safe-area-inset-bottom));
  }
  .save-btn, .cancel-btn { min-height: 48px; }
}
</style>
