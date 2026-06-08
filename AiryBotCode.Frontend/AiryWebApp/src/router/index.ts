import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import CommandsView from '../views/CommandsView.vue'
import BotSettingsView from '../views/BotSettingsView.vue'
import ThemeView from '../views/ThemeView.vue'
import DatabasesView from '../views/DatabasesView.vue'
import DatabaseView from '../views/DatabaseView.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'home', component: HomeView },
    { path: '/commands', name: 'commands', component: CommandsView },
    { path: '/bot-settings', name: 'bot-settings', component: BotSettingsView },
    { path: '/theme', name: 'theme', component: ThemeView },
    { path: '/database', name: 'databases', component: DatabasesView },
    { path: '/database/:db', name: 'database', component: DatabaseView, props: true },
  ],
})
