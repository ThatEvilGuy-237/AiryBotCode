<script setup lang="ts">
import { computed, ref } from 'vue'
import { BrandMark, Button, Field, Icon, TextField } from '@airy/ui'
import { discordSignInUrl, openGate } from '@/lib/api'

const password = ref('')
const gate = ref<string | null>(null)
const busy = ref(false)
const error = ref<string | null>(null)
const step = computed(() => (gate.value ? 2 : 1))

async function next() {
  if (!password.value || busy.value) return
  busy.value = true
  error.value = null
  try {
    gate.value = await openGate(password.value)
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Signing in failed.'
  } finally {
    busy.value = false
  }
}

function back() {
  gate.value = null
  password.value = ''
}
</script>

<template>
  <main class="signin">
    <section class="panel" aria-labelledby="signin-title">
      <BrandMark name="Airy" :size="40" class="mark" />
      <div class="steps" role="progressbar" aria-label="Sign-in steps" aria-valuemin="1" aria-valuemax="2" :aria-valuenow="step" :aria-valuetext="`Step ${step} of 2`">
        <span class="seg" :class="step === 1 ? 'on' : 'done'" />
        <span class="seg" :class="{ on: step === 2 }" />
      </div>
      <template v-if="step === 1">
        <h1 id="signin-title">Sign in to Airy</h1>
        <p class="lead">First the access password, then Discord tells Airy who you are.</p>
        <form class="form" @submit.prevent="next">
          <Field label="Access password" :error="error ?? undefined">
            <TextField v-model="password" type="password" autocomplete="current-password" />
          </Field>
          <Button type="submit" :disabled="!password" :loading="busy">Continue</Button>
        </form>
      </template>
      <template v-else>
        <h1 id="signin-title">Now sign in with Discord</h1>
        <p class="lead">Only Discord accounts allowed to run Airy get in.</p>
        <div class="form">
          <Button :href="discordSignInUrl(gate!)"><Icon name="log-in" :size="16" />Sign in with Discord</Button>
          <Button variant="ghost" @click="back">Back</Button>
        </div>
      </template>
    </section>
  </main>
</template>

<style scoped>
.signin {
  min-height: 100vh;
  min-height: 100dvh;
  display: grid;
  place-items: center;
  padding: var(--space-6) var(--space-4);
  background: var(--color-bg);
}
.panel {
  width: min(400px, 100%);
  display: grid;
  gap: var(--space-4);
  padding: var(--space-6);
  border: var(--border);
  border-radius: var(--r-3);
  background: var(--color-surface);
}
.mark {
  font-size: var(--font-size-lg);
}
.steps {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-1);
}
.seg {
  height: 4px;
  border-radius: 2px;
  background: var(--color-surface-hi);
}
.seg.on {
  background: var(--color-accent);
}
.seg.done {
  background: var(--color-accent-edge);
}
h1 {
  margin: 0;
  font-size: var(--font-size-xl);
  font-family: var(--font-display);
  color: var(--color-fg);
}
.lead {
  margin: 0;
  color: var(--color-text-dim);
}
.form {
  display: grid;
  gap: var(--space-3);
}
</style>
