import { createRouter, createWebHistory } from 'vue-router'
import { isSignedIn } from './lib/auth'
import Shell from './layouts/Shell.vue'
import HomePage from './pages/HomePage.vue'
import SignInPage from './pages/SignInPage.vue'
import ChannelsPage from './pages/ChannelsPage.vue'
import CommandsPage from './pages/CommandsPage.vue'
import SettingsPage from './pages/SettingsPage.vue'
import LookPage from './pages/LookPage.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/login', name: 'login', component: SignInPage, meta: { public: true } },
    {
      path: '/',
      component: Shell,
      children: [
        { path: '', name: 'home', component: HomePage, meta: { title: 'Home' } },
        { path: 'channels', name: 'channels', component: ChannelsPage, meta: { title: 'Channels' } },
        { path: 'commands', name: 'commands', component: CommandsPage, meta: { title: 'Commands' } },
        { path: 'settings', name: 'settings', component: SettingsPage, meta: { title: 'Settings' } },
        { path: 'look', name: 'look', component: LookPage, meta: { title: 'Look' } },
      ],
    },
    { path: '/:rest(.*)*', redirect: '/' },
  ],
})

router.beforeEach(to => {
  if (to.name === 'login' && isSignedIn()) return '/'
  if (!to.meta.public && !isSignedIn()) return '/login'
  return true
})

router.afterEach(to => {
  document.title = to.meta.title ? `${to.meta.title} · Airy` : 'Airy'
})
