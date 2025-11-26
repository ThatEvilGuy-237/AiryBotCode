<!-- src/routes/Database.svelte -->
<script lang="ts">
    import DataTable from '../lib/components/DataTable.svelte';
    import TableList from '../lib/components/TableList.svelte';
    import { mockChatUsers, mockMessages, mockChannelConversations, mockGiveAwayUsers } from '../lib/mock';

    let selectedTableId = 'chatUsers';

    const tables = [
        { id: 'chatUsers', name: 'Chat Users' },
        { id: 'messages', name: 'Messages' },
        { id: 'conversations', name: 'Conversations' },
        { id: 'giveaways', name: 'Giveaways' },
    ];

    const columnConfig = {
        chatUsers: [
            { key: 'id', label: 'ID', type: 'number' },
            { key: 'userName', label: 'Username', type: 'string' },
            { key: 'role', label: 'Role', type: 'string' },
            { key: 'createdAt', label: 'Created At', type: 'date' }
        ],
        messages: [
            { key: 'id', label: 'ID', type: 'number' },
            { key: 'userId', label: 'User ID', type: 'number' },
            { key: 'context', label: 'Content', type: 'string' },
            { key: 'createdAt', label: 'Sent At', type: 'date' }
        ],
        conversations: [
            { key: 'id', label: 'ID', type: 'number' },
            { key: 'channelId', label: 'Channel ID', type: 'number' },
            { key: 'conversationSummary', label: 'Summary', type: 'string' },
            { key: 'updatedAt', label: 'Last Updated', type: 'date' }
        ],
        giveaways: [
            { key: 'id', label: 'User ID', type: 'number' },
            { key: 'userName', label: 'Username', type: 'string' },
            { key: 'submitedAt', label: 'Submitted At', type: 'date' }
        ]
    };

    const dataConfig = {
        chatUsers: mockChatUsers,
        messages: mockMessages,
        conversations: mockChannelConversations,
        giveaways: mockGiveAwayUsers
    };

    function handleTableSelect(event: any) {
        selectedTableId = event.detail;
    }
</script>

<div class="page-layout">
    <TableList {tables} on:select={handleTableSelect} />
    <div class="content-area">
        <h1>{tables.find(t => t.id === selectedTableId)?.name}</h1>
        <div class="card">
            <DataTable items={dataConfig[selectedTableId]} columns={columnConfig[selectedTableId]} />
        </div>
    </div>
</div>

<style>
    .page-layout {
        display: grid;
        grid-template-columns: 300px 1fr;
        gap: 2rem;
        height: 100%;
        align-items: start;
    }

    .content-area {
        padding: 1.5rem;
    }

    h1 {
        font-size: 2rem;
        margin-bottom: 1.5rem;
    }

    .card {
        background-color: var(--card-background, white);
        border-radius: 12px;
        padding: 1.5rem;
        box-shadow: 0 4px 12px rgba(0,0,0,0.05);
    }
</style>
