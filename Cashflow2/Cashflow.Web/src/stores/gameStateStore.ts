import { computed, ref, watch } from 'vue'
import { defineStore } from 'pinia'
import type { AssetModel, GameModel, GameResponseModel, PlayerModel, PlayerOptionsModel, ProfessionModel } from "@/apiClient";
import { useSignalR, useSignalRInvoke, useSignalROn, HubConnectionState } from "@/lib/signalR";

export const useGameStateStore = defineStore('gameState', () => {
    const game = ref<GameModel>();
    const player = ref<PlayerModel>();
    const playerOptions = ref<PlayerOptionsModel>();
    const myTurn = ref(false);
    const error = ref<string | null>(null);

    const { start, connection, status } = useSignalR(import.meta.env.VITE_API_URL.concat("/gameHub"));

    const { execute: invokeCreateGame, data: newGame } = useSignalRInvoke(connection, 'CreateGame');
    watch(newGame, (gameResponse: GameResponseModel) => {
        if (gameResponse.isSuccess && gameResponse.game && gameResponse.player) {
            game.value = gameResponse.game;
            player.value = gameResponse.player;
            playerOptions.value = gameResponse.playerOptions;
            myTurn.value = gameResponse.game.currentPlayerId == player.value?.id;
            localStorage.setItem('cf2_session', JSON.stringify({ gameCode: gameResponse.game.code, playerName: gameResponse.player.name }));
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
            localStorage.setItem('cf2_session', JSON.stringify({ gameCode: gameResponse.game.code, playerName: gameResponse.player.name }));
        } else {
            console.log("Failed to join game");
            console.log(gameResponse.message);
            localStorage.removeItem('cf2_session');
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

    const { execute: invokePlaceBid } = useSignalRInvoke(connection, 'PlaceBid');

    async function placeBid(bidAmount: number) {
        await invokePlaceBid(game.value?.code, player.value?.id, bidAmount);
    }

    const { execute: invokeAuctionPass } = useSignalRInvoke(connection, 'AuctionPass');

    async function auctionPass() {
        await invokeAuctionPass(game.value?.code, player.value?.id);
    }

    const { execute: invokeMarketPass } = useSignalRInvoke(connection, 'MarketPass');

    async function marketPass() {
        await invokeMarketPass(game.value?.code, player.value?.id);
    }

    const { execute: invokePayDoodad } = useSignalRInvoke(connection, 'PayDoodad');

    async function payDoodad(useCard: boolean) {
        await invokePayDoodad(game.value?.code, player.value?.id, useCard);
    }

    const { execute: invokeSetEmoji } = useSignalRInvoke(connection, 'SetEmoji');

    async function setEmoji(emoji: string) {
        await invokeSetEmoji(game.value?.code, player.value?.id, emoji);
    }

    const { execute: invokeRemovePlayer } = useSignalRInvoke(connection, 'RemovePlayer');

    async function removePlayer(targetPlayerId: string) {
        await invokeRemovePlayer(game.value?.code, player.value?.id, targetPlayerId);
    }

    const { execute: invokeLeaveGame } = useSignalRInvoke(connection, 'LeaveGame');

    async function leaveGame() {
        await invokeLeaveGame(game.value?.code, player.value?.id);
        game.value = undefined;
        player.value = undefined;
        playerOptions.value = undefined;
        myTurn.value = false;
        localStorage.removeItem('cf2_session');
    }

    const { execute: invokeBuyStock } = useSignalRInvoke(connection, 'BuyStock');

    async function buyStock(ticker: string, quantity: number) {
        await invokeBuyStock(game.value?.code, player.value?.id, ticker, quantity);
    }

    const { execute: invokeSellStock } = useSignalRInvoke(connection, 'SellStock');

    async function sellStock(ticker: string, quantity: number) {
        await invokeSellStock(game.value?.code, player.value?.id, ticker, quantity);
    }

    const { execute: invokeTakeOutLoan } = useSignalRInvoke(connection, 'TakeOutLoan');

    async function takeOutLoan(amount: number, term: number) {
        await invokeTakeOutLoan(game.value?.code, player.value?.id, amount, term);
    }

    const { execute: invokePayOffLoan } = useSignalRInvoke(connection, 'PayOffLoan');

    async function payOffLoan(liabilityId: string, amount: number) {
        await invokePayOffLoan(game.value?.code, player.value?.id, liabilityId, amount);
    }

    async function autoRejoin() {
        const session = localStorage.getItem('cf2_session');
        if (!session) return;

        // Wait for SignalR connection to be ready
        if (status.value !== HubConnectionState.Connected) {
            await new Promise<void>((resolve) => {
                const unwatch = watch(status, (newStatus) => {
                    if (newStatus === HubConnectionState.Connected) {
                        unwatch();
                        resolve();
                    }
                }, { immediate: true });
            });
        }

        try {
            const { gameCode, playerName } = JSON.parse(session);
            if (gameCode && playerName) {
                await joinGame(playerName, gameCode);
            }
        } catch {
            localStorage.removeItem('cf2_session');
        }
    }

    useSignalROn(connection, 'GameStateUpdated', ([gameModel]: [GameModel | undefined]
    ) => {
        if (!gameModel || !player.value?.id) return;

        const updatedPlayer = gameModel.players?.find(x => x.id == player.value?.id);
        if (!updatedPlayer || updatedPlayer.isActive === false) {
            game.value = undefined;
            player.value = undefined;
            playerOptions.value = undefined;
            myTurn.value = false;
            localStorage.removeItem('cf2_session');
            return;
        }
        game.value = gameModel;
        player.value = updatedPlayer;
        myTurn.value = game.value.currentPlayerId == player.value?.id;
    });

    useSignalROn(connection, 'Error', ([message]: [string]) => {
        console.log("Error received from server:", message);
        error.value = message;
        setTimeout(() => { error.value = null; }, 5000);
    });

    return {
        game,
        player,
        playerOptions,
        myTurn,
        error,
        createGame,
        joinGame,
        selectProfession,
        movePlayer,
        endTurn,
        buyCharity,
        getDeal,
        buyDeal,
        sellDeal,
        placeBid,
        auctionPass,
        sellToMarket,
        marketPass,
        buyStock,
        sellStock,
        takeOutLoan,
        payOffLoan,
        payDoodad,
        setEmoji,
        removePlayer,
        leaveGame,
        autoRejoin
    }
})
