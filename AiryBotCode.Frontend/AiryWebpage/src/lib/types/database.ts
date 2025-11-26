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
    openAIModel: string;
    openAIPrompt: string;
    enabled: boolean;
    token: string;
    adminRoleIds: string;
    evilId: number; // ulong
    logChannelId: number; // ulong
    evilLogChannelId: number; // ulong
}
