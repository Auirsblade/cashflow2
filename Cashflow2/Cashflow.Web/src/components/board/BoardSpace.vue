<script setup lang="ts">
import { Icon } from "@iconify/vue";
import type {PlayerModel} from "@/apiClient";

defineProps({
    title: { type: String, required: true },
    players: Array<PlayerModel>
});

const spaceStyle = (title: string) => {
    title = title.toUpperCase();
    switch (title){
        case "PAYDAY":
            return { color: 'bg-green-200 dark:bg-green-800', icon: 'bi:cash-coin' };
        case "BABY":
            return { color: 'bg-linear-to-tr from-blue-400 to-pink-400 dark:from-blue-800 dark:to-pink-800', icon: 'mingcute:baby-line' };
        case "DEAL":
            return { color: 'bg-cyan-200 dark:bg-cyan-800', icon: 'mdi:deal-outline' };
        case "DOWNSIZED":
            return { color: 'bg-red-200 dark:bg-red-800', icon: 'solar:course-down-linear' };
        case "MARKET":
            return { color: 'bg-blue-200 dark:bg-blue-800', icon: 'solar:diagram-up-linear' };
        case "DOODAD":
            return { color: 'bg-yellow-200 dark:bg-yellow-800', icon: 'material-symbols:toys-and-games-outline' };
        case "CHARITY":
            return { color: 'bg-pink-200 dark:bg-pink-800', icon: 'mdi:charity' };
        default:
            return { color: 'bg-gray-200 dark:bg-gray-800', icon: 'bi:exclamation-lg' };
    }
}

</script>

<template>
    <div :class="'p-2 text-center rounded-md h-full min-h-16 ' + spaceStyle(title).color">
        <Icon :icon="spaceStyle(title).icon" class="md:hidden w-full h-6" />
        <div class="max-md:hidden">{{ title }}</div>
        <span v-for="player in players" :key="player.id" class="inline-block text-[0.5rem] w-3.5 h-3.5 bg-gray-200 dark:bg-gray-900 rounded-full mx-0.25">
            {{ player.name![0] }}
        </span>
    </div>
</template>