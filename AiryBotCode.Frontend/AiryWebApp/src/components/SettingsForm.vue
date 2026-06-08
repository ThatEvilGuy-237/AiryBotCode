<script setup lang="ts">
import type { BotSetting } from '../types/botSetting'

// `bot` is the parent's working draft. Inputs bind straight onto its fields with
// v-model; the parent owns persistence and reacts to the `save` event.
defineProps<{
  bot: BotSetting
  saving: boolean
}>()

const emit = defineEmits<{
  (e: 'save'): void
}>()
</script>

<template>
  <form class="settings-form" @submit.prevent="emit('save')">
    <section class="group">
      <h3>General</h3>
      <div class="form-group">
        <label for="botName">Bot Name</label>
        <input id="botName" type="text" v-model="bot.botName" />
      </div>
      <label class="check">
        <input type="checkbox" v-model="bot.enabled" />
        <span>Enabled</span>
      </label>
    </section>

    <section class="group">
      <h3>AI</h3>
      <div class="form-group">
        <label for="openAIModel">Model</label>
        <input id="openAIModel" type="text" v-model="bot.openAIModel" />
      </div>
      <div class="form-group">
        <label for="openAIPrompt">System Prompt</label>
        <textarea id="openAIPrompt" rows="4" v-model="bot.openAIPrompt"></textarea>
      </div>
    </section>

    <section class="group">
      <h3>Channels</h3>
      <p class="hint">Discord channel IDs.</p>
      <div class="form-group">
        <label for="logChannelId">Log Channel</label>
        <input id="logChannelId" type="text" inputmode="numeric" v-model="bot.logChannelId" />
      </div>
      <div class="form-group">
        <label for="evilLogChannelId">Evil Log Channel</label>
        <input id="evilLogChannelId" type="text" inputmode="numeric" v-model="bot.evilLogChannelId" />
      </div>
    </section>

    <section class="group">
      <h3>Roles</h3>
      <div class="form-group">
        <label for="adminRoleIds">Admin Roles</label>
        <input id="adminRoleIds" type="text" v-model="bot.adminRoleIds" />
        <small>Comma-separated role IDs allowed to run admin commands.</small>
      </div>
    </section>

    <section class="group">
      <h3>Identity</h3>
      <div class="form-group">
        <label for="evilId">Evil ID</label>
        <input id="evilId" type="text" inputmode="numeric" v-model="bot.evilId" />
      </div>
    </section>

    <section class="group">
      <h3>Token</h3>
      <div class="form-group">
        <label for="token">Bot Token</label>
        <input
          id="token"
          type="password"
          autocomplete="off"
          v-model="bot.token"
          :placeholder="bot.hasToken ? '•••••••• (stored — leave blank to keep)' : 'No token stored'"
        />
        <small>For security the current token is never shown. Leave blank to keep it unchanged.</small>
      </div>
    </section>

    <div class="form-actions">
      <button type="submit" class="save-btn" :disabled="saving">
        {{ saving ? 'Saving…' : 'Save Settings' }}
      </button>
    </div>
  </form>
</template>

<style scoped>
.settings-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.group {
  background-color: var(--surface);
  border: 1px solid var(--border-color);
  border-radius: 14px;
  padding: 1.5rem;
  box-shadow: 0 4px 16px var(--shadow);
}

.group h3 {
  margin-bottom: 1rem;
  font-size: 1.15rem;
  color: var(--foxfire);
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.group h3::before {
  content: '';
  width: 6px;
  height: 18px;
  border-radius: 3px;
  background: linear-gradient(180deg, var(--foxfire), var(--violet));
}

.hint {
  color: var(--muted-color);
  font-size: 0.85rem;
  margin: -0.5rem 0 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  margin-bottom: 1rem;
}

.form-group:last-child {
  margin-bottom: 0;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

label {
  font-weight: 500;
  color: var(--text-color);
}

input[type='text'],
input[type='number'],
input[type='password'],
textarea {
  width: 100%;
  padding: 0.6rem 0.7rem;
  border: 1px solid var(--input-border);
  border-radius: 8px;
  font: inherit;
  color: var(--text-color);
  background: var(--surface-2);
  transition: border-color 0.15s ease, box-shadow 0.15s ease;
}

input::placeholder {
  color: var(--muted-color);
}

input:focus,
textarea:focus {
  outline: none;
  border-color: var(--foxfire);
  box-shadow: 0 0 0 3px rgba(232, 70, 122, 0.16);
}

textarea {
  resize: vertical;
  min-height: 90px;
}

small {
  color: var(--muted-color);
  font-size: 0.8rem;
}

.check {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 500;
}

.check input {
  width: 18px;
  height: 18px;
}

.form-actions {
  position: sticky;
  bottom: 0;
}

.save-btn {
  background: linear-gradient(90deg, var(--foxfire), var(--foxfire-deep));
  color: #fff;
  border: none;
  border-radius: 10px;
  padding: 0.75rem 1.85rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 2px 8px var(--shadow);
  transition: transform 0.15s ease, filter 0.2s ease;
}

.save-btn:hover:not(:disabled) {
  filter: brightness(1.05);
  transform: translateY(-1px);
}

.save-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
