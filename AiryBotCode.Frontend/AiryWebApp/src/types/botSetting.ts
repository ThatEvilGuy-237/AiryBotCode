// Mirror of AiryBotCode.Api.Models.BotSettingDto.
//
// Every Discord snowflake id is a string here on purpose: snowflakes are 64-bit
// and overflow JavaScript's safe-integer range, so the API exchanges them as
// strings. `token` is write-only — the API never returns it; `hasToken` says
// whether one is stored. These are exactly the columns of the real BotSettings
// table (the row each bot seeds on first run).
export interface BotSetting {
  botId: string
  botName: string
  enabled: boolean

  openAIModel: string
  openAIPrompt: string

  adminRoleIds: string
  evilId: string

  logChannelId: string
  evilLogChannelId: string

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
    adminRoleIds: '',
    evilId: '',
    logChannelId: '',
    evilLogChannelId: '',
    hasToken: false,
    token: '',
  }
}
