<script setup lang="ts">
import {Button} from '@/components/ui/button';
import {Icon} from '@iconify/vue'
import {useColorMode} from "@vueuse/core";
import Board from './components/Board.vue';
import ControlCenter from "@/components/ControlCenter.vue";
import Ticker from "@/components/Ticker.vue";

const mode = useColorMode();

const getGameCode = async () => {
    await fetch()
}
</script>

<template>
    <div class="bg-slate-400 text-slate-900 dark:bg-gray-800  dark:text-blue-300 min-h-dvh">
        <header class="inline-flex items-center w-full">
            <h1 class="text-5xl text-center ml-auto">Cashflow 2</h1>
            <Button @click="mode = mode == 'light' ? 'dark' : 'light'" class="ml-auto shadow-none">
                <Icon icon="radix-icons:moon" class="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0"/>
                <Icon icon="radix-icons:sun" class="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100"/>
            </Button>
        </header>
        <Ticker></Ticker>
        <div class="grid grid-cols-1 lg:grid-cols-2">
            <Board></Board>
            <ControlCenter></ControlCenter>
            <Button @click="getGameCode" class="w-32 h-32 ml-auto shadow-none"/>
        </div>
    </div>
</template>
