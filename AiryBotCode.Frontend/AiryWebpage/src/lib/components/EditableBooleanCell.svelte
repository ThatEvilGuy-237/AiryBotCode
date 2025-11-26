<!-- src/lib/components/EditableBooleanCell.svelte -->
<script lang="ts">
    export let value: boolean;
    export let isEditing = false;

    function handleChange(e: Event) {
        const target = e.target as HTMLInputElement;
        value = target.checked;
    }
</script>

<td>
    {#if isEditing}
        <input type="checkbox" checked={value} on:change={handleChange} />
    {:else}
        {value ? 'Enabled' : 'Disabled'}
    {/if}
</td>
