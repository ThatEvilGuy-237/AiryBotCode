import { createApp } from 'vue'
import '@airy/ui/fonts.css'
import '@airy/ui/reset.css'
import '@airy/ui/theme.css'
import '@airy/ui/site-tokens.css'
import '@airy/ui/tokens.css'
import '@airy/ui/components.css'
import './style.css'
import { useInstallPrompt } from '@airy/ui'
import App from './App.vue'
import { captureToken } from './lib/auth'
import { router } from './router'

captureToken()
useInstallPrompt()
if ('serviceWorker' in navigator && window.isSecureContext && location.hostname !== 'localhost') {
  navigator.serviceWorker.register('/sw.js').catch(() => undefined)
}

createApp(App).use(router).mount('#app')
