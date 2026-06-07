<!-- src/lib/components/CommandCard.svelte -->
<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import { SaveIcon, ZapIcon, RefreshCwIcon } from 'svelte-feather-icons';
    import type { CommandConfig } from '../types/database';

    // A working copy of one command's config. The parent owns the source list;
    // this card edits a local clone and emits `save` with the updated config.
    export let command: CommandConfig;

    const dispatch = createEventDispatcher();

    // Deep clone so edits don't mutate the parent's data until saved.
    let draft: CommandConfig = structuredClone(command);
    let showSuccess = false;
    let invalidJson: Record<string, boolean> = {};
    let successTimeout: number;

    // Re-clone if the parent swaps in a different command object.
    $: if (command.commandName !== draft.commandName) {
        draft = structuredClone(command);
        showSuccess = false;
        invalidJson = {};
    }

    function validateJson(key: string, value: string) {
        try {
            JSON.parse(value);
            invalidJson[key] = false;
        } catch {
            invalidJson[key] = true;
        }
        invalidJson = invalidJson;
    }

    function save() {
        // Block save if any JSON field is malformed.
        if (Object.values(invalidJson).some(Boolean)) return;

        dispatch('save', structuredClone(draft));
        showSuccess = true;
        clearTimeout(successTimeout);
        successTimeout = setTimeout(() => (showSuccess = false), 3000);
    }
</script>

<div class="command-card">
    <header class="card-header">
        <div class="title-block">
            <h3>{draft.displayName}</h3>
            <code class="slug">/{draft.slug}</code>
        </div>
        <p class="description">{draft.description}</p>
    </header>

    <form class="card-body" on:submit|preventDefault={save}>
        {#each draft.settings as setting (setting.key)}
            <div class="form-group">
                <div class="label-row">
                    <label for="{draft.commandName}-{setting.key}">{setting.key}</label>
                    <span
                        class="badge"
                        class:reloadable={setting.isReloadable}
                        title={setting.isReloadable
                            ? 'Reloadable: applied after a reload'
                            : 'Live: applied immediately'}
                    >
                        {#if setting.isReloadable}
                            <RefreshCwIcon size="12" /> reloadable
                        {:else}
                            <ZapIcon size="12" /> live
                        {/if}
                    </span>
                </div>

                {#if setting.uiHint === 'textarea'}
                    <textarea
                        id="{draft.commandName}-{setting.key}"
                        rows="4"
                        bind:value={setting.value}
                    ></textarea>
                {:else if setting.uiHint === 'json'}
                    <textarea
                        id="{draft.commandName}-{setting.key}"
                        class="json"
                        class:invalid={invalidJson[setting.key]}
                        rows="5"
                        bind:value={setting.value}
                        on:input={() => validateJson(setting.key, setting.value)}
                    ></textarea>
                    {#if invalidJson[setting.key]}
                        <span class="json-error">Invalid JSON</span>
                    {/if}
                {:else if setting.uiHint === 'number'}
                    <input
                        id="{draft.commandName}-{setting.key}"
                        type="number"
                        bind:value={setting.value}
                    />
                {:else if setting.uiHint === 'boolean'}
                    <label class="switch">
                        <input
                            id="{draft.commandName}-{setting.key}"
                            type="checkbox"
                            checked={setting.value === 'true'}
                            on:change={(e) => (setting.value = e.currentTarget.checked ? 'true' : 'false')}
                        />
                        <span>{setting.value === 'true' ? 'Enabled' : 'Disabled'}</span>
                    </label>
                {:else}
                    <input
                        id="{draft.commandName}-{setting.key}"
                        type="text"
                        bind:value={setting.value}
                    />
                {/if}

                <p class="help">{setting.description}</p>
            </div>
        {/each}

        {#if draft.settings.length === 0}
            <p class="empty">No configurable settings for this command yet.</p>
        {/if}

        <div class="card-actions">
            <button type="submit" class="save-btn">
                <SaveIcon size="16" /> Save
            </button>
            {#if showSuccess}
                <span class="success-message">Saved!</span>
            {/if}
        </div>
    </form>
</div>

<style>
    .command-card {
        background-color: var(--card-background, white);
        border-radius: 12px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
        display: flex;
        flex-direction: column;
        overflow: hidden;
    }

    .card-header {
        padding: 1.25rem 1.5rem;
        border-bottom: 1px solid #eef0f3;
    }

    .title-block {
        display: flex;
        align-items: center;
        gap: 0.75rem;
    }

    h3 {
        font-size: 1.25rem;
        margin: 0;
    }

    .slug {
        font-size: 0.8rem;
        background-color: #f1f3f5;
        color: #495057;
        padding: 0.15rem 0.5rem;
        border-radius: 6px;
    }

    .description {
        margin: 0.5rem 0 0;
        color: #6b7280;
        font-size: 0.9rem;
    }

    .card-body {
        padding: 1.25rem 1.5rem;
        display: flex;
        flex-direction: column;
        gap: 1.25rem;
    }

    .form-group {
        display: flex;
        flex-direction: column;
        gap: 0.4rem;
    }

    .label-row {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 0.5rem;
    }

    label {
        font-weight: 600;
        color: #374151;
        font-size: 0.95rem;
    }

    .badge {
        display: inline-flex;
        align-items: center;
        gap: 0.25rem;
        font-size: 0.7rem;
        font-weight: 500;
        padding: 0.15rem 0.45rem;
        border-radius: 999px;
        background-color: #fff7ed;
        color: #c2410c;
    }

    .badge.reloadable {
        background-color: #eef2ff;
        color: #4338ca;
    }

    input[type='text'],
    input[type='number'],
    textarea {
        width: 100%;
        box-sizing: border-box;
        padding: 0.55rem;
        border: 1px solid #d1d5db;
        border-radius: 6px;
        font-size: 0.9rem;
        font-family: inherit;
    }

    textarea {
        resize: vertical;
        min-height: 70px;
    }

    textarea.json {
        font-family: ui-monospace, SFMono-Regular, Menlo, monospace;
        font-size: 0.85rem;
    }

    textarea.json.invalid {
        border-color: #dc2626;
        background-color: #fef2f2;
    }

    .json-error {
        color: #dc2626;
        font-size: 0.8rem;
    }

    .switch {
        display: flex;
        flex-direction: row;
        align-items: center;
        gap: 0.5rem;
        font-weight: 400;
        color: #374151;
    }

    .switch input {
        width: 20px;
        height: 20px;
    }

    .help {
        margin: 0;
        color: #9ca3af;
        font-size: 0.8rem;
    }

    .empty {
        color: #9ca3af;
        font-style: italic;
        margin: 0;
    }

    .card-actions {
        display: flex;
        align-items: center;
        gap: 1rem;
        margin-top: 0.25rem;
    }

    .save-btn {
        display: inline-flex;
        align-items: center;
        gap: 0.5rem;
        border: none;
        border-radius: 8px;
        padding: 0.6rem 1.25rem;
        font-size: 0.95rem;
        font-weight: 500;
        cursor: pointer;
        background-color: var(--primary-color, #4a90e2);
        color: white;
        transition: filter 0.2s ease;
    }

    .save-btn:hover {
        filter: brightness(0.95);
    }

    .success-message {
        color: #16a34a;
        font-weight: 500;
    }
</style>
