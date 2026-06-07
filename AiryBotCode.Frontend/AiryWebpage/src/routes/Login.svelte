<script lang="ts">
  import { login } from "../lib/auth";
</script>

<div class="login-screen">
  <div class="login-card">
    <h1>Airy Control Panel</h1>
    <p>Sign in with your Discord account to continue.</p>
    <button class="discord-btn" on:click={login}>
      <svg viewBox="0 0 24 24" width="20" height="20" fill="currentColor" aria-hidden="true">
        <path d="M20.317 4.369A19.79 19.79 0 0 0 16.558 3c-.18.324-.39.762-.535 1.108a18.27 18.27 0 0 0-5.043 0A12.6 12.6 0 0 0 10.44 3 19.74 19.74 0 0 0 6.677 4.37C2.79 10.13 1.79 15.74 2.29 21.27a19.9 19.9 0 0 0 6.06 3.06c.49-.67.926-1.39 1.3-2.14a12.9 12.9 0 0 1-2.05-.99c.17-.13.34-.26.5-.4a14.2 14.2 0 0 0 12.2 0c.16.14.33.27.5.4-.65.39-1.34.72-2.05.99.374.75.81 1.47 1.3 2.14a19.86 19.86 0 0 0 6.06-3.06c.6-6.43-1-12-4.793-16.9ZM9.68 17.34c-1.18 0-2.15-1.08-2.15-2.41 0-1.33.95-2.42 2.15-2.42 1.2 0 2.17 1.09 2.15 2.42 0 1.33-.95 2.41-2.15 2.41Zm4.64 0c-1.18 0-2.15-1.08-2.15-2.41 0-1.33.95-2.42 2.15-2.42 1.2 0 2.17 1.09 2.15 2.42 0 1.33-.95 2.41-2.15 2.41Z"/>
      </svg>
      Login with Discord
    </button>
  </div>
</div>

<style>
  .login-screen {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100dvh;
    width: 100%;
    padding: 1.25rem;
    background-color: var(--background-color, #f4f7fa);
  }
  .login-card {
    background: var(--card-background, #fff);
    border: 1px solid var(--border-color, #e0e0e0);
    border-radius: 14px;
    padding: 40px 48px;
    text-align: center;
    box-shadow: 0 8px 30px rgba(0, 0, 0, 0.08);
    width: 100%;
    max-width: 360px;
  }
  .discord-btn { min-height: 48px; width: 100%; justify-content: center; }
  @media (max-width: 768px) {
    .login-card { padding: 32px 24px; }
  }
  .login-card h1 {
    margin: 0 0 8px;
    color: var(--text-color, #333);
    font-size: 1.5rem;
  }
  .login-card p {
    margin: 0 0 24px;
    color: var(--text-color, #333);
    opacity: 0.7;
  }
  .discord-btn {
    display: inline-flex;
    align-items: center;
    gap: 10px;
    background: #5865f2;
    color: #fff;
    border: none;
    border-radius: 8px;
    padding: 12px 22px;
    font-size: 1rem;
    font-weight: 600;
    cursor: pointer;
    transition: background 0.15s ease;
  }
  .discord-btn:hover {
    background: #4752c4;
  }
</style>
