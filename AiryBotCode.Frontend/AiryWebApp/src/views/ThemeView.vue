<script setup lang="ts">
// The CORRECT theme page (Item E): upload an image → the WHOLE panel re-themes via
// the @hive/ui image-theme engine, it PERSISTS (survives reload), and the image is the
// panel profile picture. Still saves the derived accent as the bot's Discord-embed
// brand colour. Replaces the old per-bot 2-colour picker, which no longer drove anything.
import { ref, watch } from 'vue'
import { PageHeader, Card, Button, Badge, deriveThemeFromImage, applyTheme, type DerivedTheme } from '@hive/ui'
import { saveImageTheme, clearImageTheme, loadImageTheme, downscaleDataUrl } from '../lib/imageTheme'
import { extractPalette, pickPrimaryAccent } from '../lib/palette'
import { api, ApiError } from '../lib/api'
import { useBots } from '../lib/bots'

const { currentBot, currentBotId, applyUpsert } = useBots()

const imgSrc = ref<string | null>(null)
const theme = ref<DerivedTheme | null>(null)
const primaryHex = ref('')
const accentHex = ref('')

// Reflect the selected bot's saved theme — on first load and whenever the bot
// changes (the panel itself is already re-themed by App.vue's watcher). Show the
// local cache instantly, then pull the server copy (cross-device) if present.
watch(currentBotId, async (id) => {
  const stored = loadImageTheme(id)
  imgSrc.value = stored?.image ?? null
  theme.value = stored?.theme ?? null
  primaryHex.value = ''
  accentHex.value = ''
  if (!id) return
  try {
    const t = await api.getTheme(id)
    if (id === currentBotId.value && t.image && t.data) {
      imgSrc.value = t.image
      theme.value = JSON.parse(t.data) as DerivedTheme
    }
  } catch { /* offline / no server theme */ }
}, { immediate: true })

const busy = ref(false)
const saving = ref(false)
const error = ref('')
const success = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)

function readDataUrl(file: File): Promise<string> {
  return new Promise((res, rej) => { const r = new FileReader(); r.onload = () => res(r.result as string); r.onerror = rej; r.readAsDataURL(file) })
}
function loadImg(src: string): Promise<HTMLImageElement> {
  return new Promise((res, rej) => { const i = new Image(); i.onload = () => res(i); i.onerror = rej; i.src = src })
}

async function handleImage(file?: File) {
  if (!file) return
  busy.value = true; error.value = ''; success.value = false
  try {
    const dataUrl = await readDataUrl(file)
    imgSrc.value = dataUrl
    // Panel theme — re-themes every @hive/ui surface live.
    theme.value = deriveThemeFromImage(await loadImg(dataUrl))
    applyTheme(theme.value, document.documentElement)
    // Embed brand colour (a hex) for the bot's Discord embeds.
    try {
      const pick = pickPrimaryAccent(await extractPalette(file, 6))
      primaryHex.value = pick.primary; accentHex.value = pick.accent
    } catch { /* embed colour optional */ }
  } catch {
    error.value = 'Could not read that image.'
  } finally {
    busy.value = false
  }
}

function pick() { fileInput.value?.click() }
function onDrop(e: DragEvent) { handleImage(e.dataTransfer?.files?.[0]) }

async function save() {
  if (!imgSrc.value || !theme.value || !currentBotId.value) return
  saving.value = true; error.value = ''; success.value = false
  // Local cache (instant, survives reload) — image doubles as its profile pic.
  saveImageTheme(currentBotId.value, imgSrc.value, theme.value)
  // Server-side: a downscaled image + the derived theme JSON (so it follows
  // across devices) plus the bot's embed brand colour.
  try {
    const small = await downscaleDataUrl(imgSrc.value, 256)
    const updated = await api.setTheme(
      currentBotId.value,
      primaryHex.value || '',
      accentHex.value || primaryHex.value || '',
      small,
      JSON.stringify(theme.value),
    )
    applyUpsert(updated)
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Theme applied locally, but saving to the server failed.'
  }
  saving.value = false
  if (!error.value) { success.value = true; setTimeout(() => (success.value = false), 3000) }
}

function reset() {
  clearImageTheme(currentBotId.value)
  theme.value = null; imgSrc.value = null; primaryHex.value = ''; accentHex.value = ''
  if (currentBotId.value) api.setTheme(currentBotId.value, '', '').then(applyUpsert).catch(() => {})
}

const avatarBusy = ref(false)
const avatarMsg = ref('')
async function setAvatar() {
  if (!currentBotId.value || avatarBusy.value) return
  avatarBusy.value = true; avatarMsg.value = ''; error.value = ''
  try {
    await api.setBotAvatar(currentBotId.value)
    avatarMsg.value = 'Avatar update requested — the bot applies it within a few seconds (Discord limits avatar changes to ~2/hour).'
    setTimeout(() => (avatarMsg.value = ''), 6000)
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Could not request the avatar update.'
  } finally {
    avatarBusy.value = false
  }
}
</script>

<template>
  <div class="page">
    <PageHeader title="Theme" :subtitle="currentBot ? `Theme the panel from an image — saved for ${currentBot.botName} (switching bots re-themes)` : 'Select a bot to theme.'" />

    <p v-if="error" class="banner err">{{ error }}</p>
    <p v-if="success" class="banner ok">Theme saved.</p>
    <p v-if="avatarMsg" class="banner ok">{{ avatarMsg }}</p>

    <Card>
      <template #head>
        <img v-if="imgSrc" :src="imgSrc" class="profile" alt="theme source / profile" />
        <span v-else class="profile placeholder" aria-hidden="true">🦊</span>
        <h3>Panel theme</h3>
        <span class="hd-sub">// the image sets the accent + surface tint, and is the profile picture</span>
        <div style="flex: 1" />
        <Button @click="pick">{{ imgSrc ? 'Change image' : 'Upload image' }}</Button>
        <Button v-if="theme" variant="outline" :disabled="saving || !currentBotId" @click="save">{{ saving ? 'Saving…' : 'Save' }}</Button>
        <Button v-if="theme && currentBotId" variant="ghost" :disabled="avatarBusy" @click="setAvatar">{{ avatarBusy ? 'Requesting…' : 'Set as bot avatar' }}</Button>
        <Button v-if="theme" variant="ghost" @click="reset">Reset</Button>
        <input ref="fileInput" type="file" accept="image/*" hidden @change="handleImage(($event.target as HTMLInputElement).files?.[0])" />
      </template>

      <label class="drop" @dragover.prevent @drop.prevent="onDrop">
        <input type="file" accept="image/*" hidden @change="handleImage(($event.target as HTMLInputElement).files?.[0])" />
        <img v-if="imgSrc" :src="imgSrc" class="preview" alt="source" />
        <div v-else class="drop-hint"><div class="emoji">🖼️</div><p>Drop an image here, or click to choose</p></div>
      </label>

      <p v-if="busy" class="muted">Reading image…</p>
      <div v-else-if="theme" class="tokens">
        <span class="mono">accent {{ theme.accentHue }}° · L {{ theme.accentL }} · C {{ theme.accentC }}</span>
        <span class="mono">base {{ theme.baseHue }}° · tint × {{ theme.baseChroma }}</span>
        <Badge v-if="primaryHex">embed {{ primaryHex }}</Badge>
      </div>
      <p v-else class="muted">Upload an image — the whole panel re-themes from it. <strong>Save</strong> keeps it across reloads.</p>
    </Card>
  </div>
</template>

<style scoped>
.page { padding: 2rem; display: flex; flex-direction: column; gap: 1rem; max-width: 760px; }
.muted { color: var(--color-muted); }
.banner { border-radius: var(--radius); padding: .7rem 1rem; font-weight: 500; border: var(--border); }
.banner.ok { color: var(--color-ok); background: var(--color-surface-mute); border-color: var(--color-accent-edge); }
.banner.err { color: var(--color-danger); background: var(--color-surface-mute); }
.profile { width: 36px; height: 36px; border-radius: 50%; object-fit: cover; display: grid; place-items: center; }
.profile.placeholder { background: var(--color-surface-mute); border: var(--border); font-size: 1.1rem; }
.hd-sub { font-family: var(--font-mono); color: var(--color-muted); font-size: var(--font-size-sm); }
.drop { display: block; cursor: pointer; border: 2px dashed var(--color-border); border-radius: var(--r-3); margin-top: var(--space-2); }
.drop:hover { border-color: var(--color-accent-edge); }
.drop-hint { display: flex; flex-direction: column; align-items: center; justify-content: center; gap: var(--space-2);
  min-height: 180px; color: var(--color-muted); text-align: center; }
.emoji { font-size: 2rem; }
.preview { width: 100%; max-height: 240px; object-fit: contain; border-radius: var(--r-2); padding: var(--space-2); }
.tokens { display: flex; gap: var(--space-4); flex-wrap: wrap; align-items: center; margin-top: var(--space-3); }
.mono { font-family: var(--font-mono); color: var(--color-muted); font-size: var(--font-size-sm); }
@media (max-width: 768px) { .page { padding: 1rem; } }
</style>
