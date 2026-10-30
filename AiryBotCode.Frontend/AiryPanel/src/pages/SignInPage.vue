<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { BrandMark, Button, Field, Icon, TextField } from '@airy/ui'
import { discordSignIn, discordStart, nextPath, signIn } from '@/lib/auth'

const route = useRoute()
const router = useRouter()
const login = ref('')
const password = ref('')
const busy = ref(false)
const error = ref<string | null>(null)
const discord = ref(false)

onMounted(async () => {
  discord.value = await discordSignIn()
})

async function submit() {
  if (!login.value.trim() || !password.value || busy.value) return
  busy.value = true
  error.value = null
  try {
    await signIn(login.value.trim(), password.value)
    await router.replace(nextPath(route.query.next))
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Signing in failed.'
    password.value = ''
  } finally {
    busy.value = false
  }
}
</script>

<template>
  <main class="signin">
    <section class="panel" aria-labelledby="signin-title">
      <BrandMark name="Airy" :size="40" class="mark" />
      <h1 id="signin-title">Sign in to Airy</h1>
      <p class="lead">With your Evil account.</p>
      <form class="form" @submit.prevent="submit">
        <Field label="User name or email">
          <TextField v-model="login" autocomplete="username" autocapitalize="none" spellcheck="false" />
        </Field>
        <Field label="Password" :error="error ?? undefined">
          <TextField v-model="password" type="password" autocomplete="current-password" />
        </Field>
        <Button type="submit" :disabled="!login.trim() || !password" :loading="busy">Sign in</Button>
      </form>
      <template v-if="discord">
        <p class="or" aria-hidden="true">or</p>
        <Button variant="ghost" :href="discordStart"><Icon name="log-in" :size="16" />Sign in with Discord</Button>
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
.or {
  margin: 0;
  text-align: center;
  color: var(--color-text-dim);
}
</style>
