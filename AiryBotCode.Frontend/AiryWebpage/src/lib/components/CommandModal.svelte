<!-- src/lib/components/CommandModal.svelte -->
<!-- Popup editor for a single command's settings. Edits a local clone and emits
     `save` with the updated config; emits `close` to dismiss. -->
<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import { SaveIcon, ZapIcon, RefreshCwIcon, XIcon } from 'svelte-feather-icons';
    import type { CommandConfig } from '../types/database';

    export let command: CommandConfig;

    const dispatch = createEventDispatcher();

    let draft: CommandConfig = structuredClone(command);
    let invalidJson: Record<string, boolean> = {};
    let saving = false;

    function validateJson(key: string, value: string) {
        try { JSON.parse(value); invalidJson[key] = false; }
        catch { invalidJson[key] = true; }
        invalidJson = invalidJson;
    }

    async function save() {
        if (Object.values(invalidJson).some(Boolean)) return;
        saving = true;
        dispatch('save', structuredClone(draft));
    }

    function close() {
        dispatch('close');
    }

    function onKeydown(e: KeyboardEvent) {
        if (e.key === 'Escape') close();
    }
</script>

<svelte:window on:keydown={onKeydown} />

<!-- svelte-ignore a11y-click-events-have-key-events a11y-no-static-element-interactions -->
<div class="overlay" on:click={close}>
    <div class="modal" on:click|stopPropagation role="dialog" aria-modal="true">
        <header class="modal-header">
            <div class="title-block">
                <h2>{draft.displayName}</h2>
                <code class="slug">/{draft.slug}</code>
            </div>
            <button class="close-btn" on:click={close} aria-label="Close"><XIcon size="20" /></button>
        </header>
        <p class="description">{draft.description}</p>

        <form class="modal-body" on:submit|preventDefault={save}>
            {#each draft.settings as setting (setting.key)}
                <div class="form-group">
                    <div class="label-row">
                        <label for="m-{draft.commandName}-{setting.key}">{setting.key}</label>
                        <span
                            class="badge"
                            class:reloadable={setting.isReloadable}
                            title={setting.isReloadable
                                ? 'Applies automatically a few seconds after saving'
                                : 'Applies on the next full bot reload'}
                        >
                            {#if setting.isReloadable}
                                <RefreshCwIcon size="12" /> auto-applies
                            {:else}
                                <ZapIcon size="12" /> needs reload
                            {/if}
                        </span>
                    </div>

                    {#if setting.uiHint === 'textarea'}
                        <textarea id="m-{draft.commandName}-{setting.key}" rows="4" bind:value={setting.value}></textarea>
                    {:else if setting.uiHint === 'json'}
                        <textarea
                            id="m-{draft.commandName}-{setting.key}"
                            class="json" class:invalid={invalidJson[setting.key]} rows="5"
                            bind:value={setting.value}
                            on:input={() => validateJson(setting.key, setting.value)}
                        ></textarea>
                        {#if invalidJson[setting.key]}<span class="json-error">Invalid JSON</span>{/if}
                    {:else if setting.uiHint === 'number'}
                        <input id="m-{draft.commandName}-{setting.key}" type="number" bind:value={setting.value} />
                    {:else if setting.uiHint === 'boolean'}
                        <label class="switch">
                            <input
                                id="m-{draft.commandName}-{setting.key}" type="checkbox"
                                checked={setting.value === 'true'}
                                on:change={(e) => (setting.value = e.currentTarget.checked ? 'true' : 'false')}
                            />
                            <span>{setting.value === 'true' ? 'Enabled' : 'Disabled'}</span>
                        </label>
                    {:else}
                        <input id="m-{draft.commandName}-{setting.key}" type="text" bind:value={setting.value} />
                    {/if}

                    <p class="help">{setting.description}</p>
                </div>
            {/each}

            {#if draft.settings.length === 0}
                <p class="empty">No configurable settings for this command yet.</p>
            {/if}
        </form>

        <footer class="modal-footer">
            <button class="cancel-btn" on:click={close}>Cancel</button>
            <button class="save-btn" on:click={save} disabled={saving}>
                <SaveIcon size="16" /> {saving ? 'Saving…' : 'Save'}
            </button>
        </footer>
    </div>
</div>

<style>
    .overlay {
        position: fixed;
        inset: 0;
        background: rgba(17, 24, 39, 0.55);
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 1.5rem;
        z-index: 100;
    }
    .modal {
        background: var(--card-background, #fff);
        border-radius: 14px;
        width: 100%;
        max-width: 560px;
        max-height: 85vh;
        display: flex;
        flex-direction: column;
        box-shadow: 0 20px 60px rgba(0, 0, 0, 0.25);
    }
    .modal-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 1.25rem 1.5rem 0.5rem;
    }
    .title-block { display: flex; align-items: center; gap: 0.75rem; }
    h2 { margin: 0; font-size: 1.3rem; }
    .slug {
        font-size: 0.8rem; background: #f1f3f5; color: #495057;
        padding: 0.15rem 0.5rem; border-radius: 6px;
    }
    .close-btn {
        background: transparent; border: none; cursor: pointer; color: #6b7280;
        padding: 0.25rem; border-radius: 6px; display: inline-flex;
    }
    .close-btn:hover { background: #f3f4f6; }
    .description { margin: 0; padding: 0 1.5rem 0.5rem; color: #6b7280; font-size: 0.9rem; }
    .modal-body {
        padding: 1rem 1.5rem;
        overflow-y: auto;
        display: flex;
        flex-direction: column;
        gap: 1.25rem;
    }
    .form-group { display: flex; flex-direction: column; gap: 0.4rem; }
    .label-row { display: flex; align-items: center; justify-content: space-between; gap: 0.5rem; }
    label { font-weight: 600; color: #374151; font-size: 0.95rem; }
    .badge {
        display: inline-flex; align-items: center; gap: 0.25rem;
        font-size: 0.7rem; font-weight: 500; padding: 0.15rem 0.45rem;
        border-radius: 999px; background: #fff7ed; color: #c2410c; white-space: nowrap;
    }
    .badge.reloadable { background: #eef2ff; color: #4338ca; }
    input[type='text'], input[type='number'], textarea {
        width: 100%; box-sizing: border-box; padding: 0.55rem;
        border: 1px solid #d1d5db; border-radius: 6px; font-size: 0.9rem; font-family: inherit;
    }
    textarea { resize: vertical; min-height: 70px; }
    textarea.json { font-family: ui-monospace, SFMono-Regular, Menlo, monospace; font-size: 0.85rem; }
    textarea.json.invalid { border-color: #dc2626; background: #fef2f2; }
    .json-error { color: #dc2626; font-size: 0.8rem; }
    .switch { display: flex; align-items: center; gap: 0.5rem; font-weight: 400; color: #374151; }
    .switch input { width: 20px; height: 20px; }
    .help { margin: 0; color: #9ca3af; font-size: 0.8rem; }
    .empty { color: #9ca3af; font-style: italic; margin: 0; }
    .modal-footer {
        display: flex; justify-content: flex-end; gap: 0.75rem;
        padding: 1rem 1.5rem; border-top: 1px solid #eef0f3;
    }
    .cancel-btn {
        background: #fff; border: 1px solid #d1d5db; border-radius: 8px;
        padding: 0.6rem 1.1rem; font-size: 0.95rem; cursor: pointer; color: #374151;
    }
    .cancel-btn:hover { background: #f9fafb; }
    .save-btn {
        display: inline-flex; align-items: center; gap: 0.5rem; border: none;
        border-radius: 8px; padding: 0.6rem 1.25rem; font-size: 0.95rem; font-weight: 500;
        cursor: pointer; background: var(--primary-color, #4a90e2); color: #fff;
    }
    .save-btn:hover { filter: brightness(0.95); }
    .save-btn:disabled { opacity: 0.6; cursor: default; }

    /* On phones the modal becomes a full-screen sheet. */
    @media (max-width: 768px) {
        .overlay { padding: 0; align-items: stretch; }
        .modal {
            max-width: 100%;
            width: 100%;
            max-height: 100dvh;
            height: 100dvh;
            border-radius: 0;
        }
        .modal-footer {
            position: sticky;
            bottom: 0;
            background: var(--card-background, #fff);
            /* keep the Save button clear of the iOS home indicator */
            padding-bottom: calc(1rem + env(safe-area-inset-bottom));
        }
        .save-btn, .cancel-btn { min-height: 48px; }
    }
</style>
