<script setup lang="ts">
import { onMounted } from 'vue'

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
        .then((unityInstance) => {
            console.log('Unity loaded!', unityInstance)
        })
        .catch((message) => {
            console.error('Failed to load Unity:', message)
        })
})
</script>

<template>
    <div class="unity-container">
        <canvas id="unity-canvas"></canvas>
    </div>

    <div class="header-container top left">
        <div class="header">
            <a-space>
                <a-space>
                    <a-avatar :size="32" :style="{ backgroundColor: 'transparent' }">
                        <img src="@/assets/cdi-logo-black.png" />
                    </a-avatar>
                    <strong>CDI-AI2THOR</strong>
                </a-space>
                <a-button type="text">完成</a-button>
            </a-space>
        </div>
    </div>

    <div class="header-container bottom left" style="width: 30%">
        <a-collapse expand-icon-position="right">
            <a-collapse-item>
                <template #header> <strong>任务说明</strong> </template>
                <template #expand-icon="{ active }">
                    <a-button type="text" size="small" v-if="active">隐藏</a-button>
                    <a-button type="text" size="small" v-else>显示</a-button>
                </template>
            </a-collapse-item>
        </a-collapse>
    </div>

    <!-- <a-layout-sider
        collapsible
        breakpoint="xxl"
        width="40vw"
        :collapsed-width="48"
    ></a-layout-sider> -->
</template>

<style scoped>
.header-container {
    position: fixed;
    max-width: 40%;
    margin: 1em;
}

.left {
    left: 0px;
}

.right {
    right: 0px;
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
