<script setup lang="ts">
import {computed} from "vue";
import type {PropType} from "vue";
import type {BoardSpaceModel} from "@/models/BoardSpaceModel.ts";
import BoardSpace from '@/components/BoardSpace.vue'

const { spaces } = defineProps({
    spaces: Object as PropType<Array<BoardSpaceModel> | null>
})

const sortedSpaces = computed(() => {
    let reSpaces = new Array<BoardSpaceModel | undefined>();
    if (spaces){
        spaces?.filter(x => x.id! < 8).every(x => reSpaces.push(x));
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
        spaces.filter(x => x.id! > 12 && x.id! < 20).sort((x, y) => y.id! - x.id!).every(x => reSpaces.push(x));
    }
    return reSpaces;
})

</script>

<template>
    <div class="border-2 dark:border-emerald-300 border-emerald-900 rounded-md m-2 p-2 shadow drop-shadow-md shadow-gray-600
            grid grid-cols-7 grid-rows-7 gap-1">
        <div v-for="(space, i) in sortedSpaces" :key="i">
            <BoardSpace :title="space?.name ?? ''"></BoardSpace>
        </div>
        <div class="col-start-2 col-end-7 row-start-2 row-end-7">Center</div>
    </div>
</template>