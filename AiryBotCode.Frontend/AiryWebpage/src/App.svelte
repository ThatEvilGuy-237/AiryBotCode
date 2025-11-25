<script lang="ts">
  import { onMount } from 'svelte';

  // --- Configuration ---
  const API_BASE_URL = 'http://localhost:7215';
  const DISCORD_CLIENT_ID = '1318870826862379018';
  const DISCORD_REDIRECT_URI = `${API_BASE_URL}/api/auth/discord/redirect`;
  const DISCORD_SCOPE = 'identify guilds guilds.join connections';
  const DISCORD_AUTH_URL = `https://discord.com/api/oauth2/authorize?client_id=${DISCORD_CLIENT_ID}&redirect_uri=${encodeURIComponent(DISCORD_REDIRECT_URI)}&response_type=code&scope=${encodeURIComponent(DISCORD_SCOPE)}`;

  // --- Reactive State ---
  let token: string | null = null;
  let apiResponse: string = '';

  // --- Functions ---
  function handleLoginLogout() {
    if (token) {
      // Logout
      localStorage.removeItem('jwt');
      token = null;
      apiResponse = 'Logged out.';
    } else {
      // Login
      window.location.href = DISCORD_AUTH_URL;
    }
  }

  async function handlePing() {
    if (!token) {
      apiResponse = 'You must be logged in to ping the API.';
      return;
    }

    apiResponse = 'Pinging...';
    try {
      const response = await fetch(`${API_BASE_URL}/ping`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (response.ok) {
        const data = await response.text();
        apiResponse = `Success! API says: "${data}"`;
      } else {
        apiResponse = `Error: ${response.status} ${response.statusText}`;
        if (response.status === 401) {
          apiResponse += ' (Your token might be expired. Please log out and log in again.)';
          localStorage.removeItem('jwt');
          token = null;
        }
      }
    } catch (error) {
      console.error('Ping failed:', error);
      apiResponse = 'Failed to connect to the API. Is it running?';
    }
  }

  // --- Lifecycle ---
  onMount(() => {
    const fragment = new URLSearchParams(window.location.hash.substring(1));
    const tokenFromRedirect = fragment.get('token');

    if (tokenFromRedirect) {
      localStorage.setItem('jwt', tokenFromRedirect);
      // Clean the URL
      history.replaceState(null, '', window.location.pathname + window.location.search);
    }
    
    // Load token from storage on initial mount
    token = localStorage.getItem('jwt');
  });
</script>

<main>
  <h1>Airy Bot Control Panel</h1>

  <div class="card">
    {#if token}
      <p>You are logged in.</p>
      <button on:click={handleLoginLogout}>Logout</button>
    {:else}
      <p>You are not logged in.</p>
      <button on:click={handleLoginLogout}>Login with Discord</button>
    {/if}
  </div>

  <hr>
  
  <div class="card" style:display={token ? 'block' : 'none'}>
    <h2>Welcome!</h2>
    <p>Your JWT is stored in your browser's local storage.</p>
  </div>

  <hr>

  <h2>API Test</h2>
  <div class="card">
    <button on:click={handlePing} disabled={!token}>Ping API (Requires Login)</button>
    <p>API Response: <code>{apiResponse}</code></p>
  </div>
</main>

<style>
  :root {
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen,
      Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
    --primary-color: #646cff;
  }
  main {
    max-width: 600px;
    margin: 2rem auto;
    padding: 1rem;
    text-align: center;
  }
  .card {
    border: 1px solid #ccc;
    border-radius: 8px;
    padding: 1.5rem;
    margin-bottom: 1rem;
  }
  button {
    background-color: var(--primary-color);
    color: white;
    border: none;
    padding: 0.75rem 1.5rem;
    border-radius: 8px;
    cursor: pointer;
    font-size: 1rem;
  }
  button:hover {
    filter: brightness(1.1);
  }
  button:disabled {
    background-color: #ccc;
    cursor: not-allowed;
  }
  hr {
    border: 0;
    height: 1px;
    background: #eee;
    margin: 2rem 0;
  }
</style>