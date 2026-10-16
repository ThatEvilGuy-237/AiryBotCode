import type { AnswerMode } from './api'

export const ruleLabels: Record<AnswerMode, string> = {
  always: 'Always',
  mention: 'When mentioned',
  decide: "When it's worth it",
}

export const ruleHelp: Record<AnswerMode, string> = {
  always: 'Every message.',
  mention: 'When someone mentions Airy or replies to it.',
  decide: 'Airy follows the talk and joins in when the Hive finds it worth it, at most once every 10 minutes. Mentions are always answered.',
}
