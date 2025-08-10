<script setup lang="ts">
import { onMounted } from 'vue'
import { useUrlSearchParams, useWebSocket } from '@vueuse/core'

import TaskInstructions from './components/TaskInstructions.vue'

import { setupUnity, initializeAi2Thor, command } from './utils/ai2thor'
import { sendAi2ThorPacked } from '@/utils/data'
import ServerStatus from './components/ServerStatus.vue'

const { status, send } = useWebSocket('ws://localhost:8000/api/ws', {
    autoReconnect: true,
    heartbeat: {
        message: 'ping',
        interval: 2000,
        pongTimeout: 3000,
    },
})

// Initialize Unity
onMounted(setupUnity)
window.onGameLoaded = initializeAi2Thor
// Handle metadata sent from Unity
window.onUnityMetadata = async (data: string) => {
    window.ai2thor.metadata = data
    await sendAi2ThorPacked(send, window.ai2thor)
}
window.onUnityImage = (key: string, image: Uint8Array) => {
    window.ai2thor.images[key] = image
    // We don't need to send anything here. Images messages are always concluded by a metadata message.
}

const params = useUrlSearchParams('hash-params')
if (!params.experiment) params.experiment = 'demo'
console.log(params)

// Some functions to trigger with buttons.
const testFunctions = {
    最新数据: () => {
        console.log(window.ai2thor)
    },

    左转: () => {
        command('RotateLeft', { degrees: 30 })
    },

    机器人左转: () => {
        command('RotateLeft', {
            degrees: 30,
            agentId: 1,
        })
    },
}

import { useDevicesList, useUserMedia } from '@vueuse/core'
import { computed, reactive } from 'vue'

const { audioInputs: microphones } = useDevicesList({
    requestPermissions: true,
})
const currentMicrophone = computed(() => microphones.value[0]?.deviceId)

const { stream } = useUserMedia({
    constraints: reactive({
        audio: { deviceId: currentMicrophone },
    }),
})
console.log(stream)
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
                <ServerStatus :status="status" />

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
