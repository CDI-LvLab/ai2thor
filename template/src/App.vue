<script setup lang="ts">
import { onMounted } from 'vue'

import TaskInstructions from './components/TaskInstructions.vue'

window.onUnityMetadata = (data: string) => {
    window.ai2thor.metadata = JSON.parse(data)
}

window.onUnityImage = (key: string, image: Uint8Array) => {
    window.ai2thor.images[key] = image
}

window.onGameLoaded = () => {
    console.log('Game Loaded!')
    // Set controller to enable interaction
    window.Unity?.SendMessage('FPSController', 'SetController', 'HRI')

    window.Unity?.SendMessage(
        'FPSController',
        'Step',
        JSON.stringify({
            action: 'Initialize',
            gridSize: 0.25,
            agentCount: 2,
            fieldOfView: 65,
        }),
    )
}

onMounted(() => {
    if (!window.createUnityInstance) {
        console.error(
            "Can't find createUnityInstance(). Is /Build/Unity.loader.js correctly loaded?",
        )
        return
    }
    const canvas: HTMLCanvasElement | null = document.querySelector('#unity-canvas')
    if (!canvas) {
        console.error('Could not get the canvas element for Unity (#unity-canvas)')
        return
    }
    window
        .createUnityInstance(canvas, {
            dataUrl: `Build/Unity.data`,
            frameworkUrl: 'Build/Unity.framework.js',
            codeUrl: 'Build/Unity.wasm',
            streamingAssetsUrl: 'StreamingAssets',
            companyName: 'CDI',
            productName: 'AI2THOR',
            productVersion: '1.0',
        })
        .then(async (unityInstance: Unity) => {
            console.log('Unity loaded!', unityInstance)
            window.Unity = unityInstance
            window.ai2thor = {
                metadata: {},
                images: {},
            }
        })
        .catch((message) => {
            console.error('Failed to load Unity:', message)
        })
})

const testFunctions = {
    最新数据: () => {
        console.log(window.ai2thor)
    },

    左转: () => {
        window.Unity?.SendMessage(
            'FPSController',
            'Step',
            JSON.stringify({
                action: 'RotateLeft',
                degrees: 30,
            }),
        )
    },

    机器人左转: () => {
        window.Unity?.SendMessage(
            'FPSController',
            'Step',
            JSON.stringify({
                action: 'RotateLeft',
                degrees: 30,
                agentId: 1,
            }),
        )
    },
}
</script>

<template>
    <div class="unity-container">
        <canvas id="unity-canvas"></canvas>
    </div>

    <div class="header-container float top left">
        <div class="header">
            <a-space>
                <a-space>
                    <a-avatar :size="32" :style="{ backgroundColor: 'transparent' }">
                        <img src="@/assets/cdi-logo-black.png" />
                    </a-avatar>
                    <strong>CDI-AI2THOR</strong>
                </a-space>
                <a-button
                    v-for="(action, name) in testFunctions"
                    :key="name"
                    type="text"
                    @click="action"
                    >{{ name }}</a-button
                >
            </a-space>
        </div>
    </div>

    <div class="header-container float bottom left" style="width: 30%">
        <a-collapse expand-icon-position="right">
            <TaskInstructions />
        </a-collapse>
    </div>
</template>

<style scoped>
.header-container {
    margin: 1em;
}

.float {
    position: fixed;
}

.left {
    left: 0px;
}

.right {
    right: 0px;
}

.v-center {
    top: 50%;
    transform: translateY(-50%);
}

.h-center {
    left: 50%;
    transform: translateX(-50%);
}

.top {
    top: 0px;
}

.bottom {
    bottom: 0px;
}

.header {
    border-radius: var(--border-radius-medium);
    background-color: var(--color-bg-1);
    padding: 0.5em;
    max-width: 100%;
}
</style>
