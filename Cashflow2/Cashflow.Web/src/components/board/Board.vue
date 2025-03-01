<script setup lang="ts">
import {computed} from "vue";
import type {BoardSpaceModel} from "@/apiClient";
import BoardSpace from '@/components/board/BoardSpace.vue'
import {useGameStateStore} from "@/stores/gameStateStore.ts";
import {storeToRefs} from "pinia";
import BoardCenter from "@/components/board/BoardCenter.vue";

const gameState = useGameStateStore();
const { game } = storeToRefs(gameState)

const sortedSpaces = computed(() => {
    let reSpaces = new Array<BoardSpaceModel>();
    if (game.value?.boardSpaces){
        let spaces = game.value?.boardSpaces;
        spaces?.filter(x => x.id! < 8).every(x => reSpaces.push(x));
        reSpaces.push(spaces.find(x => x.id == 24)!)
        reSpaces.push(spaces.find(x => x.id == 8)!)
        reSpaces.push(spaces.find(x => x.id == 23)!)
        reSpaces.push(spaces.find(x => x.id == 9)!)
        reSpaces.push(spaces.find(x => x.id == 22)!)
        reSpaces.push(spaces.find(x => x.id == 10)!)
        reSpaces.push(spaces.find(x => x.id == 21)!)
        reSpaces.push(spaces.find(x => x.id == 11)!)
        reSpaces.push(spaces.find(x => x.id == 20)!)
        reSpaces.push(spaces.find(x => x.id == 12)!)
        spaces.filter(x => x.id! > 12 && x.id! < 20).sort((x, y) => y.id! - x.id!).every(x => reSpaces.push(x));
    }
    return reSpaces;
});

const playersOnSpace = (spaceId: number) => {
    return game?.value?.players?.filter(x => x.boardSpaceId == spaceId);
}

</script>

<template>
    <div class="border-2 dark:border-blue-300 border-slate-900 rounded-md m-2 p-2 shadow drop-shadow-md shadow-gray-600
            grid grid-cols-7 grid-rows-7 gap-1">
        <div v-for="space in sortedSpaces" :key="space.id">
            <BoardSpace :title="space.name!" :players="playersOnSpace(space.id!)"></BoardSpace>
        </div>
        <div class="col-start-2 col-end-7 row-start-2 row-end-7"><BoardCenter/></div>
    </div>
</template>