<script setup lang="ts">
import { ref, watch, onBeforeUnmount } from 'vue'
import { api, ApiError } from '../lib/api'
import { useBots } from '../lib/bots'
import { extractPalette, pickPrimaryAccent, type Swatch } from '../lib/palette'
import { applyTheme } from '../lib/theme'

const { currentBot, currentBotId, applyUpsert } = useBots()

const primary = ref('#e8467a')
const accent = ref('#e06699')
// Which slot a tapped swatch fills (works on touch — no shift key needed).
const target = ref<'primary' | 'accent'>('primary')
const swatches = ref<Swatch[]>([])

function assign(hex: string) {
  if (target.value === 'primary') primary.value = hex
  else accent.value = hex
}
function isPrimary(hex: string) {
  return hex.toLowerCase() === primary.value.toLowerCase()
}
function isAccent(hex: string) {
  return hex.toLowerCase() === accent.value.toLowerCase()
}
const imgSrc = ref('')
const extracting = ref(false)
const saving = ref(false)
const error = ref('')
const success = ref(false)

// Seed from the selected bot.
function seed() {
  primary.value = currentBot.value?.themePrimary || '#e8467a'
  accent.value = currentBot.value?.themeAccent || '#e06699'
  swatches.value = []
  imgSrc.value = ''
  success.value = false
  error.value = ''
}
watch(currentBotId, seed, { immediate: true })

// Live-preview the whole panel as the colors change.
watch([primary, accent], () => applyTheme(primary.value, accent.value))

// Leaving without saving: restore the bot's stored theme.
onBeforeUnmount(() => applyTheme(currentBot.value?.themePrimary, currentBot.value?.themeAccent))

async function onFile(file: File | undefined) {
  if (!file) return
  error.value = ''
  extracting.value = true
  imgSrc.value = URL.createObjectURL(file)
  try {
    swatches.value = await extractPalette(file, 6)
    const pick = pickPrimaryAccent(swatches.value)
    primary.value = pick.primary
    accent.value = pick.accent
  } catch {
    error.value = 'Could not read that image.'
  } finally {
    extracting.value = false
  }
}

function onDrop(e: DragEvent) {
  onFile(e.dataTransfer?.files?.[0])
}

async function save() {
  if (!currentBotId.value) return
  saving.value = true
  error.value = ''
  success.value = false
  try {
    const updated = await api.setTheme(currentBotId.value, primary.value, accent.value)
    applyUpsert(updated)
    success.value = true
    setTimeout(() => (success.value = false), 3000)
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Could not save the theme.'
  } finally {
    saving.value = false
  }
}

async function reset() {
  if (!currentBotId.value) return
  primary.value = '#e8467a'
  accent.value = '#e06699'
  saving.value = true
  try {
    const updated = await api.setTheme(currentBotId.value, '', '')
    applyUpsert(updated)
    applyTheme(null, null)
    primary.value = '#e8467a'
    accent.value = '#e06699'
  } catch (e) {
    error.value = e instanceof ApiError ? e.message : 'Could not reset.'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="page">
    <header class="page-header">
      <h1>Theme</h1>
      <span v-if="currentBot" class="for">for {{ currentBot.botName }}</span>
    </header>

    <p v-if="!currentBotId" class="muted">Pick a bot in the menu (top-left) to theme it.</p>

    <template v-else>
      <p class="sub">
        Drop an image to pull its main colors (center-weighted, most vivid first), then fine-tune.
        This re-skins the panel and becomes the bot's brand color for Discord embeds.
      </p>

      <div v-if="error" class="banner error">{{ error }}</div>
      <div v-if="success" class="banner success">Theme saved.</div>

      <div class="grid">
        <!-- Image / extraction -->
        <section class="card drop"
          @dragover.prevent
          @drop.prevent="onDrop"
        >
          <label class="dropzone">
            <input type="file" accept="image/*" hidden @change="onFile(($event.target as HTMLInputElement).files?.[0])" />
            <img v-if="imgSrc" :src="imgSrc" alt="source" class="preview-img" />
            <div v-else class="drop-hint">
              <div class="drop-emoji" aria-hidden="true">🖼️</div>
              <p>Drop an image here<br />or click to choose</p>
            </div>
          </label>
          <p v-if="extracting" class="muted small">Extracting…</p>
          <div v-else-if="swatches.length" class="extract">
            <div class="target-toggle" role="group" aria-label="Fill which slot">
              <button type="button" :class="{ active: target === 'primary' }" @click="target = 'primary'">
                <span class="dot" :style="{ background: primary }"></span> Primary
              </button>
              <button type="button" :class="{ active: target === 'accent' }" @click="target = 'accent'">
                <span class="dot" :style="{ background: accent }"></span> Accent
              </button>
            </div>
            <div class="swatches">
              <button
                v-for="s in swatches"
                :key="s.hex"
                type="button"
                class="swatch"
                :class="{ chosen: isPrimary(s.hex) || isAccent(s.hex) }"
                :style="{ background: s.hex }"
                :title="s.hex"
                @click="assign(s.hex)"
              >
                <span v-if="isPrimary(s.hex)" class="badge">P</span>
                <span v-else-if="isAccent(s.hex)" class="badge">A</span>
                <span class="swatch-hex">{{ s.hex }}</span>
              </button>
            </div>
            <p class="muted small">Pick <strong>{{ target }}</strong> above, then tap a color to set it.</p>
          </div>
        </section>

        <!-- Chosen colors + preview -->
        <section class="card">
          <div class="picker">
            <label>Primary</label>
            <div class="picker-row">
              <input type="color" v-model="primary" />
              <input type="text" v-model="primary" class="hex" />
            </div>
          </div>
          <div class="picker">
            <label>Accent</label>
            <div class="picker-row">
              <input type="color" v-model="accent" />
              <input type="text" v-model="accent" class="hex" />
            </div>
          </div>

          <div class="mini-preview">
            <div class="mp-bar" :style="{ background: `linear-gradient(90deg, ${primary}, ${accent})` }">Aa</div>
            <button type="button" class="mp-btn" :style="{ background: primary }">Primary button</button>
            <span class="mp-pill" :style="{ borderColor: accent, color: accent }">accent</span>
          </div>

          <div class="actions">
            <button type="button" class="save-btn" :disabled="saving" @click="save">
              {{ saving ? 'Saving…' : 'Save theme' }}
            </button>
            <button type="button" class="reset-btn" :disabled="saving" @click="reset">Reset to default</button>
          </div>
          <p class="muted small">The whole panel previews live as you tweak; Save persists it (and a bot reload applies the embed color).</p>
        </section>
      </div>
    </template>
  </div>
</template>

<style scoped>
.page { padding: 2rem; }
.page-header { display: flex; align-items: baseline; gap: 0.75rem; margin-bottom: 0.5rem; }
.page-header h1 {
  margin: 0;
  font-size: 1.9rem;
  background: linear-gradient(90deg, var(--foxfire), var(--violet));
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
}
.for { color: var(--muted-color); font-weight: 500; }
.sub { color: var(--muted-color); margin: 0 0 1.25rem; max-width: 640px; }
.muted { color: var(--muted-color); }
.small { font-size: 0.8rem; }

.banner { border-radius: 8px; padding: 0.7rem 1rem; margin-bottom: 1rem; font-weight: 500; }
.banner.error { background: rgba(248,113,113,0.12); border: 1px solid rgba(248,113,113,0.4); color: var(--danger-color); }
.banner.success { background: rgba(74,222,128,0.12); border: 1px solid rgba(74,222,128,0.4); color: var(--success-color); }

.grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1.25rem; align-items: start; }
.card {
  background: var(--surface);
  border: 1px solid var(--border-color);
  border-radius: 14px;
  padding: 1.25rem;
}

.dropzone { display: block; cursor: pointer; }
.drop { transition: border-color 0.15s ease; }
.drop:hover { border-color: var(--foxfire); }
.drop-hint {
  display: flex; flex-direction: column; align-items: center; justify-content: center;
  gap: 0.5rem; min-height: 180px; color: var(--muted-color); text-align: center;
  border: 2px dashed var(--border-color); border-radius: 12px;
}
.drop-emoji { font-size: 2rem; }
.preview-img { width: 100%; max-height: 240px; object-fit: contain; border-radius: 10px; }

.extract { margin-top: 1rem; }
.target-toggle {
  display: inline-flex;
  gap: 0.25rem;
  padding: 0.25rem;
  border: 1px solid var(--border-color);
  border-radius: 999px;
  background: var(--surface-2, #fff);
  margin-bottom: 0.75rem;
}
.target-toggle button {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  border: none;
  background: transparent;
  border-radius: 999px;
  padding: 0.45rem 0.9rem;
  font: inherit;
  font-weight: 600;
  color: var(--muted-color);
  cursor: pointer;
}
.target-toggle button.active { background: var(--foxfire); color: #fff; }
.target-toggle .dot {
  width: 14px; height: 14px; border-radius: 50%;
  border: 1px solid rgba(255,255,255,0.6);
}

.swatches { display: flex; flex-wrap: wrap; gap: 0.5rem; }
.swatch {
  position: relative; width: 64px; height: 52px; border-radius: 10px;
  border: 1px solid rgba(0,0,0,0.1); cursor: pointer; overflow: hidden;
}
.swatch.chosen { outline: 3px solid var(--foxfire); outline-offset: 1px; }
.badge {
  position: absolute; top: 3px; left: 3px;
  width: 18px; height: 18px; border-radius: 50%;
  background: rgba(0,0,0,0.55); color: #fff;
  font-size: 0.65rem; font-weight: 700;
  display: grid; place-items: center;
}
.swatch-hex {
  position: absolute; inset: auto 0 0 0; font-size: 0.6rem; text-align: center;
  background: rgba(0,0,0,0.45); color: #fff; padding: 1px 0;
}

@media (max-width: 768px) {
  .swatch { width: 56px; height: 56px; }
  .target-toggle { width: 100%; justify-content: center; }
}

.picker { margin-bottom: 1rem; }
.picker label { font-weight: 600; display: block; margin-bottom: 0.35rem; }
.picker-row { display: flex; gap: 0.5rem; align-items: center; }
.picker-row input[type='color'] { width: 48px; height: 38px; border: 1px solid var(--border-color); border-radius: 8px; background: none; cursor: pointer; }
.hex { flex: 1; padding: 0.55rem 0.7rem; border: 1px solid var(--input-border, #ecc0cf); border-radius: 8px; font: inherit; font-size: 16px; }

.mini-preview { display: flex; align-items: center; gap: 0.75rem; flex-wrap: wrap; margin: 1rem 0; }
.mp-bar { width: 56px; height: 40px; border-radius: 8px; color: #fff; font-weight: 700; display: grid; place-items: center; }
.mp-btn { border: none; color: #fff; border-radius: 8px; padding: 0.5rem 1rem; font-weight: 600; }
.mp-pill { border: 1px solid; border-radius: 999px; padding: 0.2rem 0.7rem; font-size: 0.8rem; font-weight: 600; }

.actions { display: flex; gap: 0.6rem; flex-wrap: wrap; margin-top: 0.5rem; }
.save-btn { background: var(--foxfire); color: #fff; border: none; border-radius: 10px; padding: 0.6rem 1.4rem; font-weight: 600; cursor: pointer; }
.save-btn:disabled { opacity: 0.6; cursor: default; }
.reset-btn { background: var(--surface-2); border: 1px solid var(--border-color); border-radius: 10px; padding: 0.6rem 1rem; cursor: pointer; }

@media (max-width: 768px) {
  .page { padding: 1rem; }
  .page-header h1 { font-size: 1.5rem; }
  .grid { grid-template-columns: 1fr; }
}
</style>
