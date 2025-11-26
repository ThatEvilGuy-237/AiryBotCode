<!-- src/lib/components/DataTable.svelte -->
<script lang="ts">
    import { createEventDispatcher, tick } from 'svelte';
    import { CheckIcon, XIcon, PlusIcon, Trash2Icon, Edit2Icon } from 'svelte-feather-icons';
    import type { Column } from '../schema';

    export let items: any[];
    export let columns: Column[];

    let editingItem: any = null;
    let originalItem: any = null;
    let isAdding = false;
    let newItem: any = {};

    const dispatch = createEventDispatcher();

    function handleEdit(item: any) {
        originalItem = { ...item };
        editingItem = item;
        isAdding = false;
    }

    function handleCancel() {
        if (editingItem && originalItem) {
            Object.assign(editingItem, originalItem);
        }
        editingItem = null;
        originalItem = null;
    }

    function handleSave() {
        if (!editingItem) return;
        dispatch('update', editingItem);
        editingItem = null;
        originalItem = null;
    }

    async function handleAdd() {
        isAdding = !isAdding;
        newItem = {}; // Reset
        await tick();
        if (isAdding) {
            // Focus logic can be added here if needed
        }
    }

    function handleSaveNew() {
        dispatch('add', newItem);
        isAdding = false;
        newItem = {};
    }
</script>

<div class="table-container">
    <div class="table-controls">
        <button class="add-btn" on:click={handleAdd}>
            <PlusIcon size="18" />
            <span>Add New</span>
        </button>
        <button class="delete-all-btn" on:click={() => dispatch('deleteAll')}>
            <Trash2Icon size="18" />
            <span>Delete All</span>
        </button>
    </div>

    <div class="table-wrapper">
        <table>
            <thead>
                <tr>
                    {#each columns as column}
                        <th>{column.label}</th>
                    {/each}
                    <th class="actions-header">Actions</th>
                </tr>
            </thead>
            <tbody>
                {#if isAdding}
                    <tr class="add-row">
                        {#each columns as column}
                            <td>
                                <input
                                    type={column.type === 'date' ? 'date' : column.type}
                                    placeholder={column.label}
                                    bind:value={newItem[column.key]}
                                />
                            </td>
                        {/each}
                        <td class="actions-cell">
                            <button class="icon-btn save" on:click={handleSaveNew}><CheckIcon size="20" /></button>
                            <button class="icon-btn cancel" on:click={() => isAdding = false}><XIcon size="20" /></button>
                        </td>
                    </tr>
                {/if}

                {#each items as item (item.id)}
                    <tr>
                        {#each columns as column}
                            <td>
                                {#if editingItem?.id === item.id}
                                    {#if column.editable}
                                        <input
                                            type={column.type === 'date' ? 'date' : column.type}
                                            bind:value={editingItem[column.key]}
                                        />
                                    {:else}
                                        {item[column.key]}
                                    {/if}
                                {:else}
                                    {column.type === 'date' ? new Date(item[column.key]).toLocaleDateString() : item[column.key]}
                                {/if}
                            </td>
                        {/each}
                        <td class="actions-cell">
                            {#if editingItem?.id === item.id}
                                <button class="icon-btn save" on:click={handleSave}><CheckIcon size="20" /></button>
                                <button class="icon-btn cancel" on:click={handleCancel}><XIcon size="20" /></button>
                            {:else}
                                <button class="icon-btn edit" on:click={() => handleEdit(item)}><Edit2Icon size="20" /></button>
                                <button class="icon-btn delete" on:click={() => dispatch('delete', item.id)}><Trash2Icon size="20" /></button>
                            {/if}
                        </td>
                    </tr>
                {/each}
            </tbody>
        </table>
    </div>
</div>

<style>
    .table-container {
        background: #fff;
        border-radius: 12px;
        padding: 1.5rem;
    }
    .table-controls {
        display: flex;
        justify-content: space-between;
        margin-bottom: 1.5rem;
    }
    .table-wrapper {
        border: 1px solid #e5e7eb;
        box-shadow: 2px 4px 6px -1px rgb(0 0 0 / 0.02), 0 2px 4px -2px rgb(0 0 0 / 0.01);
        border-radius: 8px;
        overflow: hidden;
    }
    button {
        display: inline-flex;
        align-items: center;
        gap: 0.5rem;
        border: none;
        border-radius: 8px;
        padding: 0.6rem 1.2rem;
        font-size: 0.95rem;
        font-weight: 500;
        cursor: pointer;
        transition: all 0.2s ease;
    }
    .add-btn {
        background-color: var(--primary-color, #4a90e2);
        color: white;
    }
    .add-btn:hover {
        filter: brightness(1.1);
    }
    .delete-all-btn {
        background-color: #e74c3c;
        color: white;
    }
    .delete-all-btn:hover {
        background-color: #c0392b;
    }
    table {
        width: 100%;
        border-collapse: separate;
        border-spacing: 0;
    }
    th {
        background-color: #f9fafb;
        padding: 0.8rem 1rem;
        text-align: left;
        font-weight: 600;
        color: #6b7280;
        border-bottom: 2px solid #e5e7eb;
    }

    td {
        padding: 0.8rem 1rem;
        border-bottom: 1px solid #e5e7eb;
        color: #374151;
        background-color: #fff;
    }
    tr:last-child td {
        border-bottom: none;
    }
    input {
        width: 100%;
        padding: 0.5rem;
        border: 1px solid #d1d5db;
        border-radius: 6px;
        background-color: #f9fafb;
    }
    input:focus {
        outline: 2px solid var(--primary-color, #4a90e2);
        border-color: transparent;
    }
    .actions-header { text-align: right; }
    .actions-cell {
        display: flex;
        justify-content: flex-end;
        gap: 0.5rem;
    }
    .icon-btn {
        background: transparent;
        padding: 0.25rem;
        border-radius: 4px;
    }
    .icon-btn.edit { color: #3b82f6; }
    .icon-btn.delete { color: #ef4444; }
    .icon-btn.save { color: #22c55e; }
    .icon-btn.cancel { color: #848d9a; }
    .icon-btn:hover { background-color: #f3f4f6; }
</style>
