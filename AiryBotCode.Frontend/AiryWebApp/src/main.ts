import { createApp } from 'vue'
// The Hive's shared design system — the panel is migrating onto @hive/ui (pages
// rebuilt incrementally). theme.css carries the tokens the image-derived theme drives;
// components.css styles the shared components. Imported BEFORE the panel's own style.css
// so existing (not-yet-migrated) pages keep their look during the migration.
import '@hive/ui/theme.css'
import '@hive/ui/components.css'
import './style.css'
import App from './App.vue'
import { router } from './router'
import { captureTokenFromHash, isAuthenticated, LOGIN_URL } from './lib/auth'
import { applyStartupTheme } from './lib/imageTheme'

// Re-apply the last-active bot's saved image-derived theme before mount so the
// panel renders themed (no flash of the default look). The per-bot theme is then
// re-applied on every bot switch (App.vue watches currentBotId).
applyStartupTheme()

// Grab the JWT the API hands back via the #token=… redirect fragment BEFORE the
// router guard runs, so the freshly-issued token is already in place.
captureTokenFromHash()

// Gate every route: no valid JWT → straight back to the login app.
router.beforeEach(() => {
  if (!isAuthenticated()) {
    window.location.replace(LOGIN_URL)
    return false
  }
  return true
})

createApp(App).use(router).mount('#app')
