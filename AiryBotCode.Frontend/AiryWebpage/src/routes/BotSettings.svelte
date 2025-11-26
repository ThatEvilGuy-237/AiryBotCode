<!-- src/routes/BotSettings.svelte -->
<script lang="ts">
    import { mockBotSettings } from '../lib/mock';
    import type { BotSetting } from '../lib/types/database';

    let selectedBotId: number | null = null;
    let selectedBot: BotSetting | null = null;
    let bots = mockBotSettings;

    $: {
        if (selectedBotId) {
            selectedBot = bots.find(b => b.botId === selectedBotId) || null;
        } else {
            selectedBot = null;
        }
    }

    function saveSettings() {
        if (selectedBot) {
            console.log('Saving settings for:', selectedBot);
            alert('Settings saved! (Check console for details)');
        }
    }
</script>

<h1>Bot Settings</h1>

<div class="settings-container">
    <div class="bot-selector">
        <label for="bot-select">Select a Bot:</label>
        <select id="bot-select" bind:value={selectedBotId}>
            <option value={null}>-- Choose a bot --</option>
            {#each bots as bot}
                <option value={bot.botId}>{bot.botName}</option>
            {/each}
        </select>
    </div>

    {#if selectedBot}
        <div class="settings-form">
            <h2>Settings for {selectedBot.botName}</h2>
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
                    <label for="enabled">Enabled</label>
                    <input id="enabled" type="checkbox" bind:checked={selectedBot.enabled}>
                </div>
                <button type="submit">Save Settings</button>
            </form>
        </div>
    {/if}
</div>

<style>
    .settings-container {
        max-width: 800px;
        margin: 0 auto;
        padding: 1rem;
    }
    .bot-selector {
        margin-bottom: 2rem;
    }
    select {
        padding: 0.5rem;
        font-size: 1rem;
    }
    .settings-form {
        border: 1px solid #ddd;
        border-radius: 8px;
        padding: 2rem;
        background-color: #f9f9f9;
    }
    .form-group {
        display: grid;
        grid-template-columns: 1fr 2fr;
        gap: 1rem;
        margin-bottom: 1rem;
    }
     .form-group-check {
        display: grid;
        grid-template-columns: 1fr 2fr;
        gap: 1rem;
        margin-bottom: 1rem;
        align-items: center;
    }
    label {
        font-weight: bold;
        text-align: left;
    }
    input[type="text"], input[type="number"], textarea, select {
        width: 100%;
        padding: 0.5rem;
        border: 1px solid #ccc;
        border-radius: 4px;
    }
    textarea {
        resize: vertical;
    }
    button {
        padding: 0.75rem 1.5rem;
        background-color: var(--primary-color, #646cff);
        color: white;
        border: none;
        border-radius: 8px;
        cursor: pointer;
        font-size: 1rem;
        margin-top: 1rem;
    }
</style>
