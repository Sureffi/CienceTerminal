import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'
import svgr from "vite-plugin-svgr";

// https://vite.dev/config/
export default defineConfig(({ mode }) => ({
    plugins: [react(), svgr()],
    resolve: {
        alias: {
            '@': path.resolve(__dirname, './src'),
        },
    },
    server: {
        proxy: {
            '/api': {
                target: 'http://localhost:5149',
                changeOrigin: true,
                secure: false
            },
            '/alert-hub': {
                target: 'http://localhost:5149',
                changeOrigin: true,
                secure: false,
                ws: true
            }
        }
    },
    // Remove all console logs in production builds
    build: {
        minify: 'terser',
        terserOptions: mode === 'production' ? {
            compress: {
                drop_console: true,
                drop_debugger: true
            }
        } : {}
    }
}))
