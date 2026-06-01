// Mirror of AiryBotCode.Api.Models.BotSettingDto.
//
// Every Discord snowflake id is a string here on purpose: snowflakes are 64-bit
// and overflow JavaScript's safe-integer range, so the API exchanges them as
// strings. `token` is write-only — the API never returns it; `hasToken` says
// whether one is stored.
export interface BotSetting {
  botId: string
  botName: string
  enabled: boolean

  openAIModel: string
  openAIPrompt: string
  maxTokens: number
  retryAttempts: number

  ownerId: string
  evilId: string

  logChannelId: string
  evilLogChannelId: string
  verifyLogChannelId: string
  rulesChannelId: string
  giveawayScoreboardChannelId: string
  listenChannelIds: string

  adminRoleIds: string
  verifiedRoleId: string
  unverifiedRoleId: string

  contactCategoryId: string

  hasToken: boolean
  token?: string | null
}

export function emptyBotSetting(): BotSetting {
  return {
    botId: '',
    botName: '',
    enabled: false,
    openAIModel: '',
    openAIPrompt: '',
    maxTokens: 0,
    retryAttempts: 0,
    ownerId: '',
    evilId: '',
    logChannelId: '',
    evilLogChannelId: '',
    verifyLogChannelId: '',
    rulesChannelId: '',
    giveawayScoreboardChannelId: '',
    listenChannelIds: '',
    adminRoleIds: '',
    verifiedRoleId: '',
    unverifiedRoleId: '',
    contactCategoryId: '',
    hasToken: false,
    token: '',
  }
}
