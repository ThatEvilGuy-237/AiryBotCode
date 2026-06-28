import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import CommandsView from '../views/CommandsView.vue'
import BotSettingsView from '../views/BotSettingsView.vue'
import ThemeView from '../views/ThemeView.vue'
import WebhooksView from '../views/WebhooksView.vue'
import StatusView from '../views/StatusView.vue'
import SuggestionsView from '../views/SuggestionsView.vue'
import DatabasesView from '../views/DatabasesView.vue'
import DatabaseView from '../views/DatabaseView.vue'
import HiveUiView from '../views/HiveUiView.vue'
import PublicSuggestView from '../views/PublicSuggestView.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'home', component: HomeView },
    { path: '/commands', name: 'commands', component: CommandsView },
    { path: '/bot-settings', name: 'bot-settings', component: BotSettingsView },
    { path: '/theme', name: 'theme', component: ThemeView },
    { path: '/webhooks', name: 'webhooks', component: WebhooksView },
    { path: '/status', name: 'status', component: StatusView },
    { path: '/suggestions', name: 'suggestions', component: SuggestionsView },
    { path: '/database', name: 'databases', component: DatabasesView },
    { path: '/database/:db', name: 'database', component: DatabaseView, props: true },
    { path: '/hive-ui', name: 'hive-ui', component: HiveUiView },
    // PUBLIC capability link — no login, no dashboard chrome (meta.public). The
    // server validates the code on every API call; this route is UX only.
    { path: '/suggest/:code', name: 'public-suggest', component: PublicSuggestView, props: true, meta: { public: true } },
  ],
})
