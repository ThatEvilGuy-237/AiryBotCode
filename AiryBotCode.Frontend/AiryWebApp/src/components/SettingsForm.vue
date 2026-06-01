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
      <div class="form-row">
        <div class="form-group">
          <label for="maxTokens">Max Tokens</label>
          <input id="maxTokens" type="number" min="0" v-model.number="bot.maxTokens" />
        </div>
        <div class="form-group">
          <label for="retryAttempts">Retry Attempts</label>
          <input id="retryAttempts" type="number" min="0" v-model.number="bot.retryAttempts" />
        </div>
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
      <div class="form-group">
        <label for="verifyLogChannelId">Verify Log Channel</label>
        <input id="verifyLogChannelId" type="text" inputmode="numeric" v-model="bot.verifyLogChannelId" />
      </div>
      <div class="form-group">
        <label for="rulesChannelId">Rules Channel</label>
        <input id="rulesChannelId" type="text" inputmode="numeric" v-model="bot.rulesChannelId" />
      </div>
      <div class="form-group">
        <label for="giveawayScoreboardChannelId">Giveaway Scoreboard Channel</label>
        <input
          id="giveawayScoreboardChannelId"
          type="text"
          inputmode="numeric"
          v-model="bot.giveawayScoreboardChannelId"
        />
      </div>
      <div class="form-group">
        <label for="listenChannelIds">Listen Channels</label>
        <input id="listenChannelIds" type="text" v-model="bot.listenChannelIds" />
        <small>Comma-separated channel IDs Airy listens in.</small>
      </div>
    </section>

    <section class="group">
      <h3>Roles</h3>
      <div class="form-group">
        <label for="adminRoleIds">Admin Roles</label>
        <input id="adminRoleIds" type="text" v-model="bot.adminRoleIds" />
        <small>Comma-separated role IDs allowed to run admin commands.</small>
      </div>
      <div class="form-group">
        <label for="verifiedRoleId">Verified Role</label>
        <input id="verifiedRoleId" type="text" inputmode="numeric" v-model="bot.verifiedRoleId" />
      </div>
      <div class="form-group">
        <label for="unverifiedRoleId">Unverified Role</label>
        <input id="unverifiedRoleId" type="text" inputmode="numeric" v-model="bot.unverifiedRoleId" />
      </div>
    </section>

    <section class="group">
      <h3>Identity &amp; Categories</h3>
      <div class="form-group">
        <label for="ownerId">Owner ID</label>
        <input id="ownerId" type="text" inputmode="numeric" v-model="bot.ownerId" />
      </div>
      <div class="form-group">
        <label for="evilId">Evil ID</label>
        <input id="evilId" type="text" inputmode="numeric" v-model="bot.evilId" />
      </div>
      <div class="form-group">
        <label for="contactCategoryId">Contact Category</label>
        <input id="contactCategoryId" type="text" inputmode="numeric" v-model="bot.contactCategoryId" />
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
  background-color: var(--card-background);
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

.group h3 {
  margin-bottom: 1rem;
  font-size: 1.1rem;
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
  color: #374151;
}

input[type='text'],
input[type='number'],
input[type='password'],
textarea {
  width: 100%;
  padding: 0.6rem;
  border: 1px solid var(--input-border);
  border-radius: 6px;
  font: inherit;
}

input:focus,
textarea:focus {
  outline: none;
  border-color: var(--primary-color);
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
  background-color: var(--primary-color);
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 0.75rem 1.75rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
}

.save-btn:hover:not(:disabled) {
  background-color: var(--primary-hover);
}

.save-btn:disabled {
  background-color: #9bbbe0;
  cursor: not-allowed;
}
</style>
