<script setup lang="ts">
    import { computed, onMounted, ref, watch } from "vue";
    import type { BoardSpaceModel, PlayerModel } from "@/apiClient";
    import BoardSpace from '@/components/board/BoardSpace.vue'
    import { useGameStateStore } from "@/stores/gameStateStore.ts";
    import { storeToRefs } from "pinia";
    import BoardCenter from "@/components/board/BoardCenter.vue";

    const gameState = useGameStateStore();
    const { game } = storeToRefs(gameState);
    const sortedSpaces = ref();


    interface SpaceInfo {
        id: string;
        space: BoardSpaceModel;
        players: Array<PlayerModel>;
    }

    function SpaceInfoById(spaceId: number, spaces: Array<BoardSpaceModel>, players: Array<PlayerModel>) {
        let spaceInfo: SpaceInfo = {} as SpaceInfo;
        let space = spaces.find(x => x.id == spaceId);
        spaceInfo.id = space!.id + players?.map(x => x.id).join('_');
        spaceInfo.space = space!;
        spaceInfo.players = players?.filter(x => x.boardSpaceId == space!.id);
        return spaceInfo;
    }

    function SpaceInfoBySpace(space: BoardSpaceModel, players: Array<PlayerModel>) {
        let spaceInfo: SpaceInfo = {} as SpaceInfo;
        spaceInfo.id = space!.id + players?.map(x => x.id).join('_');
        spaceInfo.space = space!;
        spaceInfo.players = players?.filter(x => x.boardSpaceId == space.id);
        if (spaceInfo.players.length > 0) {
            console.log("found players on space: " + space.id);
        }
        return spaceInfo;
    }

    const getSortedSpaces = (): Array<{ space: BoardSpaceModel, players: Array<PlayerModel> }> => {
        let players = game?.value?.players;

        let reSpaces = new Array<SpaceInfo>();
        if (game.value?.boardSpaces) {
            let spaces = game.value?.boardSpaces;
            spaces?.filter(x => x.id! < 8).every(x => reSpaces.push(SpaceInfoBySpace(x, players ?? [])));
            reSpaces.push(SpaceInfoById(24, spaces, players ?? []));
            reSpaces.push(SpaceInfoById(8, spaces, players ?? []));
            reSpaces.push(SpaceInfoById(23, spaces, players ?? []));
            reSpaces.push(SpaceInfoById(9, spaces, players ?? []));
            reSpaces.push(SpaceInfoById(22, spaces, players ?? []));
            reSpaces.push(SpaceInfoById(10, spaces, players ?? []));
            reSpaces.push(SpaceInfoById(21, spaces, players ?? []));
            reSpaces.push(SpaceInfoById(11, spaces, players ?? []));
            reSpaces.push(SpaceInfoById(20, spaces, players ?? []));
            reSpaces.push(SpaceInfoById(12, spaces, players ?? []));
            spaces.filter(x => x.id! > 12 && x.id! < 20).sort((x, y) => y.id! - x.id!).every(x => reSpaces.push(SpaceInfoBySpace(x, players ?? [])));
        }
        console.log("reloaded spaces");
        return reSpaces;
    }

    onMounted(() => {
        sortedSpaces.value = getSortedSpaces();
    });

    watch(() => game?.value?.players, () => {
        sortedSpaces.value = getSortedSpaces();
    });

</script>

<template>
    <div class="border-2 dark:border-blue-300 border-slate-900 rounded-md m-2 p-2 shadow drop-shadow-md shadow-gray-600
            grid grid-cols-7 grid-rows-7 gap-1">
        <div v-for="spaceInfo in sortedSpaces" :key="spaceInfo.id">
            <BoardSpace :title="spaceInfo.space.name!" :players="spaceInfo.players"></BoardSpace>
        </div>
        <div class="col-start-2 col-end-7 row-start-2 row-end-7">
            <BoardCenter/>
        </div>
    </div>
</template>