<script setup lang="ts">
    import { Button } from "@/components/ui/button"
    import { Input } from "@/components/ui/input"
    import DiceRoller from "@/components/board/DiceRoller.vue";
    import { storeToRefs } from "pinia";
    import { useGameStateStore } from "@/stores/gameStateStore.ts";
    import { computed, onMounted, onUnmounted, ref, watch } from "vue";
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

    const payDoodad = (useCard: boolean) => {
        gameState.payDoodad(useCard);
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

    const dealLoanTerm = ref<number>(3);
    const showDealLoanOption = ref(false);

    const dealShortfall = computed(() => {
        const equity = game.value?.dealAction?.asset?.equity ?? 0;
        const cash = player.value?.cash ?? 0;
        return Math.max(0, equity - cash);
    });

    const canAffordDeal = computed(() => dealShortfall.value <= 0);

    const PAYMENTS_PER_ROUND = 12;

    const dealLoanMonthlyPayment = computed(() => {
        const principal = dealShortfall.value;
        if (principal <= 0 || dealLoanTerm.value < 1) return 0;
        const rate = 0.10;
        const monthlyRate = rate / PAYMENTS_PER_ROUND;
        const payments = dealLoanTerm.value * PAYMENTS_PER_ROUND;
        const payment = principal * monthlyRate / (1 - Math.pow(1 + monthlyRate, -payments));
        return Math.round(payment * 100) / 100;
    });

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

    const buyDealWithLoan = () => {
        gameState.buyDealWithLoan(dealLoanTerm.value);
        rolled.value = 0;
        promptSelectCharity.value = true;
        showDealLoanOption.value = false;
        dealLoanTerm.value = 3;
    };

    watch(() => game.value?.dealAction?.auctionState, (newVal, oldVal) => {
        if (oldVal && !newVal) bidAmount.value = 0;
    });

    watch(() => game.value?.dealAction, () => {
        showDealLoanOption.value = false;
        dealLoanTerm.value = 3;
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

    // Player checklist for auctions
    const auctionChecklist = computed(() => {
        const auction = game.value?.dealAction?.auctionState;
        if (!auction) return [];
        const bids = auction.bids as Record<string, number | null> | undefined;
        return (game.value?.players ?? [])
            .filter(p => p.id !== auction.sellerId)
            .map(p => ({
                emoji: p.emoji ?? '',
                name: p.name ?? '',
                responded: bids != null && (p.id ?? '') in bids
            }));
    });

    // Player checklist for market actions
    const marketChecklist = computed(() => {
        const market = game.value?.marketAction;
        if (!market) return [];
        const responded = market.playersResponded as string[] | undefined;
        const respondedSet = new Set(responded ?? []);
        return (game.value?.players ?? []).map(p => ({
            emoji: p.emoji ?? '',
            name: p.name ?? '',
            responded: respondedSet.has(p.id ?? '')
        }));
    });

    // Downsized auto-accept timer (5s)
    const downsizedCountdown = ref(0);
    let downsizedTimer: ReturnType<typeof setInterval> | null = null;

    const isDownsizedVisible = computed(() =>
        myTurn.value && (
            game.value?.confirmAction?.title === 'Downsized' ||
            (!game.value?.confirmAction && !game.value?.dealAction &&
             !game.value?.marketAction && !game.value?.charityAction &&
             (player.value?.downsizedTurnsRemaining ?? 0) > 0)
        )
    );

    function startDownsizedTimer() {
        if (downsizedTimer) return;
        downsizedCountdown.value = 5;
        downsizedTimer = setInterval(() => {
            downsizedCountdown.value--;
            if (downsizedCountdown.value <= 0) {
                stopDownsizedTimer();
                gameState.endTurn();
                rolled.value = 0;
            }
        }, 1000);
    }

    function stopDownsizedTimer() {
        if (downsizedTimer) { clearInterval(downsizedTimer); downsizedTimer = null; }
        downsizedCountdown.value = 0;
    }

    watch(isDownsizedVisible, (visible) => {
        if (visible) startDownsizedTimer();
        else stopDownsizedTimer();
    }, { immediate: true });

    // Auction timer (30s)
    const auctionCountdown = ref(0);
    let auctionTimer: ReturnType<typeof setInterval> | null = null;

    function startAuctionTimer() {
        stopAuctionTimer();
        auctionCountdown.value = 30;
        auctionTimer = setInterval(() => {
            auctionCountdown.value--;
            if (auctionCountdown.value <= 0) {
                stopAuctionTimer();
                // Auto-pass if we haven't responded yet
                if (!hasRespondedToAuction.value && !isSeller.value) {
                    auctionPass();
                }
            }
        }, 1000);
    }

    function stopAuctionTimer() {
        if (auctionTimer) { clearInterval(auctionTimer); auctionTimer = null; }
        auctionCountdown.value = 0;
    }

    watch(isAuctionActive, (active) => {
        if (active && !isAuctionComplete.value) startAuctionTimer();
        else stopAuctionTimer();
    }, { immediate: true });

    watch(isAuctionComplete, (complete) => {
        if (complete) stopAuctionTimer();
    });

    // Market auto-pass timer (5s) for turn player without assets
    const marketAutoPassCountdown = ref(0);
    let marketAutoPassTimer: ReturnType<typeof setInterval> | null = null;

    const turnPlayerNoMarketAssets = computed(() =>
        myTurn.value && game.value?.marketAction != null &&
        !completedMarketAction.value &&
        (assetsOfType.value?.length ?? 0) === 0
    );

    function startMarketAutoPassTimer() {
        stopMarketAutoPassTimer();
        marketAutoPassCountdown.value = 5;
        marketAutoPassTimer = setInterval(() => {
            marketAutoPassCountdown.value--;
            if (marketAutoPassCountdown.value <= 0) {
                stopMarketAutoPassTimer();
                marketPass();
            }
        }, 1000);
    }

    function stopMarketAutoPassTimer() {
        if (marketAutoPassTimer) { clearInterval(marketAutoPassTimer); marketAutoPassTimer = null; }
        marketAutoPassCountdown.value = 0;
    }

    watch(turnPlayerNoMarketAssets, (val) => {
        if (val) startMarketAutoPassTimer();
        else stopMarketAutoPassTimer();
    }, { immediate: true });

    onUnmounted(() => {
        stopDownsizedTimer();
        stopAuctionTimer();
        stopMarketAutoPassTimer();
    });

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
                    <div class="grid grid-cols-2 gap-2">
                        <Button @click="payDoodad(false)" class="min-h-24 bg-yellow-200 dark:bg-yellow-800 hover:bg-yellow-200/70 hover:dark:bg-yellow-800/80"
                                :disabled="!myTurn || (player?.cash ?? 0) < (game!.confirmAction.doodad?.cost ?? 0)">Pay Cash
                        </Button>
                        <Button @click="payDoodad(true)" class="min-h-24 bg-orange-200 dark:bg-orange-800 hover:bg-orange-200/70 hover:dark:bg-orange-800/80"
                                :disabled="!myTurn">Put on Card
                        </Button>
                    </div>
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
                        {{ myTurn && downsizedCountdown > 0 ? `Unfortunate... (${downsizedCountdown}s)` : 'Unfortunate...' }}
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
                    <!-- Buy outright (only shown when player can afford it) -->
                    <Button v-if="canAffordDeal" @click="buyDeal" :disabled="!myTurn"
                            class="col-span-2 md:col-span-4 min-h-10 bg-blue-200 dark:bg-blue-800 hover:bg-blue-500">
                        Buy Deal
                    </Button>

                    <!-- Cannot afford: show loan option -->
                    <template v-else>
                        <div v-if="!showDealLoanOption" class="col-span-2 md:col-span-4 text-center text-sm text-red-600 dark:text-red-400 py-1">
                            You need {{ formatCurrency(dealShortfall) }} more to buy this deal
                        </div>
                        <Button v-if="!showDealLoanOption" @click="showDealLoanOption = true" :disabled="!myTurn"
                                class="col-span-2 md:col-span-4 min-h-10 bg-amber-200 dark:bg-amber-800 hover:bg-amber-500">
                            Take Out Loan to Buy
                        </Button>

                        <!-- Loan configuration panel -->
                        <div v-else class="col-span-2 md:col-span-4 bg-amber-100 dark:bg-amber-900/40 rounded p-2 text-sm">
                            <div class="flex items-center justify-between mb-2">
                                <span class="font-semibold">Loan for {{ formatCurrency(dealShortfall) }}</span>
                                <Button @click="showDealLoanOption = false"
                                        class="text-xs px-2 py-0.5 bg-slate-200 dark:bg-slate-700 hover:bg-slate-300 dark:hover:bg-slate-600">
                                    Cancel
                                </Button>
                            </div>
                            <div class="flex items-center gap-2 flex-wrap">
                                <div class="flex items-center gap-1">
                                    <span class="text-xs text-gray-500">Term:</span>
                                    <select v-model.number="dealLoanTerm"
                                            class="text-sm rounded border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 px-2 py-1">
                                        <option :value="1">1 yr</option>
                                        <option :value="2">2 yr</option>
                                        <option :value="3">3 yr</option>
                                        <option :value="4">4 yr</option>
                                        <option :value="5">5 yr</option>
                                    </select>
                                </div>
                                <span class="text-xs text-gray-500">{{ formatCurrency(dealLoanMonthlyPayment) }}/mo @ 10%</span>
                                <Button @click="buyDealWithLoan" :disabled="!myTurn"
                                        class="text-xs px-3 py-1 bg-green-600 hover:bg-green-700 text-white">
                                    Borrow & Buy
                                </Button>
                            </div>
                        </div>
                    </template>

                    <Button @click="sellDeal" :disabled="!myTurn || (game?.players?.length ?? 0) < 2"
                            class="min-h-10 col-span-2 bg-green-200 dark:bg-green-800 hover:bg-green-500">Sell Deal</Button>
                    <Button @click="confirmAction" class="min-h-10 col-span-2 bg-rose-200 dark:bg-rose-800 hover:bg-rose-500" :disabled="!myTurn">Pass</Button>
                </template>

                <!-- Auction active, not complete -->
                <template v-else-if="!isAuctionComplete">
                    <div v-if="isSeller" class="col-span-2 md:col-span-4 text-center py-4">
                        <div class="text-lg font-semibold">Auction In Progress</div>
                        <div class="text-sm mt-1">{{ auctionResponseCount }} / {{ auctionTotalBidders }} players responded</div>
                        <div v-if="auctionCountdown > 0" class="text-sm mt-1 text-orange-600 dark:text-orange-400">Time remaining: {{ auctionCountdown }}s</div>
                        <div class="mt-2 text-sm space-y-1">
                            <div v-for="p in auctionChecklist" :key="p.name" class="flex items-center justify-center gap-1">
                                <span>{{ p.emoji }} {{ p.name }}</span>
                                <span v-if="p.responded" class="text-green-600">&#10003;</span>
                                <span v-else class="text-gray-400">&#8987;</span>
                            </div>
                        </div>
                    </div>
                    <template v-else-if="!hasRespondedToAuction">
                        <template v-if="maxBid > 0">
                            <div class="col-span-2 md:col-span-4 text-center text-sm">
                                Enter your bid (max {{ formatCurrency(maxBid) }}){{ auctionCountdown > 0 ? ` — ${auctionCountdown}s` : '' }}:
                            </div>
                            <Input v-model.number="bidAmount" type="number" min="1" :max="maxBid"
                                   placeholder="Bid amount"
                                   class="col-span-2 md:col-span-4" />
                            <div v-if="bidAmount > 0" class="col-span-2 md:col-span-4 text-center text-sm text-gray-600 dark:text-gray-400">
                                Total cost: {{ formatCurrency(bidPlusEquity) }} (bid + down payment)
                            </div>
                            <Button @click="placeBid" :disabled="!isBidValid"
                                    class="col-span-2 min-h-10 bg-blue-200 dark:bg-blue-800 hover:bg-blue-500">Place Bid</Button>
                        </template>
                        <div v-else class="col-span-2 md:col-span-4 text-center text-sm text-red-600 dark:text-red-400">
                            You can't afford this deal{{ auctionCountdown > 0 ? ` — ${auctionCountdown}s` : '' }}
                        </div>
                        <Button @click="auctionPass"
                                :class="maxBid > 0 ? 'col-span-2' : 'col-span-2 md:col-span-4'"
                                class="min-h-10 bg-rose-200 dark:bg-rose-800 hover:bg-rose-500">Pass</Button>
                    </template>
                    <div v-else class="col-span-2 md:col-span-4 text-center py-4">
                        Waiting for other players...
                        <div v-if="auctionCountdown > 0" class="text-sm mt-1 text-orange-600 dark:text-orange-400">Time remaining: {{ auctionCountdown }}s</div>
                        <div class="mt-2 text-sm space-y-1">
                            <div v-for="p in auctionChecklist" :key="p.name" class="flex items-center justify-center gap-1">
                                <span>{{ p.emoji }} {{ p.name }}</span>
                                <span v-if="p.responded" class="text-green-600">&#10003;</span>
                                <span v-else class="text-gray-400">&#8987;</span>
                            </div>
                        </div>
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
                <div>Waiting on other players to complete their action.</div>
                <div class="mt-2 text-sm space-y-1">
                    <div v-for="p in marketChecklist" :key="p.name" class="flex items-center justify-center gap-1">
                        <span>{{ p.emoji }} {{ p.name }}</span>
                        <span v-if="p.responded" class="text-green-600">&#10003;</span>
                        <span v-else class="text-gray-400">&#8987;</span>
                    </div>
                </div>
            </div>
            <div v-else-if="turnPlayerNoMarketAssets" class="grid grid-cols-1 m-2 p-2 gap-2 rounded-md bg-slate-300 dark:bg-gray-800 shadow-md">
                <div class="text-lg font-semibold">{{ game?.marketAction.purchaseOffer?.name }}</div>
                <div>Offer: {{ formatCurrency(game!.marketAction.purchaseOffer?.price ?? 0) }}</div>
                <div class="text-sm text-gray-500">You don't own any assets of this type</div>
                <Button @click="marketPass" class="min-h-10 bg-rose-200 dark:bg-rose-800 hover:bg-rose-200/70 dark:hover:bg-rose-800/80">
                    {{ marketAutoPassCountdown > 0 ? `Pass (${marketAutoPassCountdown}s)` : 'Pass' }}
                </Button>
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
                                    {{ formatCurrency(game!.marketAction!.purchaseOffer!.price! * ((asset.quantity ?? 0) > 0 ? asset.quantity! : 1) - (asset.loanAmount ?? 0)) }}
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
                {{ downsizedCountdown > 0 ? `Unfortunate... (${downsizedCountdown}s)` : 'Unfortunate...' }}
            </Button>
        </div>
    </div>
    <div v-else-if="!myTurn" class="m-auto text-center">
        <span>{{ playerName }}'s turn.</span>
    </div>
</template>