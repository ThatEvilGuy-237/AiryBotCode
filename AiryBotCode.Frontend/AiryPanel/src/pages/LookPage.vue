<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { Alert, Button, Card, ConfirmDialog, Icon, PageHeader, Skeleton, useToast } from '@airy/ui'
import { api, ApiError, type BotLook } from '@/lib/api'
import { bot, botId, botImage } from '@/lib/bots'
import { downscaleDataUrl, extractPalette, pickPrimaryAccent, type Swatch } from '@/lib/palette'

const toast = useToast()

const look = ref<BotLook | null>(null)
const error = ref<string | null>(null)
const picked = ref<string | null>(null)
const swatches = ref<Swatch[]>([])
const primary = ref('')
const accent = ref('')
const reading = ref(false)
const saving = ref(false)
const avatarBusy = ref(false)
const resetAsk = ref(false)
const dragging = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)

const shownPrimary = computed(() => (picked.value ? primary.value : look.value?.primary) || null)
const shownAccent = computed(() => (picked.value ? accent.value : look.value?.accent) || null)
const shownImage = computed(() => picked.value ?? look.value?.image ?? null)

async function load(id: string) {
  look.value = null
  error.value = null
  picked.value = null
  try {
    const l = await api.look(id)
    if (botId.value === id) look.value = l
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'The look could not be loaded.'
  }
}

watch(botId, id => { if (id) load(id) }, { immediate: true })

function readDataUrl(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.onload = () => resolve(String(reader.result))
    reader.onerror = () => reject(reader.error)
    reader.readAsDataURL(file)
  })
}

async function take(file: File | undefined) {
  if (!file) return
  if (!file.type.startsWith('image/')) {
    toast.error('That file is not an image.')
    return
  }
  reading.value = true
  try {
    const [url, palette] = await Promise.all([readDataUrl(file), extractPalette(file, 6)])
    picked.value = await downscaleDataUrl(url, 256)
    swatches.value = palette
    const pick = pickPrimaryAccent(palette)
    primary.value = pick.primary
    accent.value = pick.accent
  } catch {
    toast.error('The image could not be read.')
  } finally {
    reading.value = false
  }
}

function onDrop(event: DragEvent) {
  dragging.value = false
  take(event.dataTransfer?.files?.[0])
}

async function save() {
  if (!botId.value || !picked.value) return
  saving.value = true
  try {
    await api.saveLook(botId.value, { primary: primary.value, accent: accent.value, image: picked.value })
    look.value = { primary: primary.value, accent: accent.value, image: picked.value }
    botImage.value = picked.value
    picked.value = null
    toast.action('Look saved', { label: 'Use as Discord avatar', run: useAvatar }, 'success')
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Saving failed.')
  } finally {
    saving.value = false
  }
}

async function useAvatar() {
  if (!botId.value) return
  avatarBusy.value = true
  try {
    await api.useAvatar(botId.value)
    toast.success('The bot takes the picture as its avatar within a few seconds')
  } catch (e) {
    toast.error(e instanceof ApiError && e.status === 400 ? 'Save a picture first.' : e instanceof Error ? e.message : 'The avatar could not be asked.')
  } finally {
    avatarBusy.value = false
  }
}

async function reset() {
  resetAsk.value = false
  if (!botId.value) return
  try {
    await api.saveLook(botId.value, { primary: '', accent: '', image: null })
    look.value = { primary: null, accent: null, image: null }
    botImage.value = null
    toast.success('Look reset')
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Resetting failed.')
  }
}
</script>

<template>
  <div class="page">
    <PageHeader title="Look" subtitle="// the bot's picture and the colour of its messages" />

    <Alert v-if="error" variant="danger" :title="error">
      <template #actions><Button size="sm" variant="outline" @click="botId && load(botId)">Try again</Button></template>
    </Alert>
    <Skeleton v-else-if="!look" variant="block" height="220px" />
    <template v-else>
      <Card>
        <template #head><h2>{{ picked ? 'How it will look' : 'Now' }}</h2></template>
        <div class="now">
          <span class="avatar" aria-hidden="true">
            <img v-if="shownImage" :src="shownImage" alt="" />
            <Icon v-else name="image" :size="28" />
          </span>
          <div class="embed" :style="{ '--embed': shownPrimary ?? 'var(--color-border-hi)' }">
            <strong>{{ bot?.botName }}</strong>
            <span>This bot's messages carry this colour on their left edge.</span>
          </div>
        </div>
        <dl class="colours">
          <div><dt>Message colour</dt><dd><span class="dot" :style="{ background: shownPrimary ?? 'transparent' }" /><span class="mono">{{ shownPrimary ?? 'none' }}</span></dd></div>
          <div><dt>Second colour</dt><dd><span class="dot" :style="{ background: shownAccent ?? 'transparent' }" /><span class="mono">{{ shownAccent ?? 'none' }}</span></dd></div>
        </dl>
        <div v-if="!picked" class="actions">
          <Button variant="outline" :loading="avatarBusy" :disabled="!look.image" @click="useAvatar"><Icon name="image" :size="16" />Use as Discord avatar</Button>
          <Button v-if="look.image || look.primary" variant="ghost" @click="resetAsk = true">Reset the look</Button>
        </div>
      </Card>

      <Card>
        <template #head><h2>{{ picked ? 'Pick the colours' : 'New picture' }}</h2></template>
        <label
          v-if="!picked"
          class="drop"
          :class="{ over: dragging }"
          @dragover.prevent="dragging = true"
          @dragleave="dragging = false"
          @drop.prevent="onDrop"
        >
          <Icon name="download" :size="22" />
          <span>{{ reading ? 'Reading the picture…' : 'Drop a picture here, or choose one' }}</span>
          <input ref="fileInput" class="file" type="file" accept="image/*" @change="take(($event.target as HTMLInputElement).files?.[0])" />
        </label>
        <div v-else class="choose">
          <fieldset>
            <legend>Message colour</legend>
            <div class="swatches">
              <button v-for="s in swatches" :key="`p${s.hex}`" type="button" class="swatch" :style="{ background: s.hex }" :aria-label="`Message colour ${s.hex}`" :aria-pressed="primary === s.hex" @click="primary = s.hex" />
            </div>
          </fieldset>
          <fieldset>
            <legend>Second colour</legend>
            <div class="swatches">
              <button v-for="s in swatches" :key="`a${s.hex}`" type="button" class="swatch" :style="{ background: s.hex }" :aria-label="`Second colour ${s.hex}`" :aria-pressed="accent === s.hex" @click="accent = s.hex" />
            </div>
          </fieldset>
          <div class="actions">
            <Button :loading="saving" @click="save">Save the look</Button>
            <Button variant="ghost" @click="picked = null">Cancel</Button>
          </div>
        </div>
      </Card>
    </template>

    <ConfirmDialog
      :open="resetAsk"
      title="Reset the look?"
      message="The picture and both colours are removed from the panel. The bot's current Discord avatar stays until you set a new one."
      confirm-text="Reset"
      danger
      @confirm="reset"
      @cancel="resetAsk = false"
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
.now {
  display: grid;
  grid-template-columns: 96px minmax(0, 1fr);
  align-items: center;
  gap: var(--space-5);
}
.avatar {
  width: 96px;
  height: 96px;
  border-radius: 50%;
  overflow: hidden;
  display: grid;
  place-items: center;
  background: var(--color-surface-hi);
  color: var(--color-muted);
}
.avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.embed {
  display: grid;
  gap: var(--space-1);
  padding: var(--space-3) var(--space-4);
  border-radius: var(--r-1);
  background: var(--color-surface-mute);
  box-shadow: inset 4px 0 0 var(--embed);
  color: var(--color-text-dim);
}
.embed strong {
  color: var(--color-fg);
}
.colours {
  display: flex;
  flex-wrap: wrap;
  gap: var(--space-6);
  margin: var(--space-4) 0 0;
}
.colours div {
  display: grid;
  gap: var(--space-1);
}
dt {
  font-size: var(--font-size-sm);
  color: var(--color-text-dim);
}
dd {
  margin: 0;
  display: flex;
  align-items: center;
  gap: var(--space-2);
}
.dot {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  border: var(--border);
}
.mono {
  font-family: var(--font-mono);
  font-size: var(--font-size-sm);
  color: var(--color-fg);
}
.actions {
  display: flex;
  flex-wrap: wrap;
  gap: var(--space-2);
  margin-top: var(--space-4);
}
.drop {
  display: grid;
  justify-items: center;
  gap: var(--space-2);
  padding: var(--space-6);
  border: 1px dashed var(--color-border-hi);
  border-radius: var(--r-2);
  color: var(--color-text-dim);
  cursor: pointer;
  text-align: center;
}
.drop.over,
.drop:hover {
  border-color: var(--color-accent);
  color: var(--color-fg);
}
.drop:focus-within {
  outline: 2px solid var(--color-accent);
  outline-offset: 2px;
}
.file {
  position: absolute;
  width: 1px;
  height: 1px;
  opacity: 0;
}
.choose {
  display: grid;
  gap: var(--space-4);
}
fieldset {
  margin: 0;
  padding: 0;
  border: 0;
  display: grid;
  gap: var(--space-2);
}
legend {
  margin-bottom: var(--space-2);
  font-size: var(--font-size-sm);
  color: var(--color-text-dim);
}
.swatches {
  display: flex;
  flex-wrap: wrap;
  gap: var(--space-2);
}
.swatch {
  width: 44px;
  height: 44px;
  border-radius: var(--r-2);
  border: 2px solid transparent;
  cursor: pointer;
  box-shadow: inset 0 0 0 1px var(--color-border);
}
.swatch[aria-pressed='true'] {
  box-shadow: inset 0 0 0 1px var(--color-border), 0 0 0 2px var(--color-bg), 0 0 0 4px var(--color-fg);
}
.swatch:focus-visible {
  outline: 2px solid var(--color-accent);
  outline-offset: 2px;
}
@media (max-width: 520px) {
  .now {
    grid-template-columns: 64px minmax(0, 1fr);
  }
  .avatar {
    width: 64px;
    height: 64px;
  }
}
</style>
