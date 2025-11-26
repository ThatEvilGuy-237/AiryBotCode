<!-- src/lib/components/DateCell.svelte -->
<script lang="ts">
    export let value: string;
    let formattedDate = new Date(value).toLocaleString();
</script>

<td>{formattedDate}</td>
