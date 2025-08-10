import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

import path from 'path'
import fs from 'fs'
import fsp from 'fs/promises'

import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { ArcoResolver } from 'unplugin-vue-components/resolvers'
import { vitePluginForArco } from '@arco-plugins/vite-vue'

// This will be where the template is built to!
const buildDirectory = path.resolve(__dirname, '../unity/Assets/WebGLTemplates/HRI')
const unityBuildFolder = path.join(buildDirectory, 'Build')

// Custom plugin to delete the folder
function removeDevUnityBuild(): import('vite').Plugin {
    return {
        name: 'remove-dev-unity-build',
        apply: 'build',
        closeBundle: async () => {
            if (fs.existsSync(unityBuildFolder)) {
                console.log(`Removing dev-time Unity Build folder: ${unityBuildFolder}`)
                await fsp.rm(unityBuildFolder, { recursive: true, force: true })
            }
        },
    }
}

// https://vite.dev/config/
export default defineConfig({
    plugins: [
        vue(),
        vueDevTools(),
        AutoImport({
            resolvers: [ArcoResolver()],
        }),
        Components({
            resolvers: [
                ArcoResolver({
                    sideEffect: true,
                }),
            ],
        }),
        vitePluginForArco({
            style: 'css',
        }),
        removeDevUnityBuild(),
    ],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
        },
    },
    // Build the template directly to its final location
    build: {
        outDir: path.resolve(__dirname, buildDirectory),
        emptyOutDir: true, // Optional: clears the folder before each build
    },
})
