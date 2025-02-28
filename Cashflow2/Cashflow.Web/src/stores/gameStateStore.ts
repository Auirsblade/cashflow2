import {ref, watch} from 'vue'
import {defineStore} from 'pinia'
import type {GameModel, GameResponseModel, PlayerModel, PlayerOptionsModel, ProfessionModel} from "@/apiClient";
import {useSignalR, useSignalRInvoke, useSignalROn} from "@/lib/signalR";

export const useGameStateStore = defineStore('gameState', () => {
    const game = ref<GameModel>();
    const player = ref<PlayerModel>();
    const playerOptions = ref<PlayerOptionsModel>()

    const {start, connection, status} = useSignalR(import.meta.env.VITE_API_URL.concat("/gameHub"));

    const {execute: invokeCreateGame, data: newGame} = useSignalRInvoke(connection, 'CreateGame');
    watch(newGame, (gameResponse: GameResponseModel) => {
        if (gameResponse.isSuccess && gameResponse.game && gameResponse.player) {
            game.value = gameResponse.game;
            player.value = gameResponse.player;
            playerOptions.value = gameResponse.playerOptions;
        } else {
            console.log("Game failed to start");
            console.log(gameResponse.message);
        }
    });
    async function createGame(playerName: string) {
        await invokeCreateGame(playerName.trim());
    }

    const {execute: invokeJoinGame, data: joinedGame} = useSignalRInvoke(connection, 'JoinGame');
    watch(joinedGame, (gameResponse: GameResponseModel) => {
        if (gameResponse.isSuccess && gameResponse.game && gameResponse.player) {
            game.value = gameResponse.game;
            player.value = gameResponse.player;
            playerOptions.value = gameResponse.playerOptions;
        } else {
            console.log("Failed to join game");
            console.log(gameResponse.message);
        }
    });
    async function joinGame(playerName: string, gameCode: string) {
        await invokeJoinGame(playerName.trim(), gameCode.trim());
    }

    const {execute: invokeSelectProfession} = useSignalRInvoke(connection, 'SelectProfession');
    async function selectProfession(profession: ProfessionModel) {
        await invokeSelectProfession(game.value?.code, player.value, profession);
    }

    useSignalROn(connection, 'GameStateUpdated', ([gameModel]: [GameModel | undefined]
    ) => {
        if (gameModel) {
            game.value = gameModel;
            player.value = gameModel.players?.find(x => x.id == player.value?.id)
        } else {
            console.log("No game state received");
        }
    });

    useSignalROn(connection, 'Error', ([message]: [string]) => {
        console.log("Error received from server:");
        console.log(message);
    });

    return { game, player, playerOptions, createGame, joinGame, selectProfession }
})
