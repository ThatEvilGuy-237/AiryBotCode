import { ref, computed } from 'vue'
import { api } from './api'
import type { BotSetting } from '../types/botSetting'

// Shared, app-wide bot roster + the currently-selected bot. The sidebar switcher
// drives the selection; per-bot pages (Settings, …) read `currentBot`.
const bots = ref<BotSetting[]>([])
const currentBotId = ref<string | null>(null)
const loaded = ref(false)
const loadError = ref('')

// Set by the switcher's "New bot" button; consumed by the Settings page.
const createRequested = ref(false)

const currentBot = computed(() => bots.value.find((b) => b.botId === currentBotId.value) ?? null)

function ensureSelection(): void {
  if (!bots.value.length) {
    currentBotId.value = null
    return
  }
  if (!currentBotId.value || !bots.value.some((b) => b.botId === currentBotId.value)) {
    currentBotId.value = bots.value[0].botId
  }
}

async function loadBots(force = false): Promise<void> {
  if (loaded.value && !force) return
  try {
    bots.value = await api.getBotSettings()
    loaded.value = true
    loadError.value = ''
  } catch (e) {
    loadError.value = e instanceof Error ? e.message : 'Failed to load bots.'
    return
  }
  ensureSelection()
}

function selectBot(id: string): void {
  currentBotId.value = id
}

// Insert or update a bot, selecting it.
function applyUpsert(bot: BotSetting): void {
  const i = bots.value.findIndex((b) => b.botId === bot.botId)
  if (i === -1) bots.value.push(bot)
  else bots.value[i] = bot
  currentBotId.value = bot.botId
}

// Replace a possibly re-keyed bot (BotId may have changed on save).
function applyReplace(oldId: string, bot: BotSetting): void {
  bots.value = bots.value.filter((b) => b.botId !== oldId && b.botId !== bot.botId)
  bots.value.push(bot)
  currentBotId.value = bot.botId
}

function applyRemove(id: string): void {
  bots.value = bots.value.filter((b) => b.botId !== id)
  ensureSelection()
}

function requestCreate(): void {
  createRequested.value = true
}

// Returns true once per request, then resets.
function consumeCreateRequest(): boolean {
  if (!createRequested.value) return false
  createRequested.value = false
  return true
}

export function useBots() {
  return {
    bots,
    currentBotId,
    currentBot,
    loaded,
    loadError,
    createRequested,
    loadBots,
    selectBot,
    applyUpsert,
    applyReplace,
    applyRemove,
    requestCreate,
    consumeCreateRequest,
  }
}
