<script setup lang="ts">
import {Button} from '@/components/ui/button';
import {Icon} from '@iconify/vue'
import {useColorMode} from "@vueuse/core";
import Board from './components/Board.vue';
import ControlCenter from "@/components/ControlCenter.vue";
import Ticker from "@/components/Ticker.vue";
import {ref} from "vue";

const mode = useColorMode();

const game = ref();

const getGame = async () => {
    fetch(import.meta.env.VITE_API_URL + "/game/new").then((response) => {
        response.json().then(gameData => {
            game.value = gameData;
        });
    }).catch((err) => {
        console.error(err);
    });
}
</script>

<template>
    <div class="bg-slate-400 text-slate-900 dark:bg-gray-800  dark:text-blue-300 min-h-dvh">
        <header class="inline-flex items-center w-full">
            <span v-if="game" class="absolute left-0 p-2">Game Code: {{ game.code }}</span>
            <h1 class="text-5xl mx-auto">Cashflow 2</h1>
            <Button @click="mode = mode == 'light' ? 'dark' : 'light'" class="ml-auto absolute right-0 shadow-none">
                <Icon icon="radix-icons:moon" class="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0"/>
                <Icon icon="radix-icons:sun" class="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100"/>
            </Button>
        </header>
        <Ticker></Ticker>
        <div v-if="!game" class="flex items-center justify-center h-full">
            <Button @click="getGame"
                    class="w-32 h-32 ml-auto border-2 dark:border-emerald-300 border-emerald-900 rounded-md m-2 p-2 shadow drop-shadow-md shadow-gray-600">
                {{ 'Start Game' }}
            </Button>
        </div>
        <div v-else class="grid grid-cols-1 lg:grid-cols-2">
            <Board :spaces="game.boardSpaces"></Board>
            <ControlCenter></ControlCenter>
        </div>
    </div>
</template>
