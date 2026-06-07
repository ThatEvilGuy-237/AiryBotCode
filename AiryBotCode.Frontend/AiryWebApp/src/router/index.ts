import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import CommandsView from '../views/CommandsView.vue'
import BotSettingsView from '../views/BotSettingsView.vue'
import DatabaseView from '../views/DatabaseView.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'home', component: HomeView },
    { path: '/commands', name: 'commands', component: CommandsView },
    { path: '/bot-settings', name: 'bot-settings', component: BotSettingsView },
    { path: '/database', name: 'database', component: DatabaseView },
  ],
})
