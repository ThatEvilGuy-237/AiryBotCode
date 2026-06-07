// src/lib/types/database.ts

export enum ChatRole {
    System,
    User,
    Assistant,
    Owner
}

export interface ChatUser {
    id: number; // Using number for frontend simplicity, will be ulong on backend
    userName: string;
    role: ChatRole;
    createdAt: string; // ISO 8601 date string
    aiOpinion: string;
    messages: Message[];
}

export interface Message {
    id: number;
    channelId: number; // ulong
    userId: number; // ulong
    user: ChatUser;
    context: string;
    channelConversationId: number;
    channelConversation: ChannelConversation;
    createdAt: string; // ISO 8601 date string
}

export interface ChannelConversation {
    id: number;
    channelId: number; // ulong
    conversationSummary: string;
    createdAt: string; // ISO 8601 date string
    updatedAt: string; // ISO 8601 date string
    messages: Message[];
}

export interface GiveAwayUser {
    id: number; // ulong
    userName: string;
    submitedAt: string; // ISO 8601 date string
}

export interface BotSetting {
    botId: number; // ulong
    botName: string;
    avatarUrl?: string; // Add optional avatar URL
    openAIModel: string;
    openAIPrompt: string;
    enabled: boolean;
    token: string;
    adminRoleIds: string;
    evilId: number; // ulong
    logChannelId: number; // ulong
    evilLogChannelId: number; // ulong
}

// --- Command configuration ---
// Mirrors the backend CommandSetting entity (AiryBotCode.Domain/database/CommandSetting.cs),
// which is projected from the [ConfigurableCommand] / [LiveSetting] / [ReloadableSetting]
// attributes declared on each command. The UiHint drives how a value is rendered/edited.

export type CommandSettingUiHint = 'text' | 'number' | 'textarea' | 'json' | 'boolean';

export interface CommandSetting {
    key: string;            // property name, e.g. "SuccessMessageFormat"
    value: string;          // current value, stored as string (JSON-encoded for objects)
    description: string;    // help text from the setting attribute
    category: string;       // grouping hint, e.g. "General" / "Moderation"
    uiHint: CommandSettingUiHint;
    isReloadable: boolean;  // true => [ReloadableSetting] (needs reload), false => [LiveSetting] (hot)
}

export interface CommandConfig {
    commandName: string;    // backing class name, e.g. "TimeoutCommand"
    displayName: string;    // human label, e.g. "Timeout"
    slug: string;           // discord command name / id, e.g. "timeout"
    description: string;    // short summary shown on the card
    settings: CommandSetting[];
}
