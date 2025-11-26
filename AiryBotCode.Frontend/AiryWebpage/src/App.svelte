<script lang="ts">
  import { Route } from "tinro";
  import Sidebar from "./lib/components/Sidebar.svelte";
  import Home from "./routes/Home.svelte";
  import Database from "./routes/Database.svelte";
  import BotSettings from "./routes/BotSettings.svelte";
</script>

<div class="app-layout">
  <Sidebar />
  <main class="main-content">
    <Route path="/"><Home /></Route>
    <Route path="/database"><Database /></Route>
    <Route path="/bot-settings"><BotSettings /></Route>
  </main>
</div>

<style>
  .app-layout {
    display: grid;
    grid-template-columns: var(--sidebar-width, 250px) 1fr;
    height: 100vh;
  }

  .main-content {
    background-color: var(--background-color, #f4f7fa);
    padding: 2rem;
    overflow-y: auto;
    height: 100vh;
  }
</style>