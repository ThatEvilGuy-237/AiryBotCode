import type { AnswerMode } from './api'

export const ruleLabels: Record<AnswerMode, string> = {
  always: 'Always',
  mention: 'When mentioned',
  decide: "When it's worth it",
}

export const ruleHelp: Record<AnswerMode, string> = {
  always: 'Every message.',
  mention: 'When someone mentions Airy or replies to it.',
  decide: 'The Hive picks the messages worth an answer. Mentions always count.',
}
