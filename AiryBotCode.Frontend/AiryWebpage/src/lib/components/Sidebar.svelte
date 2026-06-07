<!-- src/lib/components/Sidebar.svelte -->
<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import { HomeIcon, DatabaseIcon, SettingsIcon, TerminalIcon, LogOutIcon } from 'svelte-feather-icons';
    import { logout } from '../auth';

    const dispatch = createEventDispatcher();
    const nav = () => dispatch('navigate'); // lets the shell close the mobile drawer
</script>

<aside class="sidebar">
    <div class="sidebar-header">
        <h2>Airy Control</h2>
    </div>
    <nav class="sidebar-nav">
        <a href="/" on:click={nav}>
            <HomeIcon size="24" />
            <span>Home</span>
        </a>
        <a href="/database" on:click={nav}>
            <DatabaseIcon size="24" />
            <span>Database</span>
        </a>
        <a href="/commands" on:click={nav}>
            <TerminalIcon size="24" />
            <span>Commands</span>
        </a>
        <a href="/bot-settings" on:click={nav}>
            <SettingsIcon size="24" />
            <span>Bots Settings</span>
        </a>
    </nav>
    <button class="logout-btn" on:click={logout}>
        <LogOutIcon size="20" />
        <span>Logout</span>
    </button>
</aside>

<style>
    .sidebar {
        display: flex;
        flex-direction: column;
        background-color: #2c3e50;
        color: #ecf0f1;
        height: 100%;
        padding: 1rem;
        overflow-y: auto;
    }

    .sidebar-nav a, .logout-btn {
        min-height: 48px; /* comfortable tap target */
    }

    .sidebar-header {
        margin-bottom: 2rem;
        text-align: center;
    }

    .sidebar-nav {
        display: flex;
        flex-direction: column;
        gap: 1rem;
        flex: 1;
    }

    .logout-btn {
        display: flex;
        align-items: center;
        gap: 1rem;
        color: #ecf0f1;
        background: transparent;
        border: none;
        cursor: pointer;
        padding: 0.75rem 1rem;
        border-radius: 8px;
        font-size: 1rem;
        font-weight: 500;
        transition: background-color 0.2s;
    }

    .logout-btn:hover {
        background-color: #34495e;
    }

    .sidebar-nav a {
        display: flex;
        align-items: center;
        gap: 1rem;
        color: #ecf0f1;
        text-decoration: none;
        padding: 0.75rem 1rem;
        border-radius: 8px;
        transition: background-color 0.2s;
    }

    .sidebar-nav a:hover {
        background-color: #34495e;
    }

    .sidebar-nav a span {
        font-weight: 500;
        font-size: 1rem;
    }
</style>
