<!-- src/lib/components/DataTable.svelte -->
<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import StringCell from './StringCell.svelte';
    import NumberCell from './NumberCell.svelte';
    import DateCell from './DateCell.svelte';
    import ActionsCell from './ActionsCell.svelte';

    export let items: any[];
    export let columns: { key: string; label: string; type: 'string' | 'number' | 'date' }[];

    const dispatch = createEventDispatcher();

    function handleEdit(event: any) {
        // will be implemented later
        console.log('Edit:', event.detail);
    }

    function handleDelete(event: any) {
        // will be implemented later
        console.log('Delete:', event.detail);
    }

    function handleAdd() {
        // will be implemented later
        console.log('Add new item');
    }

    function handleDeleteAll() {
        // will be implemented later
        console.log('Delete all items');
    }
</script>

<div class="table-controls">
    <button on:click={handleAdd}>Add New</button>
    <button on:click={handleDeleteAll} class="delete-all">Delete All</button>
</div>

<table>
    <thead>
        <tr>
            {#each columns as column}
                <th>{column.label}</th>
            {/each}
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        {#each items as item (item.id)}
            <tr>
                {#each columns as column}
                    {#if column.type === 'string'}
                        <StringCell value={item[column.key]} />
                    {:else if column.type === 'number'}
                        <NumberCell value={item[column.key]} />
                    {:else if column.type === 'date'}
                        <DateCell value={item[column.key]} />
                    {:else}
                        <td>Unsupported type</td>
                    {/if}
                {/each}
                <ActionsCell {item} on:edit={handleEdit} on:delete={handleDelete} />
            </tr>
        {/each}
    </tbody>
</table>

<style>
    table {
        width: 100%;
        border-collapse: collapse;
        margin-top: 1rem;
    }
    th, td {
        border: 1px solid #ddd;
        padding: 8px;
        text-align: left;
    }
    th {
        background-color: #f2f2f2;
    }
    .table-controls {
        display: flex;
        justify-content: space-between;
    }
    .delete-all {
        background-color: #dc3545;
        color: white;
    }
</style>
