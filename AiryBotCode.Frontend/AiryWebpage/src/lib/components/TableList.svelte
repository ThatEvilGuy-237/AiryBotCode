<!-- src/lib/components/TableList.svelte -->
<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import { DatabaseIcon, SearchIcon } from 'svelte-feather-icons';

    export let tables: { id: string; name: string }[];
    
    let searchTerm = '';
    let filteredTables = tables;

    const dispatch = createEventDispatcher();

    $: {
        if (searchTerm) {
            filteredTables = tables.filter(table =>
                table.name.toLowerCase().includes(searchTerm.toLowerCase())
            );
        } else {
            filteredTables = tables;
        }
    }
</script>

<aside class="table-list-sidebar">
    <div class="search-container">
        <SearchIcon size="20" class="search-icon" />
        <input type="text" placeholder="Search tables..." bind:value={searchTerm} />
    </div>
    <nav>
        <ul>
            {#each filteredTables as table}
                <li>
                    <button on:click={() => dispatch('select', table.id)}>
                        <DatabaseIcon size="20" />
                        <span>{table.name}</span>
                    </button>
                </li>
            {/each}
        </ul>
    </nav>
</aside>

<style>
    .table-list-sidebar {
        background-color: #ffffff;
        padding: 1.5rem;
        border-right: 1px solid var(--border-color, #e0e0e0);
        height: 100%;
    }

    .search-container {
        position: relative;
        margin-bottom: 1.5rem;
    }

    .search-icon {
        position: absolute;
        left: 10px;
        top: 50%;
        transform: translateY(-50%);
        color: #999;
    }

    input[type="text"] {
        width: 100%;
        padding: 0.75rem 0.75rem 0.75rem 35px;
        border-radius: 8px;
        border: 1px solid var(--border-color, #e0e0e0);
        font-size: 1rem;
    }

    ul {
        list-style: none;
        padding: 0;
        margin: 0;
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
    }

    button {
        display: flex;
        align-items: center;
        gap: 1rem;
        width: 100%;
        padding: 0.75rem 1rem;
        background-color: transparent;
        border: none;
        border-radius: 8px;
        cursor: pointer;
        font-size: 1rem;
        text-align: left;
        color: var(--text-color, #333);
        transition: background-color 0.2s, color 0.2s;
    }

    button:hover {
        background-color: #e9ecef;
    }

    button.active {
        background-color: var(--primary-color, #4a90e2);
        color: white;
    }
</style>
