// src/lib/schema.ts

// A more robust type for our column definitions
export interface Column {
    key: string;
    label: string;
    type: 'string' | 'number' | 'date';
    editable: boolean;
}

// Defines the structure of a single table's schema
export interface TableSchema {
    id: string; // e.g., 'chatUsers'
    name: string; // e.g., 'Chat Users'
    columns: Column[];
}

// The complete database schema
export const dbSchema: TableSchema[] = [
    {
        id: 'chatUsers',
        name: 'Chat Users',
        columns: [
            { key: 'id', label: 'ID', type: 'number', editable: false },
            { key: 'userName', label: 'Username', type: 'string', editable: true },
            { key: 'role', label: 'Role', type: 'string', editable: true },
            { key: 'aiOpinion', label: 'AI Opinion', type: 'string', editable: true },
            { key: 'createdAt', label: 'Created At', type: 'date', editable: false }
        ]
    },
    {
        id: 'messages',
        name: 'Messages',
        columns: [
            { key: 'id', label: 'ID', type: 'number', editable: false },
            { key: 'userId', label: 'User ID', type: 'number', editable: true },
            { key: 'channelId', label: 'Channel ID', type: 'number', editable: true },
            { key: 'channelConversationId', label: 'Conversation ID', type: 'number', editable: true },
            { key: 'context', label: 'Content', type: 'string', editable: true },
            { key: 'createdAt', label: 'Sent At', type: 'date', editable: false }
        ]
    },
    {
        id: 'conversations',
        name: 'Conversations',
        columns: [
            { key: 'id', label: 'ID', type: 'number', editable: false },
            { key: 'channelId', label: 'Channel ID', type: 'number', editable: true },
            { key: 'conversationSummary', label: 'Summary', type: 'string', editable: true },
            { key: 'createdAt', label: 'Created At', type: 'date', editable: false },
            { key: 'updatedAt', label: 'Last Updated', type: 'date', editable: false }
        ]
    },
    {
        id: 'giveaways',
        name: 'Giveaways',
        columns: [
            { key: 'id', label: 'User ID', type: 'number', editable: false },
            { key: 'userName', label: 'Username', type: 'string', editable: true },
            { key: 'submitedAt', label: 'Submitted At', type: 'date', editable: false }
        ]
    }
];
