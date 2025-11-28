<!-- src/routes/BotSettings.svelte -->
<script lang="ts">
    import { onMount } from 'svelte';
    import { mockBotSettings } from '../lib/mock';
    import type { BotSetting } from '../lib/types/database';
    import BotList from '../lib/components/BotList.svelte';

    let selectedBotId: number | null = null;
    let selectedBot: BotSetting | null = null;
    let bots = mockBotSettings;
    let showSuccess = false;
    let successTimeout: number;

    function handleBotSelect(event: any) {
        selectedBotId = event.detail;
        showSuccess = false; // Hide message when switching bots
        clearTimeout(successTimeout);
    }

    $: {
        if (selectedBotId) {
            const foundBot = bots.find(b => b.botId === selectedBotId);
            selectedBot = foundBot ? { ...foundBot } : null;
        } else {
            selectedBot = null;
        }
    }

    function saveSettings() {
        if (selectedBot) {
            const index = bots.findIndex(b => b.botId === selectedBot.botId);
            if (index !== -1) {
                bots[index] = selectedBot;
            }
            
            showSuccess = true;
            clearTimeout(successTimeout);
            successTimeout = setTimeout(() => {
                showSuccess = false;
            }, 3000);
        }
    }
    
    onMount(() => {
        if (bots.length > 0) {
            selectedBotId = bots[0].botId;
        }
    });

</script>

<div class="page-layout">
    <BotList {bots} {selectedBotId} on:select={handleBotSelect} />
    <div class="content-area">
        {#if selectedBot}
            <h2>Settings for {selectedBot.botName}</h2>
            <div class="card">
                <form on:submit|preventDefault={saveSettings}>
                    <div class="form-group">
                        <label for="botName">Bot Name</label>
                        <input id="botName" type="text" bind:value={selectedBot.botName}>
                    </div>
                    <div class="form-group">
                        <label for="openAIModel">OpenAI Model</label>
                        <input id="openAIModel" type="text" bind:value={selectedBot.openAIModel}>
                    </div>
                    <div class="form-group">
                        <label for="openAIPrompt">OpenAI Prompt</label>
                        <textarea id="openAIPrompt" rows="4" bind:value={selectedBot.openAIPrompt}></textarea>
                    </div>
                    <div class="form-group">
                        <label for="adminRoleIds">Admin Role IDs</label>
                        <input id="adminRoleIds" type="text" bind:value={selectedBot.adminRoleIds}>
                    </div>
                    <div class="form-group">
                        <label for="logChannelId">Log Channel ID</label>
                        <input id="logChannelId" type="number" bind:value={selectedBot.logChannelId}>
                    </div>
                     <div class="form-group">
                        <label for="evilLogChannelId">Evil Log Channel ID</label>
                        <input id="evilLogChannelId" type="number" bind:value={selectedBot.evilLogChannelId}>
                    </div>
                    <div class="form-group-check">
                        <input id="enabled" type="checkbox" bind:checked={selectedBot.enabled}>
                        <label for="enabled">Enabled</label>
                    </div>
                    <div class="form-actions">
                        <button type="submit" class="save-btn">Save Settings</button>
                        {#if showSuccess}
                            <span class="success-message">Success!</span>
                        {/if}
                    </div>
                </form>
            </div>
        {:else}
            <h2>Select a bot to view its settings.</h2>
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

    .card {
        background-color: var(--card-background, white);
        border-radius: 12px;
        padding: 2rem;
        box-shadow: 0 4px 12px rgba(0,0,0,0.05);
    }

    h2 {
        font-size: 1.75rem;
        margin-bottom: 1.5rem;
        text-align: left;
    }

    .form-group {
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
        margin-bottom: 1.25rem;
    }
    .form-group-check {
        display: flex;
        flex-direction: row;
        gap: 0.5rem;
        margin-bottom: 1.25rem;
        align-items: center;
    }
    label {
        font-weight: 500;
        text-align: left;
        color: #374151;
    }
    input[type="text"], input[type="number"], textarea {
        width: 100%;
        padding: 0.6rem;
        border: 1px solid #d1d5db;
        border-radius: 6px;
    }
    input[type="checkbox"] {
        width: 20px;
        height: 20px;
        order: -1;
    }
    textarea {
        resize: vertical;
        min-height: 80px;
    }
    .form-actions {
        display: flex;
        align-items: center;
        gap: 1rem;
        margin-top: 1rem;
    }
    .save-btn {
        display: inline-flex;
        align-items: center;
        gap: 0.5rem;
        border: none;
        border-radius: 8px;
        padding: 0.75rem 1.5rem;
        font-size: 1rem;
        font-weight: 500;
        cursor: pointer;
        transition: all 0.2s ease;
        background-color: var(--primary-color, #4a90e2);
        color: white;
    }
    .success-message {
        color: #16a34a;
        font-weight: 500;
    }
</style>
