<!-- src/routes/Commands.svelte -->
<script lang="ts">
    import { onMount } from 'svelte';
    import { SearchIcon } from 'svelte-feather-icons';
    import type { CommandConfig } from '../lib/types/database';
    import { loadCommands, saveCommand } from '../lib/api';
    import CommandCard from '../lib/components/CommandCard.svelte';

    let commands: CommandConfig[] = [];
    let searchTerm = '';
    let loading = true;
    let live = false; // true when settings came from the API, false on local fallback

    onMount(async () => {
        const result = await loadCommands();
        commands = result.commands;
        live = result.live;
        loading = false;
    });

    $: filtered = searchTerm
        ? commands.filter(
              (c) =>
                  c.displayName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                  c.slug.toLowerCase().includes(searchTerm.toLowerCase())
          )
        : commands;

    async function handleSave(event: CustomEvent<CommandConfig>) {
        const updated = event.detail;
        const index = commands.findIndex((c) => c.commandName === updated.commandName);
        if (index !== -1) {
            commands[index] = updated;
            commands = commands; // trigger reactivity
        }
        if (live) {
            await saveCommand(updated);
        }
    }
</script>

<div class="commands-page">
    <header class="page-header">
        <div>
            <h1>Commands</h1>
            <p>Configure each command's live and reloadable settings.</p>
        </div>
        <div class="search-container">
            <SearchIcon size="18" />
            <input type="text" placeholder="Search commands..." bind:value={searchTerm} />
        </div>
    </header>

    {#if !loading && !live}
        <p class="notice">
            Showing local defaults — the settings API isn't reachable, so edits won't be saved.
        </p>
    {/if}

    {#if loading}
        <p class="empty">Loading commands…</p>
    {:else if filtered.length > 0}
        <div class="card-grid">
            {#each filtered as command (command.commandName)}
                <CommandCard {command} on:save={handleSave} />
            {/each}
        </div>
    {:else}
        <p class="empty">No commands match "{searchTerm}".</p>
    {/if}
</div>

<style>
    .commands-page {
        padding: 2rem;
    }

    .page-header {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 1rem;
        flex-wrap: wrap;
        margin-bottom: 2rem;
    }

    h1 {
        font-size: 1.875rem;
        margin: 0;
    }

    .page-header p {
        margin: 0.35rem 0 0;
        color: #6b7280;
    }

    .search-container {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        padding: 0 0.75rem;
        border-radius: 8px;
        border: 1px solid var(--border-color, #e0e0e0);
        background-color: white;
        min-width: 240px;
    }

    .search-container :global(svg) {
        color: #999;
        flex-shrink: 0;
    }

    .search-container input {
        width: 100%;
        border: none;
        outline: none;
        padding: 0.6rem 0;
        font-size: 0.95rem;
        background-color: transparent;
    }

    .card-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
        gap: 1.5rem;
        align-items: start;
    }

    .empty {
        color: #9ca3af;
        font-style: italic;
    }

    .notice {
        margin: 0 0 1.5rem;
        padding: 0.75rem 1rem;
        border-radius: 8px;
        background-color: #fff7ed;
        color: #9a3412;
        border: 1px solid #fed7aa;
        font-size: 0.9rem;
    }
</style>
