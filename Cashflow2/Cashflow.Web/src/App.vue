<script setup lang="ts">
    import {Button} from '@/components/ui/button';
    import {Input} from '@/components/ui/input'
    import {Icon} from '@iconify/vue'
    import {useColorMode} from "@vueuse/core";
    import Board from "./components/Board.vue";
    import ControlCenter from "@/components/ControlCenter.vue";
    import Ticker from "@/components/Ticker.vue";
    import {ref, watch} from "vue";
    import type {GameModel} from "@/apiClient/models/GameModel.ts";
    import type {PlayerModel} from "@/apiClient/models/PlayerModel.ts";
    import {useSignalR, useSignalRInvoke, useSignalROn} from "@/lib/signalR.js";
    import type {GameResponseModel} from "@/apiClient";

    const mode = useColorMode();

    const game = ref<GameModel>();
    const player = ref<PlayerModel>({} as PlayerModel);
    const playerName = ref<string>();
    const gameCode = ref<string>();

    const {start, connection, status} = useSignalR(import.meta.env.VITE_API_URL.concat("/gameHub"));

    const {execute: createGame, data: newGame} = useSignalRInvoke(connection, 'CreateGame');

    const {execute: joinGame, data: joinedGame} = useSignalRInvoke(connection, 'JoinGame');

    watch(newGame, (gameResponse: GameResponseModel) => {
        if (gameResponse.isSuccess && gameResponse.game && gameResponse.player) {
            game.value = gameResponse.game;
            player.value = gameResponse.player;
        } else {
            console.log("Game failed to start");
            console.log(gameResponse.message);
        }
    });

    watch(joinedGame, (gameResponse: GameResponseModel) => {
        if (gameResponse.isSuccess && gameResponse.game && gameResponse.player) {
            game.value = gameResponse.game;
            player.value = gameResponse.player;
        } else {
            console.log("Failed to join game");
            console.log(gameResponse.message);
        }
    });

    useSignalROn(connection, 'GameStateUpdated', ([gameModel]: [GameModel | undefined]
    ) => {
        if (gameModel) {
            game.value = gameModel;
        } else {
            console.log("No game state received");
        }
    });

    useSignalROn(connection, 'Error', ([message]: [string]) => {
        console.log("Error received from server:");
        console.log(message);
    });

</script>

<template>
    <div class="bg-slate-400 text-slate-900 dark:bg-gray-800  dark:text-blue-300 min-h-dvh">
        <header class="inline-flex items-center w-full">
            <span v-if="game" class="absolute left-0 p-2">Game Code: {{ game.code }} Player: {{ player.name }}</span>
            <h1 class="text-5xl mx-auto">Cashflow 2</h1>
            <Button @click="mode = mode == 'light' ? 'dark' : 'light'" class="ml-auto absolute right-0 shadow-none">
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
                <Button @click="createGame(playerName?.trim())" :disabled="!playerName"
                        class="block mx-auto my-2 h-10 lg:ml-0 w-48 border-2 dark:border-emerald-300 border-emerald-900 rounded-md shadow drop-shadow-md shadow-gray-600">
                    {{ 'Start Game' }}
                </Button>
                <Button @click="joinGame(playerName?.trim(), gameCode?.trim())" :disabled="!playerName || !gameCode"
                        class="block mx-auto my-2 h-10 lg:ml-0 w-48 border-2 dark:border-emerald-300 border-emerald-900 rounded-md shadow drop-shadow-md shadow-gray-600">
                    {{ 'Join Game' }}
                </Button>
            </div>
        </div>
        <div v-else class="grid grid-cols-1 lg:grid-cols-2">
            <Board :game="game"></Board>
            <ControlCenter></ControlCenter>
        </div>
    </div>
</template>
