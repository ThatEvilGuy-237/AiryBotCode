<script setup lang="ts">
// Item E: image → FULL-PAGE theme on the Hive's shared UI. Upload an image; its
// standout colour becomes the accent (hue+lightness+chroma) and its palette tints
// every surface — and because every @hive/ui component reads the theme tokens,
// they ALL re-theme live, with no per-component work. The image doubles as the
// panel profile picture. Engine lives in @hive/ui (deriveThemeFromImage).
import { ref } from 'vue'
import {
  Button, Badge, Card, PageHeader, StatusDot, Divider,
  deriveThemeFromImage, applyTheme, clearTheme, type DerivedTheme,
} from '@hive/ui'
import '@hive/ui/theme.css'
import '@hive/ui/components.css'

const profileSrc = ref<string | null>(null)
const theme = ref<DerivedTheme | null>(null)
const fileInput = ref<HTMLInputElement | null>(null)

function pick() { fileInput.value?.click() }

function onFile(e: Event) {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file) return
  const reader = new FileReader()
  reader.onload = () => {
    const src = reader.result as string
    profileSrc.value = src
    const img = new Image()
    img.onload = () => {
      theme.value = deriveThemeFromImage(img)
      applyTheme(theme.value, document.documentElement)   // re-theme the whole page
    }
    img.src = src
  }
  reader.readAsDataURL(file)
}

function reset() {
  theme.value = null
  profileSrc.value = null
  clearTheme(document.documentElement)
}
</script>

<template>
  <div class="theme-demo">
    <PageHeader
      title="Image → full-page theme"
      subtitle="Upload an image — the whole @hive/ui surface re-themes from its palette."
    />

    <Card>
      <template #head>
        <img v-if="profileSrc" :src="profileSrc" class="profile" alt="theme source / profile" />
        <span v-else class="profile placeholder" />
        <h3>Panel theme</h3>
        <span class="hd-sub">// the image becomes the accent + surface tint, and the profile pic</span>
        <div style="flex:1" />
        <Button @click="pick">Upload image</Button>
        <Button variant="ghost" @click="reset">Reset</Button>
        <input ref="fileInput" type="file" accept="image/*" hidden @change="onFile" />
      </template>

      <p v-if="!theme">
        Pick an image. Its most vivid colour becomes the accent (hue + lightness + chroma),
        and the dark surfaces tint toward its dominant hue. Every component below updates live.
      </p>
      <div v-else class="tokens">
        <span class="mono">accent {{ theme.accentHue }}° · L {{ theme.accentL }} · C {{ theme.accentC }}</span>
        <span class="mono">base {{ theme.baseHue }}° · tint × {{ theme.baseChroma }}</span>
      </div>
    </Card>

    <Card>
      <template #head><h3>Components re-theme automatically</h3></template>
      <div class="row">
        <Button>Primary</Button>
        <Button variant="outline">Outline</Button>
        <Button variant="ghost">Ghost</Button>
      </div>
      <Divider />
      <div class="row">
        <Badge variant="active">active</Badge>
        <Badge variant="warning">warning</Badge>
        <Badge variant="danger">danger</Badge>
        <StatusDot variant="live" /><span class="mono">live</span>
      </div>
    </Card>
  </div>
</template>

<style scoped>
.theme-demo { padding: 1.5rem; display: flex; flex-direction: column; gap: 1rem; }
.row { display: flex; gap: .5rem; align-items: center; flex-wrap: wrap; }
.profile { width: 40px; height: 40px; border-radius: 50%; object-fit: cover; }
.profile.placeholder { background: var(--color-surface-mute); border: var(--border); display: inline-block; }
.hd-sub { font-family: var(--font-mono); color: var(--color-muted); font-size: var(--font-size-sm); }
.tokens { display: flex; gap: 1.5rem; flex-wrap: wrap; }
.mono { font-family: var(--font-mono); color: var(--color-muted); font-size: var(--font-size-sm); }
</style>
