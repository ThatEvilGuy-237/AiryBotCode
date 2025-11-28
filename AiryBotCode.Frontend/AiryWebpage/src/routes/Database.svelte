<!-- src/routes/Database.svelte -->
<script lang="ts">
    import { onMount } from 'svelte';
    import DataTable from '../lib/components/DataTable.svelte';
    import TableList from '../lib/components/TableList.svelte';
    import { dbSchema, type TableSchema } from '../lib/schema';
    import * as mockData from '../lib/mock';

    let selectedTable: TableSchema | null = null;
    let activeTableData: any[] = [];

    // This maps schema IDs to the actual mock data arrays
    const mockDataMap = {
        chatUsers: mockData.mockChatUsers,
        messages: mockData.mockMessages,
        conversations: mockData.mockChannelConversations,
        giveaways: mockData.mockGiveAwayUsers
    };

    // "Fetches" data for the selected table
    function loadTableData(tableId: string) {
        selectedTable = dbSchema.find(t => t.id === tableId) || null;
        if (selectedTable) {
            // In a real app, this would be an API call:
            // activeTableData = await api.fetchData(tableId);
            // For now, we copy the mock data to simulate a fresh fetch
            activeTableData = JSON.parse(JSON.stringify(mockDataMap[tableId] || []));
        } else {
            activeTableData = [];
        }
    }

    function handleTableSelect(event: any) {
        loadTableData(event.detail);
    }

    function handleDelete(event: any) {
        const idToDelete = event.detail;
        activeTableData = activeTableData.filter(item => item.id !== idToDelete);
    }

    function handleDeleteAll() {
        activeTableData = [];
    }

    function handleUpdate(event: any) {
        const updatedItem = event.detail;
        const index = activeTableData.findIndex(item => item.id === updatedItem.id);
        if (index !== -1) {
            activeTableData[index] = updatedItem;
            activeTableData = activeTableData; // Trigger reactivity
        }
    }

    function handleAdd(event: any) {
        const newItem = event.detail;
        newItem.id = Math.floor(Math.random() * 10000); 
        activeTableData = [...activeTableData, newItem];
    }

    // Load the first table on initial component mount
    onMount(() => {
        if (dbSchema.length > 0) {
            loadTableData(dbSchema[0].id);
        }
    });
</script>

<div class="page-layout">
    <TableList tables={dbSchema} on:select={handleTableSelect} />
    <div class="content-area">
        {#if selectedTable}
            <h1>{selectedTable.name}</h1>
            <DataTable 
                items={activeTableData} 
                columns={selectedTable.columns}
                on:delete={handleDelete}
                on:deleteAll={handleDeleteAll}
                on:update={handleUpdate}
                on:add={handleAdd}
            />
        {:else}
            <h1>Select a table to begin</h1>
        {/if}
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
        text-align: left;
    }
</style>
