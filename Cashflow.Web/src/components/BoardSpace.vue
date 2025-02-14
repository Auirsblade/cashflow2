<script setup lang="ts">
import { Icon } from "@iconify/vue";

const props = defineProps({
    title: { type: String, required: true },
    playerIcons: Array<string>
});

const spaceStyle = (title: string) => {
    title = title.toUpperCase();
    switch (title){
        case "PAYDAY":
            return ' bg-green-200 dark:bg-green-600';
        default:
            return ' bg-gray-200 dark:bg-gray-600';
    }
}

</script>

<template>
    <div :class="'p-2 text-center rounded-md min-h-16' + spaceStyle(title)">
        <div>{{ title }}</div>
        <span v-for="icon in playerIcons">
            <Icon :icon/>
        </span>
    </div>
</template>

<style scoped>

</style>