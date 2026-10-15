import { createRouter, createWebHistory } from 'vue-router'
import { isSignedIn } from './lib/auth'
import Shell from './layouts/Shell.vue'
import HomePage from './pages/HomePage.vue'
import SignInPage from './pages/SignInPage.vue'
import ChannelsPage from './pages/ChannelsPage.vue'
import CommandsPage from './pages/CommandsPage.vue'
import SettingsPage from './pages/SettingsPage.vue'
import LookPage from './pages/LookPage.vue'
import ActivityPage from './pages/ActivityPage.vue'
import SuggestionsPage from './pages/SuggestionsPage.vue'
import DatabasesPage from './pages/DatabasesPage.vue'
import PublicSuggestPage from './pages/PublicSuggestPage.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/login', name: 'login', component: SignInPage, meta: { public: true } },
    { path: '/suggest/:code', name: 'suggest', component: PublicSuggestPage, props: true, meta: { public: true, title: 'Ideas' } },
    {
      path: '/',
      component: Shell,
      children: [
        { path: '', name: 'home', component: HomePage, meta: { title: 'Home' } },
        { path: 'channels', name: 'channels', component: ChannelsPage, meta: { title: 'Channels' } },
        { path: 'commands', name: 'commands', component: CommandsPage, meta: { title: 'Commands' } },
        { path: 'settings', name: 'settings', component: SettingsPage, meta: { title: 'Settings' } },
        { path: 'look', name: 'look', component: LookPage, meta: { title: 'Look' } },
        { path: 'activity', name: 'activity', component: ActivityPage, meta: { title: 'Activity' } },
        { path: 'suggestions', name: 'suggestions', component: SuggestionsPage, meta: { title: 'Suggestions' } },
        { path: 'databases', name: 'databases', component: DatabasesPage, meta: { title: 'Databases' } },
        { path: 'databases/:db', name: 'database', component: DatabasesPage, props: true, meta: { title: 'Databases' } },
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
