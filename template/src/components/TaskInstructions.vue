<template>
    <a-collapse-item>
        <template #header> <strong>任务说明</strong> </template>
        <template #expand-icon="{ active }">
            <a-button type="text" size="small" v-if="active">隐藏</a-button>
            <a-button type="text" size="small" v-else>显示</a-button>
        </template>
    </a-collapse-item>
</template>
