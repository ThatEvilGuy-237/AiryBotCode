import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import BotSettingsView from '../views/BotSettingsView.vue'
import DatabaseView from '../views/DatabaseView.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'home', component: HomeView },
    { path: '/bot-settings', name: 'bot-settings', component: BotSettingsView },
    { path: '/database', name: 'database', component: DatabaseView },
  ],
})
