import { computed, ref, watch } from 'vue'
import { defineStore } from 'pinia'
import type { AssetModel, GameModel, GameResponseModel, PlayerModel, PlayerOptionsModel, ProfessionModel } from "@/apiClient";
import { useSignalR, useSignalRInvoke, useSignalROn } from "@/lib/signalR";

export const useGameStateStore = defineStore('gameState', () => {
    const game = ref<GameModel>();
    const player = ref<PlayerModel>();
    const playerOptions = ref<PlayerOptionsModel>();
    const myTurn = ref(true);

    const { start, connection, status } = useSignalR(import.meta.env.VITE_API_URL.concat("/gameHub"));

    const { execute: invokeCreateGame, data: newGame } = useSignalRInvoke(connection, 'CreateGame');
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

    const { execute: invokeJoinGame, data: joinedGame } = useSignalRInvoke(connection, 'JoinGame');
    watch(joinedGame, (gameResponse: GameResponseModel) => {
        if (gameResponse.isSuccess && gameResponse.game && gameResponse.player) {
            game.value = gameResponse.game;
            player.value = gameResponse.player;
            playerOptions.value = gameResponse.playerOptions;
            myTurn.value = gameResponse.game.currentPlayerId == player.value?.id;
        } else {
            console.log("Failed to join game");
            console.log(gameResponse.message);
        }
    });

    async function joinGame(playerName: string, gameCode: string) {
        await invokeJoinGame(playerName.trim(), gameCode.trim());
    }

    const { execute: invokeSelectProfession } = useSignalRInvoke(connection, 'SelectProfession');

    async function selectProfession(profession: ProfessionModel) {
        await invokeSelectProfession(game.value?.code, player.value?.id, profession);
    }

    const { execute: invokeMovePlayer } = useSignalRInvoke(connection, 'MovePlayer');

    async function movePlayer(spacesToMove: number) {
        await invokeMovePlayer(game.value?.code, player.value?.id, spacesToMove);
    }

    const { execute: invokeEndTurn } = useSignalRInvoke(connection, 'EndTurn');

    async function endTurn() {
        await invokeEndTurn(game.value?.code, player.value?.id);
    }

    const { execute: invokeBuyCharity } = useSignalRInvoke(connection, 'BuyCharity');

    async function buyCharity() {
        await invokeBuyCharity(game.value?.code, player.value?.id);
    }

    const { execute: invokeGetDeal } = useSignalRInvoke(connection, 'GetDeal');

    async function getDeal(isBig: boolean) {
        await invokeGetDeal(game.value?.code, player.value?.id, isBig);
    }

    const { execute: invokeBuyDeal } = useSignalRInvoke(connection, 'BuyDeal');

    async function buyDeal() {
        await invokeBuyDeal(game.value?.code, player.value?.id);
    }

    const { execute: invokeSellDeal } = useSignalRInvoke(connection, 'SellDeal');

    async function sellDeal() {
        await invokeSellDeal(game.value?.code, player.value?.id);
    }

    const { execute: invokeSellToMarket } = useSignalRInvoke(connection, 'SellToMarket');

    async function sellToMarket(asset: AssetModel) {
        await invokeSellToMarket(game.value?.code, player.value?.id, asset?.id);
    }

    const { execute: invokeMarketPass } = useSignalRInvoke(connection, 'MarketPass');

    async function marketPass() {
        await invokeMarketPass(game.value?.code, player.value?.id);
    }

    useSignalROn(connection, 'GameStateUpdated', ([gameModel]: [GameModel | undefined]
    ) => {
        if (gameModel) {
            game.value = gameModel;
            player.value = gameModel.players?.find(x => x.id == player.value?.id)
            myTurn.value = game.value.currentPlayerId == player.value?.id;
        } else {
            console.log("No game state received");
        }
    });

    useSignalROn(connection, 'Error', ([message]: [string]) => {
        console.log("Error received from server:");
        console.log(message);
    });

    return {
        game,
        player,
        playerOptions,
        myTurn,
        createGame,
        joinGame,
        selectProfession,
        movePlayer,
        endTurn,
        buyCharity,
        getDeal,
        buyDeal,
        sellDeal,
        sellToMarket,
        marketPass
    }
})
