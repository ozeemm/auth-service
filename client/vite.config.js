import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  preview: {
    allowedHosts: ['client', 'frontend-service', 'localhost'],
    port: 4173
  },
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5127'
      },
      '/swagger': {
        target: 'http://localhost:5127'
      }
    }
  }
})
