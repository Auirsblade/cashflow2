<script setup lang="ts">
    import {Button} from '@/components/ui/button';
    import {Input} from '@/components/ui/input'
    import {Icon} from '@iconify/vue'
    import {useColorMode} from "@vueuse/core";
    import Board from "@/components/board/Board.vue";
    import ControlCenter from "@/components/ControlCenter.vue";
    import Ticker from "@/components/Ticker.vue";
    import {computed, onMounted, onUnmounted, provide, ref} from "vue";
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

    const emojiList = [
        '🚀', '🎮', '🎯', '🎨', '🦊', '🐱', '🐶', '🦄', '🐉', '🌟',
        '🔥', '🌈', '💎', '🎸', '🍕', '🌮', '🧙', '🤖', '🦅', '🐼',
        '🐸', '🦁', '🐵', '🐧', '🦋', '🌻', '👾', '🎩', '🍀', '🎲'
    ]

    const takenEmojis = computed(() => {
        return new Set(
            game.value?.players
                ?.filter(p => p.id !== player.value?.id)
                .map(p => p.emoji)
                .filter(Boolean)
        )
    })

    const isAdmin = computed(() => game.value?.creatorId === player.value?.id);

    onMounted(() => {
        gameState.autoRejoin();
    });

    // Secret dev mode: hold D for 2 seconds to toggle
    const devMode = ref(false);
    provide('devMode', devMode);

    let dHoldTimer: ReturnType<typeof setTimeout> | null = null;

    const onKeyDown = (e: KeyboardEvent) => {
        if (e.key === 'd' || e.key === 'D') {
            if (e.repeat) return;
            dHoldTimer = setTimeout(() => {
                devMode.value = !devMode.value;
                dHoldTimer = null;
            }, 2000);
        }
    };

    const onKeyUp = (e: KeyboardEvent) => {
        if (e.key === 'd' || e.key === 'D') {
            if (dHoldTimer) { clearTimeout(dHoldTimer); dHoldTimer = null; }
        }
    };

    onMounted(() => {
        window.addEventListener('keydown', onKeyDown);
        window.addEventListener('keyup', onKeyUp);
    });

    onUnmounted(() => {
        window.removeEventListener('keydown', onKeyDown);
        window.removeEventListener('keyup', onKeyUp);
    });

</script>

<template>
    <div class="bg-slate-400 text-slate-900 dark:bg-gray-900  dark:text-blue-300 min-h-dvh">
        <header class="inline-flex items-center w-full">
            <div class="absolute left-0 ml-2 mt-2">
                <DropdownMenu v-if="game">
                    <DropdownMenuTrigger class="relative">
                        <Icon icon="radix-icons:info-circled" class="h-[1.2rem] w-[1.2rem]"/>
                        <span v-if="devMode" class="absolute -top-1 -right-1 h-2 w-2 rounded-full bg-red-500"></span>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent class="ml-2 bg-slate-400 text-slate-900 dark:bg-gray-900 dark:text-blue-300">
                        <DropdownMenuItem>{{ 'Join Code: ' + game.code }}</DropdownMenuItem>
                        <DropdownMenuSeparator class="bg-slate-900 dark:bg-blue-300"/>
                        <DropdownMenuLabel>Players</DropdownMenuLabel>
                        <DropdownMenuItem v-for="p in game.players" :key="p.id ?? p.name ?? ''" class="flex items-center justify-between">
                            <span :class="p.isActive === false ? 'opacity-50' : ''">
                                {{ p.emoji }} {{ p.name }}{{ p.isActive === false ? ' (left)' : '' }}
                            </span>
                            <button v-if="isAdmin && p.id !== player?.id && p.isActive !== false"
                                    @click.stop="gameState.removePlayer(p.id!)"
                                    class="ml-2 text-red-500 hover:text-red-700 text-xs">&#10005;</button>
                        </DropdownMenuItem>
                        <DropdownMenuSeparator class="bg-slate-900 dark:bg-blue-300"/>
                        <DropdownMenuItem @click="gameState.leaveGame()" class="text-red-500 hover:text-red-700 cursor-pointer">
                            Leave Game
                        </DropdownMenuItem>
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
                <Button @click="gameCode ? gameState.joinGame(playerName!, gameCode!) : gameState.createGame(playerName!)"
                        :disabled="!playerName || (!!gameCode && gameCode.trim() === '')"
                        class="block mx-auto my-2 h-10 lg:ml-0 w-48 border-2 dark:border-emerald-300 border-emerald-900 rounded-md shadow drop-shadow-md shadow-gray-600">
                    {{ gameCode ? 'Join Game' : 'Start Game' }}
                </Button>
            </div>
        </div>
        <div v-else-if="player && !player.profession" class="content-center w-full mt-2 max-w-md mx-auto">
            <div class="text-center mb-4">
                <div class="text-4xl mb-1">{{ player.emoji || '?' }}</div>
                <div class="text-sm opacity-75">Your Emoji</div>
            </div>
            <div class="flex flex-wrap justify-center gap-1 mb-4 px-2">
                <button
                    v-for="emoji in emojiList"
                    :key="emoji"
                    @click="gameState.setEmoji(emoji)"
                    :disabled="takenEmojis.has(emoji)"
                    :class="[
                        'text-2xl w-10 h-10 rounded-md transition-all',
                        player.emoji === emoji ? 'ring-2 ring-emerald-500 bg-emerald-100 dark:bg-emerald-900' : '',
                        takenEmojis.has(emoji) ? 'opacity-25 cursor-not-allowed' : 'hover:bg-slate-300 dark:hover:bg-gray-700 cursor-pointer'
                    ]"
                >
                    {{ emoji }}
                </button>
            </div>
            <div class="flex flex-col items-center gap-2">
                <div class="w-48 h-10 border-2 dark:border-emerald-300 border-emerald-900 rounded-md shadow drop-shadow-md shadow-gray-600">
                    <DropdownMenu>
                        <DropdownMenuTrigger class="block w-full h-10 mb-0">
                            {{ selectedProfession?.name ?? 'Select Profession' }}
                        </DropdownMenuTrigger>
                        <DropdownMenuContent
                            class="w-48 max-h-60 overflow-y-auto bg-slate-400 text-slate-900 dark:bg-gray-800 dark:text-blue-300 dark:border-emerald-300 border-emerald-900">
                            <DropdownMenuLabel>Professions</DropdownMenuLabel>
                            <DropdownMenuSeparator class="bg-emerald-900 dark:bg-emerald-300"/>
                            <DropdownMenuItem v-for="(profession, i) in playerOptions?.professions" :key="i" @click="selectedProfession = profession" :class="selectedProfession == profession ? 'font-bold' : ''">
                                {{ profession.name }}
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
                <Button @click="gameState.selectProfession(selectedProfession!)" :disabled="!selectedProfession"
                        class="block h-10 w-48 border-2 dark:border-emerald-300 border-emerald-900 rounded-md shadow drop-shadow-md shadow-gray-600">
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
