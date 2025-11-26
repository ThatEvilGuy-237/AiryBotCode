<!-- src/lib/components/ActionsCell.svelte -->
<script lang="ts">
    import { createEventDispatcher } from 'svelte';

    export let item: any;
    const dispatch = createEventDispatcher();

</script>

<td>
    <button on:click={() => dispatch('edit', item)}>Edit</button>
    <button on:click={() => dispatch('delete', item)}>Delete</button>
</td>

<style>
    button {
        margin-right: 5px;
    }
</style>
