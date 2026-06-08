import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// Served behind Caddy at /login (handle_path strips the prefix), so all asset
// URLs must be prefixed with /login/ to route back through the login handler.
// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  base: '/login/',
  server: {
    port: 5174,
  },
})
