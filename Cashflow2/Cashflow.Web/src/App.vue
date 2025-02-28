<script setup lang="ts">
    import {Button} from '@/components/ui/button';
    import {Input} from '@/components/ui/input'
    import {Icon} from '@iconify/vue'
    import {useColorMode} from "@vueuse/core";
    import Board from "./components/Board.vue";
    import ControlCenter from "@/components/ControlCenter.vue";
    import Ticker from "@/components/Ticker.vue";
    import {ref} from "vue";
    import {
        DropdownMenu,
        DropdownMenuTrigger,
        DropdownMenuItem,
        DropdownMenuContent,
        DropdownMenuSeparator,
        DropdownMenuLabel
    } from "@/components/ui/dropdown-menu";
    import {useGameStateStore} from "@/stores/gameStateStore.ts";
    import {storeToRefs} from "pinia";
    import type {ProfessionModel} from "@/apiClient";

    const mode = useColorMode();

    const gameState = useGameStateStore();
    const {player, game, playerOptions} = storeToRefs(gameState);

    const playerName = ref<string>();
    const gameCode = ref<string>();
    const selectedProfession = ref<ProfessionModel>();

</script>

<template>
    <div class="bg-slate-400 text-slate-900 dark:bg-gray-900  dark:text-blue-300 min-h-dvh">
        <header class="inline-flex items-center w-full">
            <div class="absolute left-0 ml-2 mt-2">
                <DropdownMenu v-if="game">
                    <DropdownMenuTrigger>
                        <Icon icon="radix-icons:info-circled" class="h-[1.2rem] w-[1.2rem]"/>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent class="ml-2 bg-slate-400 text-slate-900 dark:bg-gray-900 dark:text-blue-300">
                        <DropdownMenuItem>{{ 'Join Code: ' + game.code }}</DropdownMenuItem>
                        <DropdownMenuSeparator class="bg-slate-900 dark:bg-blue-300"/>
                        <DropdownMenuLabel>Players</DropdownMenuLabel>
                        <DropdownMenuItem v-for="player in game.players" :key="player.id">{{ player.name }}</DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            </div>
            <h1 class="text-5xl mx-auto">Cashflow 2</h1>
            <Button @click="mode = mode == 'light' ? 'dark' : 'light'" class="absolute right-0 shadow-none">
                <Icon icon="radix-icons:moon" class="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0"/>
                <Icon icon="radix-icons:sun" class="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100"/>
            </Button>
        </header>
        <Ticker></Ticker>
        <div v-if="!game" class="content-center grid grid-cols-1 lg:grid-cols-2 w-full gap-x-2 mt-2">
            <div>
                <Input v-model="playerName" placeholder="player name" class="mx-auto lg:mr-0 my-2 w-48"></Input>
                <Input v-model="gameCode" placeholder="game code" class="mx-auto lg:mr-0 my-2 w-48"></Input>
            </div>
            <div>
                <Button @click="gameState.createGame(playerName!)" :disabled="!playerName"
                        class="block mx-auto my-2 h-10 lg:ml-0 w-48 border-2 dark:border-emerald-300 border-emerald-900 rounded-md shadow drop-shadow-md shadow-gray-600">
                    {{ 'Start Game' }}
                </Button>
                <Button @click="gameState.joinGame(playerName!, gameCode!)" :disabled="!playerName || !gameCode"
                        class="block mx-auto my-2 h-10 lg:ml-0 w-48 border-2 dark:border-emerald-300 border-emerald-900 rounded-md shadow drop-shadow-md shadow-gray-600">
                    {{ 'Join Game' }}
                </Button>
            </div>
        </div>
        <div v-else-if="player && !player.profession" class="content-center grid grid-cols-1 lg:grid-cols-2 w-full gap-x-2 mt-2">
            <div class="mx-auto lg:mr-0 my-2 w-48 h-10 border-2 dark:border-emerald-300 border-emerald-900 rounded-md shadow drop-shadow-md shadow-gray-600">
                <DropdownMenu>
                    <DropdownMenuTrigger class="block w-full h-10 mb-0">
                        {{ selectedProfession?.name ?? 'Select Profession' }}
                    </DropdownMenuTrigger>
                    <DropdownMenuContent
                        class="w-48 bg-slate-400 text-slate-900 dark:bg-gray-800 dark:text-blue-300 dark:border-emerald-300 border-emerald-900">
                        <DropdownMenuLabel>Professions</DropdownMenuLabel>
                        <DropdownMenuSeparator class="bg-emerald-900 dark:bg-emerald-300"/>
                        <DropdownMenuItem v-for="(profession, i) in playerOptions?.professions" :key="i" @click="selectedProfession = profession" :class="selectedProfession == profession ? 'font-bold' : ''">
                            {{ profession.name }}
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            </div>
            <div>
                <Button @click="gameState.selectProfession(selectedProfession!)" :disabled="!selectedProfession"
                        class="block mx-auto my-2 h-10 lg:ml-0 w-48 border-2 dark:border-emerald-300 border-emerald-900 rounded-md shadow drop-shadow-md shadow-gray-600">
                    {{ 'Start' }}
                </Button>
            </div>
        </div>
        <div v-else class="grid grid-cols-1 xl:grid-cols-2">
            <Board :game="game"></Board>
            <ControlCenter></ControlCenter>
        </div>
    </div>
</template>
