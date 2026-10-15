import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'

const api = process.env.AIRY_PANEL_API ?? 'http://127.0.0.1:8202'
const build = Date.now().toString(36)

const versionFile = () => ({
  name: 'airy-version',
  generateBundle(this: { emitFile: (f: { type: 'asset'; fileName: string; source: string }) => void }) {
    this.emitFile({ type: 'asset', fileName: 'version.json', source: JSON.stringify({ build }) })
  },
})

export default defineConfig({
  plugins: [vue(), versionFile()],
  define: { __BUILD__: JSON.stringify(build) },
  resolve: { alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) } },
  server: {
    host: '172.17.0.1',
    port: 5173,
    strictPort: true,
    allowedHosts: ['host.docker.internal'],
    proxy: { '/api': { target: api, changeOrigin: true } },
  },
  test: { environment: 'happy-dom' },
})
