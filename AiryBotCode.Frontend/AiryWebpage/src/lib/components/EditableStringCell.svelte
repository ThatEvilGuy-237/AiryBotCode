<!-- src/lib/components/EditableStringCell.svelte -->
<script lang="ts">
    export let value: string;
    export let isEditing = false;

    function handleInput(e: Event) {
        const target = e.target as HTMLInputElement;
        value = target.value;
    }
</script>

<td>
    {#if isEditing}
        <input type="text" {value} on:input={handleInput} />
    {:else}
        {value}
    {/if}
</td>
