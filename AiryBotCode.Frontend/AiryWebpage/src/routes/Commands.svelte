<!-- src/routes/Commands.svelte -->
<script lang="ts">
    import { onMount } from 'svelte';
    import { SearchIcon, RotateCwIcon } from 'svelte-feather-icons';
    import type { CommandConfig } from '../lib/types/database';
    import { loadCommands, saveCommand, reloadBot } from '../lib/api';
    import CommandCard from '../lib/components/CommandCard.svelte';
    import CommandModal from '../lib/components/CommandModal.svelte';

    let commands: CommandConfig[] = [];
    let searchTerm = '';
    let loading = true;
    let live = false;

    let selected: CommandConfig | null = null; // command shown in the modal

    let reloading = false;
    let reloadMsg = '';

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

    function openCommand(event: CustomEvent<CommandConfig>) {
        selected = event.detail;
    }

    async function handleSave(event: CustomEvent<CommandConfig>) {
        const updated = event.detail;
        const index = commands.findIndex((c) => c.commandName === updated.commandName);
        if (index !== -1) {
            commands[index] = updated;
            commands = commands;
        }
        if (live) await saveCommand(updated);
        selected = null;
    }

    async function handleReload() {
        reloading = true;
        reloadMsg = '';
        const ok = await reloadBot();
        reloading = false;
        reloadMsg = ok
            ? 'Reload requested — the bot will restart shortly.'
            : 'Reload request failed (API unreachable).';
        setTimeout(() => (reloadMsg = ''), 5000);
    }
</script>

<div class="commands-page">
    <header class="page-header">
        <div>
            <h1>Commands</h1>
            <p>Click a command to edit its settings. Reloadable settings apply automatically; others need a bot reload.</p>
        </div>
        <div class="header-actions">
            <div class="search-container">
                <SearchIcon size="18" />
                <input type="text" placeholder="Search commands..." bind:value={searchTerm} />
            </div>
            <button class="reload-btn" on:click={handleReload} disabled={reloading || !live} title={live ? 'Restart the bot to apply token / id / name and all settings' : 'API unreachable'}>
                <RotateCwIcon size="16" /> {reloading ? 'Reloading…' : 'Reload bot'}
            </button>
        </div>
    </header>

    {#if reloadMsg}
        <p class="reload-msg">{reloadMsg}</p>
    {/if}

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
                <CommandCard {command} on:open={openCommand} />
            {/each}
        </div>
    {:else}
        <p class="empty">No commands match "{searchTerm}".</p>
    {/if}
</div>

{#if selected}
    <CommandModal command={selected} on:save={handleSave} on:close={() => (selected = null)} />
{/if}

<style>
    .commands-page { padding: 2rem; }

    .page-header {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 1rem;
        flex-wrap: wrap;
        margin-bottom: 2rem;
    }
    h1 { font-size: 1.875rem; margin: 0; }
    .page-header p { margin: 0.35rem 0 0; color: #6b7280; max-width: 540px; }

    .header-actions { display: flex; align-items: center; gap: 0.75rem; flex-wrap: wrap; }

    .search-container {
        display: flex; align-items: center; gap: 0.5rem;
        padding: 0 0.75rem; border-radius: 8px;
        border: 1px solid var(--border-color, #e0e0e0);
        background-color: white; min-width: 220px;
    }
    .search-container :global(svg) { color: #999; flex-shrink: 0; }
    .search-container input {
        width: 100%; border: none; outline: none; padding: 0.6rem 0;
        font-size: 0.95rem; background-color: transparent;
    }

    .reload-btn {
        display: inline-flex; align-items: center; gap: 0.5rem;
        border: none; border-radius: 8px; padding: 0.65rem 1.1rem;
        font-size: 0.95rem; font-weight: 600; cursor: pointer;
        background-color: #4338ca; color: white; white-space: nowrap;
        transition: filter 0.15s ease;
    }
    .reload-btn:hover:not(:disabled) { filter: brightness(1.08); }
    .reload-btn:disabled { opacity: 0.5; cursor: default; }

    .reload-msg {
        margin: 0 0 1.25rem; padding: 0.7rem 1rem; border-radius: 8px;
        background: #eef2ff; color: #3730a3; border: 1px solid #c7d2fe; font-size: 0.9rem;
    }

    .card-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
        gap: 1.25rem;
    }

    .empty { color: #9ca3af; font-style: italic; }

    .notice {
        margin: 0 0 1.5rem; padding: 0.75rem 1rem; border-radius: 8px;
        background-color: #fff7ed; color: #9a3412; border: 1px solid #fed7aa; font-size: 0.9rem;
    }

    @media (max-width: 768px) {
        .commands-page { padding: 1rem; }
        h1 { font-size: 1.5rem; }
        .page-header { margin-bottom: 1.25rem; }
        .header-actions { width: 100%; }
        .search-container { flex: 1; min-width: 0; }
        .reload-btn { min-height: 46px; }
        .card-grid { grid-template-columns: 1fr; gap: 1rem; }
    }
</style>
