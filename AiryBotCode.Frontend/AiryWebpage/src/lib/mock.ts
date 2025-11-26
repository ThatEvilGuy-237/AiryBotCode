// src/lib/mock.ts
import type { BotSetting, ChatUser, Message, ChannelConversation, GiveAwayUser } from "./types/database";
import { ChatRole } from "./types/database";

export const mockBotSettings: BotSetting[] = [
    {
        botId: 1,
        botName: "AiryBot",
        openAIModel: "text-davinci-003",
        openAIPrompt: "You are a helpful assistant.",
        enabled: true,
        token: "...",
        adminRoleIds: "123,456",
        evilId: 789,
        logChannelId: 101,
        evilLogChannelId: 102,
    },
    {
        botId: 2,
        botName: "AiryGuardian",
        openAIModel: "text-davinci-003",
        openAIPrompt: "You are a moderator.",
        enabled: true,
        token: "...",
        adminRoleIds: "123",
        evilId: 789,
        logChannelId: 201,
        evilLogChannelId: 202,
    }
];

export const mockChatUsers: ChatUser[] = [
    {
        id: 1001,
        userName: "TestUser1",
        role: ChatRole.User,
        createdAt: new Date().toISOString(),
        aiOpinion: "A friendly user.",
        messages: []
    },
    {
        id: 1002,
        userName: "TestUser2",
        role: ChatRole.User,
        createdAt: new Date().toISOString(),
        aiOpinion: "A curious user.",
        messages: []
    }
];

export const mockMessages: Message[] = [
    {
        id: 2001,
        channelId: 3001,
        userId: 1001,
        context: "Hello, world!",
        channelConversationId: 4001,
        createdAt: new Date().toISOString(),
        user: mockChatUsers[0],
        channelConversation: { id: 4001, channelId: 3001, conversationSummary: "First conversation", createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), messages:[] }
    },
    {
        id: 2002,
        channelId: 3001,
        userId: 1002,
        context: "How are you?",
        channelConversationId: 4001,
        createdAt: new Date().toISOString(),
        user: mockChatUsers[1],
        channelConversation: { id: 4001, channelId: 3001, conversationSummary: "First conversation", createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(), messages:[] }
    }
];

export const mockChannelConversations: ChannelConversation[] = [
    {
        id: 4001,
        channelId: 3001,
        conversationSummary: "A summary of the conversation in this channel.",
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        messages: [mockMessages[0], mockMessages[1]]
    }
];

export const mockGiveAwayUsers: GiveAwayUser[] = [
    {
        id: 1001,
        userName: "TestUser1",
        submitedAt: new Date().toISOString()
    }
];
