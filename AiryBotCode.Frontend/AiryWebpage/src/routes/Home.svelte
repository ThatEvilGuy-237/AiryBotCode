<!-- src/routes/Home.svelte -->
<!-- Landing page. Login is handled by the app-wide gate (Login.svelte + auth.ts),
     so this page just welcomes the (already authenticated) user and links out. -->
<script lang="ts">
  import { router } from 'tinro';
  import { TerminalIcon, SettingsIcon, DatabaseIcon } from 'svelte-feather-icons';

  const tiles = [
    { href: '/commands', title: 'Commands', desc: 'Configure each command and reload the bot.', icon: TerminalIcon },
    { href: '/bot-settings', title: 'Bot Settings', desc: 'Bot identity and global configuration.', icon: SettingsIcon },
    { href: '/database', title: 'Database', desc: 'Browse stored users, messages and logs.', icon: DatabaseIcon },
  ];
</script>

<main class="home">
  <header>
    <h1>Airy Control Panel</h1>
    <p>Welcome back. Pick a section to manage your bot.</p>
  </header>

  <div class="tiles">
    {#each tiles as tile}
      <button class="tile" on:click={() => router.goto(tile.href)}>
        <svelte:component this={tile.icon} size="26" />
        <h2>{tile.title}</h2>
        <p>{tile.desc}</p>
      </button>
    {/each}
  </div>
</main>

<style>
  .home { padding: 2.5rem 2rem; max-width: 980px; }
  header h1 { font-size: 1.875rem; margin: 0; }
  header p { margin: 0.4rem 0 2rem; color: #6b7280; }

  .tiles {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
    gap: 1.25rem;
  }
  .tile {
    text-align: left;
    background: var(--card-background, #fff);
    border: 1px solid var(--border-color, #e5e7eb);
    border-radius: 12px;
    padding: 1.5rem;
    cursor: pointer;
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    color: var(--text-color, #333);
    transition: box-shadow 0.15s ease, transform 0.15s ease, border-color 0.15s ease;
    font: inherit;
  }
  .tile:hover {
    box-shadow: 0 8px 22px rgba(0, 0, 0, 0.10);
    transform: translateY(-2px);
    border-color: var(--primary-color, #4a90e2);
  }
  .tile :global(svg) { color: var(--primary-color, #4a90e2); }
  .tile h2 { margin: 0.25rem 0 0; font-size: 1.2rem; }
  .tile p { margin: 0; color: #6b7280; font-size: 0.9rem; }
</style>
