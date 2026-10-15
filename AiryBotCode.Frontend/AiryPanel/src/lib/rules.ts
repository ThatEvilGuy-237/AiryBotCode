import type { AnswerMode } from './api'

export const ruleLabels: Record<AnswerMode, string> = {
  always: 'Always',
  mention: 'When mentioned',
  decide: "When it's worth it",
}

export const ruleHelp: Record<AnswerMode, string> = {
  always: 'Airy answers every message.',
  mention: 'Airy answers when someone mentions it or replies to it.',
  decide: 'The Hive reads each message and Airy answers the ones worth it; mentions are always answered.',
}
