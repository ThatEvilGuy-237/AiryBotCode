<script lang="ts">
  import { Route, router } from "tinro";
  import { MenuIcon } from "svelte-feather-icons";
  import Sidebar from "./lib/components/Sidebar.svelte";
  import Home from "./routes/Home.svelte";
  import Database from "./routes/Database.svelte";
  import BotSettings from "./routes/BotSettings.svelte";
  import Commands from "./routes/Commands.svelte";
  import Login from "./routes/Login.svelte";
  import { isAuthenticated, captureTokenFromHash } from "./lib/auth";

  // If we just came back from the Discord OAuth round-trip, grab the token.
  captureTokenFromHash();

  let navOpen = false;
  const closeNav = () => (navOpen = false);

  // Close the mobile drawer whenever the route changes.
  router.subscribe(() => (navOpen = false));
</script>

{#if $isAuthenticated}
  <div class="shell" class:nav-open={navOpen}>
    <!-- Mobile-only top bar with the menu button. -->
    <header class="topbar">
      <button class="menu-btn" on:click={() => (navOpen = true)} aria-label="Open menu">
        <MenuIcon size="24" />
      </button>
      <span class="brand">Airy Control</span>
    </header>

    <div class="sidebar-slot">
      <Sidebar on:navigate={closeNav} />
    </div>

    {#if navOpen}
      <!-- svelte-ignore a11y-click-events-have-key-events a11y-no-static-element-interactions -->
      <div class="scrim" on:click={closeNav}></div>
    {/if}

    <main class="main-content">
      <Route path="/"><Home /></Route>
      <Route path="/database"><Database /></Route>
      <Route path="/bot-settings"><BotSettings /></Route>
      <Route path="/commands"><Commands /></Route>
    </main>
  </div>
{:else}
  <Login />
{/if}

<style>
  .shell {
    display: grid;
    grid-template-columns: var(--sidebar-width, 250px) 1fr;
    height: 100dvh;
  }

  .topbar { display: none; }

  .sidebar-slot {
    height: 100dvh;
    position: sticky;
    top: 0;
  }

  .main-content {
    background-color: var(--background-color, #f4f7fa);
    overflow-y: auto;
    height: 100dvh;
  }

  .scrim { display: none; }

  /* ---- Mobile: top bar + slide-in drawer ---- */
  @media (max-width: 768px) {
    .shell {
      grid-template-columns: 1fr;
      grid-template-rows: var(--header-height, 56px) 1fr;
      height: 100dvh;
    }

    .topbar {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0 0.75rem;
      background: #2c3e50;
      color: #ecf0f1;
      position: sticky;
      top: 0;
      z-index: 30;
    }
    .menu-btn {
      background: transparent;
      border: none;
      color: inherit;
      display: inline-flex;
      padding: 0.6rem;
      cursor: pointer;
      border-radius: 8px;
    }
    .menu-btn:active { background: rgba(255, 255, 255, 0.12); }
    .brand { font-weight: 700; font-size: 1.1rem; }

    .sidebar-slot {
      position: fixed;
      top: 0;
      left: 0;
      bottom: 0;
      width: 80vw;
      max-width: 300px;
      height: 100dvh;
      transform: translateX(-100%);
      transition: transform 0.25s ease;
      z-index: 50;
      box-shadow: 4px 0 24px rgba(0, 0, 0, 0.25);
    }
    .shell.nav-open .sidebar-slot { transform: translateX(0); }

    .scrim {
      display: block;
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.45);
      z-index: 40;
    }

    .main-content { height: auto; }
  }
</style>
