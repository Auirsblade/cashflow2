<script setup lang="ts">
import BoardSpace from "@/components/BoardSpace.vue";
import {computed} from "vue";
import type {Space} from "@/types/board.ts";

const spaces = new Array<Space>(
    {
        id: 1,
        title: "Payday"
    },
    {
        id: 2,
        title: "Deal"
    },
    {
        id: 3,
        title: "Market"
    },
    {
        id: 4,
        title: "Deal"
    },
    {
        id: 5,
        title: "Doodad"
    },
    {
        id: 6,
        title: "Deal"
    },
    {
        id: 7,
        title: "Charity"
    },
    {
        id: 8,
        title: "Deal"
    },
    {
        id: 9,
        title: "Payday"
    },
    {
        id: 10,
        title: "Deal"
    },
    {
        id: 11,
        title: "Market"
    },
    {
        id: 12,
        title: "Deal"
    },
    {
        id: 13,
        title: "Doodad"
    },
    {
        id: 14,
        title: "Deal"
    },
    {
        id: 15,
        title: "Baby"
    },
    {
        id: 16,
        title: "Deal"
    },
    {
        id: 17,
        title: "Payday"
    },
    {
        id: 18,
        title: "Deal",
        players: ['radix-icons:sketch-logo']
    },
    {
        id: 19,
        title: "Market"
    },
    {
        id: 20,
        title: "Deal"
    },
    {
        id: 21,
        title: "Doodad"
    },
    {
        id: 22,
        title: "Deal"
    },
    {
        id: 23,
        title: "Downsized"
    },
    {
        id: 24,
        title: "Deal"
    }
);

const sortedSpaces = computed(() => {
    let reSpaces = new Array<Space | undefined>();
    spaces.filter(x => x.id < 8).every(x => reSpaces.push(x));
    reSpaces.push(spaces.find(x => x.id == 24))
    reSpaces.push(spaces.find(x => x.id == 8))
    reSpaces.push(spaces.find(x => x.id == 23))
    reSpaces.push(spaces.find(x => x.id == 9))
    reSpaces.push(spaces.find(x => x.id == 22))
    reSpaces.push(spaces.find(x => x.id == 10))
    reSpaces.push(spaces.find(x => x.id == 21))
    reSpaces.push(spaces.find(x => x.id == 11))
    reSpaces.push(spaces.find(x => x.id == 20))
    reSpaces.push(spaces.find(x => x.id == 12))
    spaces.filter(x => x.id > 12 && x.id < 20).sort((x, y) => y.id - x.id).every(x => reSpaces.push(x));
    return reSpaces;
})

</script>

<template>
    <div class="border-2 dark:border-emerald-300 border-emerald-900 rounded-md m-2 p-2 shadow drop-shadow-md shadow-gray-600
            grid grid-cols-7 grid-rows-7 gap-1">
        <div v-for="(space, i) in sortedSpaces" :key="i">
            <BoardSpace :title="space?.title ?? ''" :player-icons="space?.players"></BoardSpace>
        </div>
        <div class="col-start-2 col-end-7 row-start-2 row-end-7">Center</div>
    </div>
</template>