<!-- src/lib/components/CommandCard.svelte -->
<!-- Uniform, clickable summary card. Clicking it opens the editor modal
     (handled by the parent via the `open` event). -->
<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import { SlidersIcon, RefreshCwIcon, ZapIcon } from 'svelte-feather-icons';
    import type { CommandConfig } from '../types/database';

    export let command: CommandConfig;

    const dispatch = createEventDispatcher();

    $: total = command.settings.length;
    $: autoCount = command.settings.filter((s) => s.isReloadable).length;
    $: reloadCount = total - autoCount;

    function open() {
        dispatch('open', command);
    }
</script>

<!-- svelte-ignore a11y-no-static-element-interactions -->
<button class="command-card" on:click={open}>
    <div class="title-block">
        <h3>{command.displayName}</h3>
        <code class="slug">/{command.slug}</code>
    </div>
    <p class="description">{command.description}</p>

    <div class="meta">
        <span class="settings-count">
            <SlidersIcon size="14" />
            {total} setting{total === 1 ? '' : 's'}
        </span>
        <div class="badges">
            {#if autoCount > 0}
                <span class="badge reloadable" title="{autoCount} auto-applying setting(s)">
                    <RefreshCwIcon size="11" /> {autoCount}
                </span>
            {/if}
            {#if reloadCount > 0}
                <span class="badge" title="{reloadCount} setting(s) that need a bot reload">
                    <ZapIcon size="11" /> {reloadCount}
                </span>
            {/if}
        </div>
    </div>
</button>

<style>
    .command-card {
        /* uniform size for every card */
        height: 168px;
        text-align: left;
        background-color: var(--card-background, white);
        border: 1px solid var(--border-color, #e5e7eb);
        border-radius: 12px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
        padding: 1.25rem 1.5rem;
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
        cursor: pointer;
        transition: box-shadow 0.15s ease, transform 0.15s ease, border-color 0.15s ease;
        font: inherit;
    }
    .command-card:hover {
        box-shadow: 0 8px 22px rgba(0, 0, 0, 0.10);
        transform: translateY(-2px);
        border-color: var(--primary-color, #4a90e2);
    }
    .title-block { display: flex; align-items: center; gap: 0.6rem; }
    h3 {
        font-size: 1.15rem; margin: 0;
        white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
    }
    .slug {
        font-size: 0.75rem; background: #f1f3f5; color: #495057;
        padding: 0.12rem 0.45rem; border-radius: 6px; flex-shrink: 0;
    }
    .description {
        margin: 0;
        color: #6b7280;
        font-size: 0.88rem;
        flex: 1;
        /* clamp to keep card heights identical */
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
    }
    .meta {
        display: flex; align-items: center; justify-content: space-between;
        margin-top: auto;
    }
    .settings-count {
        display: inline-flex; align-items: center; gap: 0.35rem;
        font-size: 0.8rem; color: #6b7280;
    }
    .badges { display: flex; gap: 0.35rem; }
    .badge {
        display: inline-flex; align-items: center; gap: 0.2rem;
        font-size: 0.72rem; font-weight: 600; padding: 0.12rem 0.45rem;
        border-radius: 999px; background: #fff7ed; color: #c2410c;
    }
    .badge.reloadable { background: #eef2ff; color: #4338ca; }
</style>
