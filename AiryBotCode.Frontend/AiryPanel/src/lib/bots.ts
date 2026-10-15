import { computed, ref, watch } from 'vue'
import { api, type Bot } from './api'

const KEY = 'airy.bot'

export const bots = ref<Bot[]>([])
export const botsLoaded = ref(false)
export const botsError = ref<string | null>(null)
export const botId = ref<string | null>(localStorage.getItem(KEY))
export const botImage = ref<string | null>(null)

export const bot = computed(() => bots.value.find(b => b.botId === botId.value) ?? null)

export function pickBot(id: string): void {
  botId.value = id
  localStorage.setItem(KEY, id)
}

export async function loadBots(force = false): Promise<void> {
  if (botsLoaded.value && !force) return
  botsError.value = null
  try {
    bots.value = await api.bots()
    if (!bots.value.some(b => b.botId === botId.value) && bots.value.length) pickBot(bots.value[0].botId)
  } catch (e) {
    botsError.value = e instanceof Error ? e.message : 'The bots could not be loaded.'
  } finally {
    botsLoaded.value = true
  }
}

watch(botId, async id => {
  botImage.value = null
  if (!id) return
  try {
    const look = await api.look(id)
    if (botId.value === id) botImage.value = look.image
  } catch {
    botImage.value = null
  }
}, { immediate: true })
