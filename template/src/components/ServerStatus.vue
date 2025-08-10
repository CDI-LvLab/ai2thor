<script setup lang="ts">
/// <reference types="../../node_modules/.vue-global-types/vue_3.5_0.d.ts" />
import type { WebSocketStatus } from '@vueuse/core'

defineProps<{
    status: WebSocketStatus
}>()

const statusString = {
    CONNECTING: '连接中',
    OPEN: '正常',
    CLOSED: '已断开',
}

const statusColor = {
    CONNECTING: 'gold',
    OPEN: 'green',
    CLOSED: 'red',
}
</script>

<template>
    <a-tag :color="statusColor[status]" :loading="status === 'CONNECTING'">
        服务器{{ statusString[status] }}
    </a-tag>
</template>
