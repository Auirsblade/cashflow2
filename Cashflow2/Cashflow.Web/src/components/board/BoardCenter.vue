<script setup lang="ts">
    import { Button } from "@/components/ui/button"
    import { Input } from "@/components/ui/input"
    import DiceRoller from "@/components/board/DiceRoller.vue";
    import { storeToRefs } from "pinia";
    import { useGameStateStore } from "@/stores/gameStateStore.ts";
    import { computed, onMounted, ref, watch } from "vue";
    import { formatCurrency } from "@/helpers/FormatHelper.ts";
    import type { AssetModel } from "@/apiClient";
    import {
        DropdownMenu,
        DropdownMenuContent,
        DropdownMenuItem,
        DropdownMenuLabel,
        DropdownMenuSeparator,
        DropdownMenuTrigger
    } from "@/components/ui/dropdown-menu";
    import { Icon } from "@iconify/vue";

    const gameState = useGameStateStore();
    const { game, player, myTurn, error } = storeToRefs(gameState);

    const hasCharity = computed(() => (player.value?.charityTurnsRemaining ?? 0) > 0)
    const diceToRoll = ref(1);
    const readyToRoll = computed(() => {
        return !((player.value?.downsizedTurnsRemaining ?? 0) > 0) && rolled.value == 0 && (!hasCharity.value || (hasCharity.value && !promptSelectCharity.value))
    });
    const playerName = computed(() => myTurn.value ? 'You' : game.value?.players?.find(x => x.id == game.value?.currentPlayerId)?.name)
    const promptSelectCharity = ref(false);
    const rolled = ref(0);

    onMounted(() => {
        if (hasCharity.value) promptSelectCharity.value = true;
    });

    const chooseCharity = (useCharity: boolean) => {
        if (useCharity) diceToRoll.value = 2;
        promptSelectCharity.value = false;
    }

    const movePlayer = (diceRoll: number) => {
        gameState.movePlayer(diceRoll);
        rolled.value = diceRoll;
    }

    const confirmAction = () => {
        gameState.endTurn();
        rolled.value = 0;
        promptSelectCharity.value = true;
    }

    const buyCharity = () => {
        gameState.buyCharity();
        rolled.value = 0;
        promptSelectCharity.value = true;
    }

    const getDeal = (isBig: boolean) => gameState.getDeal(isBig);

    const buyDeal = () => {
        gameState.buyDeal();
        rolled.value = 0;
        promptSelectCharity.value = true;
    }

    const sellDeal = () => gameState.sellDeal();

    const bidAmount = ref<number>(0);

    const isAuctionActive = computed(() => game.value?.dealAction?.auctionState != null);
    const isSeller = computed(() => game.value?.dealAction?.auctionState?.sellerId === player.value?.id);
    const isAuctionComplete = computed(() => game.value?.dealAction?.auctionState?.isComplete === true);
    const hasRespondedToAuction = computed(() => {
        const auction = game.value?.dealAction?.auctionState;
        if (!auction || !player.value?.id) return false;
        const bids = auction.bids as Record<string, number | null> | undefined;
        return bids != null && player.value.id in bids;
    });
    const maxBid = computed(() => {
        const equity = game.value?.dealAction?.asset?.equity ?? 0;
        return (player.value?.cash ?? 0) - equity;
    });
    const isBidValid = computed(() => bidAmount.value > 0 && bidAmount.value <= maxBid.value);

    const auctionResponseCount = computed(() => {
        const bids = game.value?.dealAction?.auctionState?.bids as Record<string, number | null> | undefined;
        return bids ? Object.keys(bids).length : 0;
    });
    const auctionTotalBidders = computed(() => (game.value?.players?.length ?? 1) - 1);

    const auctionWinnerName = computed(() => {
        const winnerId = game.value?.dealAction?.auctionState?.winnerId;
        if (!winnerId) return null;
        if (winnerId === player.value?.id) return 'You';
        return game.value?.players?.find(p => p.id === winnerId)?.name;
    });

    const bidPlusEquity = computed(() => {
        const equity = game.value?.dealAction?.asset?.equity ?? 0;
        return bidAmount.value + equity;
    });

    const placeBid = () => {
        if (isBidValid.value) gameState.placeBid(bidAmount.value);
    };
    const auctionPass = () => gameState.auctionPass();

    watch(() => game.value?.dealAction?.auctionState, (newVal, oldVal) => {
        if (oldVal && !newVal) bidAmount.value = 0;
    });

    const completedMarketAction = ref(false);
    const sellToMarket = (asset: AssetModel) => {
        gameState.sellToMarket(asset);
        completedMarketAction.value = true;
    }
    const marketPass = () => {
        gameState.marketPass();
        completedMarketAction.value = true;
    }

    watch(game, (newVal, oldVal) => {
        if (oldVal?.marketAction && !newVal?.marketAction) {
            rolled.value = 0;
            promptSelectCharity.value = true;
            completedMarketAction.value = false;
        }
    })

    const assetsOfType = computed(() => {
        if (game.value?.marketAction?.purchaseOffer) return player.value?.assets?.filter(x => x.type == game.value!.marketAction?.purchaseOffer?.type)
    })

</script>

<template>
    <div v-if="error" class="absolute top-2 left-1/2 -translate-x-1/2 z-50 bg-red-500 text-white px-4 py-2 rounded shadow-lg text-sm">
        {{ error }}
    </div>
    <div v-if="hasCharity && promptSelectCharity && rolled == 0 && myTurn" class="h-full">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-2 max-w-full m-2">
            <Button @click="chooseCharity(true)"
                    class="block min-h-24 whitespace-pre-wrap bg-pink-200 dark:bg-pink-800 hover:bg-pink-300/70 hover:dark:bg-pink-800/80">
                <div>Use Charity Dice</div>
                <div class="text-xs">(turns remaining: {{ player?.charityTurnsRemaining }})</div>
            </Button>
            <Button @click="chooseCharity(false)"
                    class="block min-h-24 whitespace-pre-wrap bg-gray-200 dark:bg-gray-700 hover:bg-gray-300/70 hover:dark:bg-gray-700/80">
                <div>Skip Charity Roll</div>
                <div class="text-xs">(will still consume 1 charity turn)</div>
            </Button>
        </div>
    </div>
    <DiceRoller v-else-if="readyToRoll && rolled == 0 && myTurn" class="h-full" :diceToRoll="(player?.charityTurnsRemaining ?? 0) > 0 ? 2 : 1"
                @diceRolled="movePlayer"/>
    <div v-else-if="rolled != 0 || !myTurn" class="m-auto text-center">
        <span v-if="myTurn">{{ playerName }} rolled a {{ rolled }}!</span>
        <span v-else>{{ playerName }}'s turn.</span>
        <div v-if="game!.confirmAction" class="m-auto">
            <div v-if="game!.confirmAction.title == 'Payday'" class="m-auto">
                <div class="grid grid-cols-1 m-2 p-2 gap-2 rounded-md bg-slate-300 dark:bg-gray-800 shadow-md">
                    {{ playerName }} got paid: {{ formatCurrency(player!.netIncome ?? 0) }}
                    <Button @click="confirmAction" class="min-h-24 bg-green-200 dark:bg-green-800 hover:bg-green-200/70 hover:dark:bg-green-800/80"
                            :disabled="!myTurn">Great!
                    </Button>
                </div>
            </div>
            <div v-if="game!.confirmAction.title == 'Doodad'" class="m-auto">
                <div class="grid grid-cols-1 m-2 p-2 gap-2 rounded-md bg-slate-300 dark:bg-gray-800 shadow-md">
                    <div>{{ game!.confirmAction.doodad?.name }}</div>
                    <div>{{ formatCurrency(game!.confirmAction.doodad?.cost ?? 0) }}</div>
                    <Button @click="confirmAction" class="min-h-24 bg-yellow-200 dark:bg-yellow-800 hover:bg-yellow-200/70 hover:dark:bg-yellow-800/80"
                            :disabled="!myTurn">Bummer
                    </Button>
                </div>
            </div>
            <div v-if="game!.confirmAction.title == 'Baby'" class="m-auto">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-2 max-w-full m-2">
                    <Button @click="confirmAction" class="min-h-24 bg-blue-200 dark:bg-blue-800 hover:bg-blue-200/70 hover:dark:bg-blue-800/80"
                            :disabled="!myTurn">It's a Boy!
                    </Button>
                    <Button @click="confirmAction" class="min-h-24 bg-pink-200 dark:bg-pink-800 hover:bg-pink-200/70 hover:dark:bg-pink-800/80"
                            :disabled="!myTurn">It's a Girl!
                    </Button>
                </div>
            </div>
            <div v-if="game!.confirmAction.title == 'Downsized'" class="m-auto">
                <div class="grid grid-cols-1 m-2 p-2 gap-2 rounded-md bg-slate-300 dark:bg-gray-800 shadow-md">
                    {{ playerName }} got downsized, and lost: {{ formatCurrency(player!.expenses ?? 0) }}
                    <Button @click="confirmAction" class="min-h-24 bg-red-200 dark:bg-red-800 hover:bg-red-200/70 hover:dark:bg-red-800/80" :disabled="!myTurn">
                        Unfortunate...
                    </Button>
                </div>
            </div>
        </div>
        <div v-else-if="game!.dealAction" class="m-auto">
            <div v-if="!game!.dealAction.asset" class="grid grid-cols-1 md:grid-cols-2 gap-2 m-2 p-2 rounded-md bg-gray-200 dark:bg-gray-800 shadow-md">
                <div class="md:col-span-2">Select a deal</div>
                <Button @click="getDeal(true)"
                        class="bg-linear-to-t from-green-200 to-green-400 dark:from-green-800 dark:to-green-950 min-h-24 hover:from-green-300 hover:dark:from-green-900"
                        :disabled="!myTurn">
                    Big Deal
                </Button>
                <Button @click="getDeal(false)"
                        class="bg-linear-to-t from-green-100 to-green-300 dark:from-green-700 dark:to-green-900 min-h-24 hover:from-green-200 hover:dark:from-green-800"
                        :disabled="!myTurn">
                    Small Deal
                </Button>
            </div>
            <div v-else class="grid grid-cols-2 md:grid-cols-4 m-2 gap-2 p-2 rounded-md bg-gray-200 dark:bg-gray-800 shadow-md">
                <div class="col-span-2 md:col-span-4">{{ game!.dealAction.asset.name }}
                    <hr/>
                </div>
                <div class="col-span-1 ml-auto">Cost:</div>
                <div class="col-span-1 ml-auto">{{ formatCurrency(game!.dealAction.asset.value ?? 0) }}</div>
                <div class="col-span-1 ml-auto">Mortgage:</div>
                <div class="col-span-1 ml-auto">{{ formatCurrency(game!.dealAction.asset.loanAmount ?? 0) }}</div>
                <div class="col-span-1 ml-auto">Down Pay:</div>
                <div class="col-span-1 ml-auto">{{ formatCurrency(game!.dealAction.asset.equity ?? 0) }}</div>
                <div class="col-span-1 ml-auto">Cash Flow:</div>
                <div class="col-span-1 ml-auto">{{ formatCurrency(game!.dealAction.asset.income ?? 0) }}</div>

                <!-- No auction active: show buy/sell/pass -->
                <template v-if="!isAuctionActive">
                    <Button @click="buyDeal" :disabled="(game!.dealAction.asset?.equity ?? 0) > player!.cash! || !myTurn"
                            class="col-span-2 md:col-span-4 min-h-10 bg-blue-200 dark:bg-blue-800 hover:bg-blue-500">Buy Deal
                    </Button>
                    <Button @click="sellDeal" :disabled="!myTurn || (game?.players?.length ?? 0) < 2"
                            class="min-h-10 col-span-2 bg-green-200 dark:bg-green-800 hover:bg-green-500">Sell Deal</Button>
                    <Button @click="confirmAction" class="min-h-10 col-span-2 bg-rose-200 dark:bg-rose-800 hover:bg-rose-500" :disabled="!myTurn">Pass</Button>
                </template>

                <!-- Auction active, not complete -->
                <template v-else-if="!isAuctionComplete">
                    <div v-if="isSeller" class="col-span-2 md:col-span-4 text-center py-4">
                        <div class="text-lg font-semibold">Auction In Progress</div>
                        <div class="text-sm mt-1">{{ auctionResponseCount }} / {{ auctionTotalBidders }} players responded</div>
                    </div>
                    <template v-else-if="!hasRespondedToAuction">
                        <div class="col-span-2 md:col-span-4 text-center text-sm">
                            Enter your bid (max {{ formatCurrency(maxBid) }}):
                        </div>
                        <Input v-model.number="bidAmount" type="number" min="1" :max="maxBid"
                               placeholder="Bid amount"
                               class="col-span-2 md:col-span-4" />
                        <div v-if="bidAmount > 0" class="col-span-2 md:col-span-4 text-center text-sm text-gray-600 dark:text-gray-400">
                            Total cost: {{ formatCurrency(bidPlusEquity) }} (bid + down payment)
                        </div>
                        <Button @click="placeBid" :disabled="!isBidValid"
                                class="col-span-2 min-h-10 bg-blue-200 dark:bg-blue-800 hover:bg-blue-500">Place Bid</Button>
                        <Button @click="auctionPass"
                                class="col-span-2 min-h-10 bg-rose-200 dark:bg-rose-800 hover:bg-rose-500">Pass</Button>
                    </template>
                    <div v-else class="col-span-2 md:col-span-4 text-center py-4">
                        Waiting for other players...
                    </div>
                </template>

                <!-- Auction complete -->
                <template v-else>
                    <div class="col-span-2 md:col-span-4 text-center py-2">
                        <template v-if="auctionWinnerName">
                            <div class="text-lg font-semibold">{{ auctionWinnerName }} won the auction!</div>
                            <div class="text-sm">Winning bid: {{ formatCurrency(game!.dealAction.auctionState!.winningBid ?? 0) }}</div>
                        </template>
                        <div v-else class="text-lg font-semibold">All players passed</div>
                    </div>
                    <Button v-if="isSeller" @click="confirmAction"
                            class="col-span-2 md:col-span-4 min-h-10 bg-green-200 dark:bg-green-800 hover:bg-green-500">Continue</Button>
                </template>
            </div>
        </div>
        <div v-else-if="game!.charityAction" class="m-auto">
            <div class="grid grid-cols-1 md:grid-cols-2 m-2 p-2 gap-2 rounded-md bg-slate-300 dark:bg-gray-800 shadow-md">
                <Button @click="buyCharity" class="min-h-24 whitespace-pre-wrap bg-pink-300 dark:bg-pink-800 hover:bg-pink-400/70 hover:dark:bg-pink-800/80"
                        :disabled="!myTurn">
                    Give 10% of your Total Income to Charity
                </Button>
                <Button @click="confirmAction" class="min-h-24 whitespace-pre-wrap bg-gray-200 dark:bg-gray-700 hover:bg-gray-300/70 hover:dark:bg-gray-700/80"
                        :disabled="!myTurn">
                    No Thanks
                </Button>
            </div>
        </div>
        <div v-else-if="game!.marketAction" class="m-auto text-center">
            <div v-if="completedMarketAction">
                Waiting on other players to complete their action.
            </div>
            <div v-else class="grid grid-cols-2 md:grid-cols-4 m-2 p-2 gap-2 rounded-md bg-slate-300 dark:bg-gray-800 shadow-md">
                <div class="col-span-2 md:col-span-4">{{ game?.marketAction.purchaseOffer?.name }}</div>
                <div class="col-span-1 md:col-span-2 ml-auto">Offer:</div>
                <div class="col-span-1 md:col-span-2 mr-auto">{{ formatCurrency(game!.marketAction.purchaseOffer?.price ?? 0) }}</div>
                <DropdownMenu>
                    <DropdownMenuTrigger class="col-span-2">
                        <Button class="min-h-24 w-full bg-green-200 dark:bg-green-800 hover:bg-green-200/70 dark:hover:bg-green-800/80"
                                :disabled="!player!.assets?.flatMap(x => x.type).includes(game!.marketAction!.purchaseOffer?.type ?? 0)">
                            Sell
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent class="ml-2 border-none text-slate-900 dark:text-blue-300 bg-green-200 dark:bg-green-900/40">
                        <DropdownMenuItem v-for="asset in assetsOfType" @click="sellToMarket(asset)">
                            <div
                                class="grid grid-cols-2 gap-2 bg-green-200 dark:bg-green-800/90 hover:bg-green-200/70 dark:hover:bg-green-800 border radius-md rounded p-2">
                                <strong class="col-span-2 text-center">{{ asset.name }}</strong>
                                <div class="ml-auto">
                                    Purchase price:
                                </div>
                                <div>
                                    {{ formatCurrency((asset.loanAmount ?? 0) + (asset.equity ?? 0)) }}
                                </div>
                                <div class="ml-auto">
                                    Outstanding loan:
                                </div>
                                <div>
                                    {{ formatCurrency(asset.loanAmount ?? 0) }}
                                </div>
                                <div class="ml-auto">
                                    Equity:
                                </div>
                                <div>
                                    {{ formatCurrency(asset.equity ?? 0) }}
                                </div>
                                <div class="ml-auto">
                                    Income provided:
                                </div>
                                <div>
                                    {{ formatCurrency(asset.income ?? 0) }}
                                </div>
                                <hr class="col-span-2"/>
                                <div class="ml-auto">
                                    Profit from sale:
                                </div>
                                <div>
<!--                                    TODO: update this to update per-unit prices -->
                                    {{ formatCurrency(game!.marketAction!.purchaseOffer!.price! - (asset.loanAmount ?? 0)) }}
                                </div>
                            </div>
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
                <Button @click="marketPass" class="col-span-2 min-h-24 bg-rose-200 dark:bg-rose-800 hover:bg-rose-200/70 dark:hover:bg-rose-800/80">
                    Pass
                </Button>
            </div>
        </div>
    </div>
    <div v-else-if="(player?.downsizedTurnsRemaining ?? 0) > 0 && myTurn" class="m-auto text-center">
        <div class="grid grid-cols-1 m-2 p-2 gap-2 rounded-md bg-slate-300 dark:bg-gray-800 shadow-md">
            <div>{{ `${playerName} ${myTurn ? 'are' : 'is'}` }} still downsized</div>
            <div class="text-xs">(turns remaining: {{ player!.downsizedTurnsRemaining! - 1 }})</div>
            <Button @click="confirmAction" class="min-h-24 bg-red-200 dark:bg-red-800 hover:bg-red-200/70 hover:dark:bg-red-800/80" :disabled="!myTurn">
                Unfortunate...
            </Button>
        </div>
    </div>
    <div v-else-if="!myTurn" class="m-auto text-center">
        <span>{{ playerName }}'s turn.</span>
    </div>
</template>