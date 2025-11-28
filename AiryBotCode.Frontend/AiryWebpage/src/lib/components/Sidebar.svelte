<!-- src/lib/components/Sidebar.svelte -->
<script lang="ts">
    import { HomeIcon, DatabaseIcon, SettingsIcon } from 'svelte-feather-icons';
</script>

<aside class="sidebar">
    <div class="sidebar-header">
        <h2>Airy Control</h2>
    </div>
    <nav class="sidebar-nav">
        <a href="/">
            <HomeIcon size="24" />
            <span>Home</span>
        </a>
        <a href="/database">
            <DatabaseIcon size="24" />
            <span>Database</span>
        </a>
        <a href="/bot-settings">
            <SettingsIcon size="24" />
            <span>Bots Settings</span>
        </a>
    </nav>
</aside>

<style>
    .sidebar {
        display: flex;
        flex-direction: column;
        background-color: #2c3e50;
        color: #ecf0f1;
        height: 100vh;
        padding: 1rem;
        transition: width 0.3s ease;
    }

    .sidebar-header {
        margin-bottom: 2rem;
        text-align: center;
    }

    .sidebar-nav {
        display: flex;
        flex-direction: column;
        gap: 1rem;
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
