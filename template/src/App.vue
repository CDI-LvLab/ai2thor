<script setup lang="ts">
import { onMounted } from 'vue'

import TaskInstructions from './components/TaskInstructions.vue'

window.onUnityMetadata = (data) => {
    window.lastEvent = data
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
            const result = await unityInstance.SendMessage(
                'GameController',
                'Initialize',
                JSON.stringify({ gridSize: 0.25, agentCount: 2 }),
            )
            console.log(result)
        })
        .catch((message) => {
            console.error('Failed to load Unity:', message)
        })
})

const testFunctions = {
    LastEvent: () => {
        console.log(window.lastEvent)
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

    添加第三人称相机: () => {
        window.Unity?.SendMessage(
            'FPSController',
            'Step',
            JSON.stringify({
                action: 'AddThirdPartyCamera',
                position: { x: 0, y: 0, z: 0 },
                rotation: { x: 0, y: 0, z: 0 },
            }),
        )
    },

    多Agent初始化: () => {
        window.Unity?.SendMessage('FPSController', 'SpawnAgent', 0)
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
