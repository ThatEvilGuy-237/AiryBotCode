<!-- src/routes/Database.svelte -->
<script lang="ts">
    import DataTable from '../lib/components/DataTable.svelte';
    import { mockChatUsers, mockMessages, mockChannelConversations, mockGiveAwayUsers } from '../lib/mock';

    let activeTab: 'chatUsers' | 'messages' | 'conversations' | 'giveaways' = 'chatUsers';

    const userColumns = [
        { key: 'id', label: 'ID', type: 'number' },
        { key: 'userName', label: 'Username', type: 'string' },
        { key: 'role', label: 'Role', type: 'string' },
        { key: 'createdAt', label: 'Created At', type: 'date' }
    ];

    const messageColumns = [
        { key: 'id', label: 'ID', type: 'number' },
        { key: 'userId', label: 'User ID', type: 'number' },
        { key: 'context', label: 'Content', type: 'string' },
        { key: 'createdAt', label: 'Sent At', type: 'date' }
    ];

    const conversationColumns = [
        { key: 'id', label: 'ID', type: 'number' },
        { key: 'channelId', label: 'Channel ID', type: 'number' },
        { key: 'conversationSummary', label: 'Summary', type: 'string' },
        { key: 'updatedAt', label: 'Last Updated', type: 'date' }
    ];

    const giveawayColumns = [
        { key: 'id', label: 'User ID', type: 'number' },
        { key: 'userName', label: 'Username', type: 'string' },
        { key: 'submitedAt', label: 'Submitted At', type: 'date' }
    ];

</script>

<h1>Database Management</h1>

<div class="tabs">
    <button on:click={() => activeTab = 'chatUsers'} class:active={activeTab === 'chatUsers'}>Chat Users</button>
    <button on:click={() => activeTab = 'messages'} class:active={activeTab === 'messages'}>Messages</button>
    <button on:click={() => activeTab = 'conversations'} class:active={activeTab === 'conversations'}>Conversations</button>
    <button on:click={() => activeTab = 'giveaways'} class:active={activeTab === 'giveaways'}>Giveaways</button>
</div>

<div class="tab-content">
    {#if activeTab === 'chatUsers'}
        <DataTable items={mockChatUsers} columns={userColumns} />
    {:else if activeTab === 'messages'}
        <DataTable items={mockMessages} columns={messageColumns} />
    {:else if activeTab === 'conversations'}
        <DataTable items={mockChannelConversations} columns={conversationColumns} />
    {:else if activeTab === 'giveaways'}
        <DataTable items={mockGiveAwayUsers} columns={giveawayColumns} />
    {/if}
</div>

<style>
    .tabs {
        display: flex;
        border-bottom: 2px solid #ccc;
        margin-bottom: 1rem;
    }
    .tabs button {
        padding: 10px 20px;
        cursor: pointer;
        border: none;
        background-color: transparent;
        font-size: 1rem;
        border-bottom: 2px solid transparent;
        margin-bottom: -2px;
    }
    .tabs button.active {
        border-bottom-color: var(--primary-color, #646cff);
        font-weight: bold;
    }
    .tab-content {
        padding: 1rem;
        border: 1px solid #ddd;
        border-top: none;
        border-radius: 0 0 8px 8px;
    }
</style>
