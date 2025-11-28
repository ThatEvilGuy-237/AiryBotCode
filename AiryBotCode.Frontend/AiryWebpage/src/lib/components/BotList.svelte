<!-- src/lib/components/BotList.svelte -->
<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import { SearchIcon } from 'svelte-feather-icons';
    import type { BotSetting } from '../types/database';

    export let bots: BotSetting[];
    export let selectedBotId: number | null;

    let searchTerm = '';
    let filteredBots = bots;

    const dispatch = createEventDispatcher();

    $: {
        if (searchTerm) {
            filteredBots = bots.filter(bot =>
                bot.botName.toLowerCase().includes(searchTerm.toLowerCase())
            );
        } else {
            filteredBots = bots;
        }
    }
</script>

<aside class="bot-list-sidebar">
    <div class="header">
        <h2>Bot List</h2>
    </div>
    <div class="search-container">
        <SearchIcon size="20" />
        <input type="text" placeholder="Search bots..." bind:value={searchTerm} />
    </div>
    <nav>
        <ul>
            {#each filteredBots as bot}
                <li>
                    <button 
                        class:active={bot.botId === selectedBotId}
                        on:click={() => dispatch('select', bot.botId)}
                    >
                        <img src={bot.avatarUrl} alt="{bot.botName} avatar" class="avatar" />
                        <span>{bot.botName}</span>
                    </button>
                </li>
            {/each}
        </ul>
    </nav>
</aside>

<style>
    .bot-list-sidebar {
        background-color: #ffffff;
        padding: 1.5rem;
        border-right: 1px solid var(--border-color, #e0e0e0);
        height: 100%;
    }

    .header {
        padding: 0 1rem;
        margin-bottom: 1.5rem;
    }

    h2 {
        font-size: 1.5rem;
    }
    
    .search-container {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding: 0 0.75rem;
        margin-bottom: 1.5rem;
        border-radius: 8px;
        border: 1px solid var(--border-color, #e0e0e0);
        background-color: white;
    }
    
    .search-container :global(svg) {
        color: #999;
        flex-shrink: 0;
    }

    input[type="text"] {
        width: 100%;
        border: none;
        outline: none;
        padding: 0.75rem 0;
        font-size: 1rem;
        background-color: transparent;
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
        font-weight: 500;
    }

    button:hover {
        background-color: #e9ecef;
    }

    button.active {
        background-color: #dbeafe; /* A light blue */
        color: #1d4ed8; /* A darker blue for text */
    }

    .avatar {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        object-fit: cover;
    }
</style>
