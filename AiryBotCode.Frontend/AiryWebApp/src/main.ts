import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import { router } from './router'
import { captureTokenFromHash, isAuthenticated, LOGIN_URL } from './lib/auth'

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
