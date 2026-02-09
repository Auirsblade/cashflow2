<script setup lang="ts">
import { storeToRefs } from "pinia";
import { useGameStateStore } from "@/stores/gameStateStore.ts";
import { formatCurrency } from "@/helpers/FormatHelper.ts";
import { computed, ref } from "vue";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import type { StockStateModel, StockPositionModel } from "@/apiClient";
import { StockCategoryModel } from "@/apiClient";

const gameState = useGameStateStore();
const { game, player } = storeToRefs(gameState);

const showMarket = ref(true);
const expandedTicker = ref<string | null>(null);
const tradeQuantity = ref<number | undefined>(undefined);

const stocks = computed(() => game.value?.stockMarket?.stocks ?? []);
const positions = computed(() => player.value?.stockPositions ?? []);

const totalPortfolioValue = computed(() => {
    return positions.value.reduce((sum, pos) => {
        const stock = stocks.value.find(s => s.ticker === pos.ticker);
        return sum + (stock?.currentPrice ?? 0) * (pos.quantity ?? 0);
    }, 0);
});

const totalCostBasis = computed(() => {
    return positions.value.reduce((sum, pos) => {
        return sum + (pos.averageCost ?? 0) * (pos.quantity ?? 0);
    }, 0);
});

const totalPnL = computed(() => {
    return totalPortfolioValue.value - totalCostBasis.value;
});

const totalPnLPercent = computed(() => {
    if (totalCostBasis.value === 0) return 0;
    return ((totalPortfolioValue.value - totalCostBasis.value) / totalCostBasis.value) * 100;
});

const totalShares = computed(() => {
    return positions.value.reduce((sum, pos) => sum + (pos.quantity ?? 0), 0);
});

function toggleRow(ticker: string) {
    if (expandedTicker.value === ticker) {
        expandedTicker.value = null;
    } else {
        expandedTicker.value = ticker;
        tradeQuantity.value = undefined;
    }
}

function maxAffordable(stock: StockStateModel): number {
    if (!stock.currentPrice || stock.currentPrice <= 0) return 0;
    return Math.floor((player.value?.cash ?? 0) / stock.currentPrice);
}

function positionPnL(pos: StockPositionModel): number {
    const stock = stocks.value.find(s => s.ticker === pos.ticker);
    return ((stock?.currentPrice ?? 0) - (pos.averageCost ?? 0)) * (pos.quantity ?? 0);
}

function positionValue(pos: StockPositionModel): number {
    const stock = stocks.value.find(s => s.ticker === pos.ticker);
    return (stock?.currentPrice ?? 0) * (pos.quantity ?? 0);
}

function positionCostBasis(pos: StockPositionModel): number {
    return (pos.averageCost ?? 0) * (pos.quantity ?? 0);
}

function positionPnLPercent(pos: StockPositionModel): number {
    const stock = stocks.value.find(s => s.ticker === pos.ticker);
    const avgCost = pos.averageCost ?? 0;
    if (avgCost === 0) return 0;
    return (((stock?.currentPrice ?? 0) - avgCost) / avgCost) * 100;
}

async function buyStock(ticker: string) {
    if (!tradeQuantity.value || tradeQuantity.value <= 0) return;
    await gameState.buyStock(ticker, tradeQuantity.value);
    expandedTicker.value = null;
}

async function sellStock(ticker: string) {
    if (!tradeQuantity.value || tradeQuantity.value <= 0) return;
    await gameState.sellStock(ticker, tradeQuantity.value);
    expandedTicker.value = null;
}

function ownedQuantity(ticker: string): number {
    return positions.value.find(p => p.ticker === ticker)?.quantity ?? 0;
}

function hasDividend(stock: StockStateModel): boolean {
    return (stock.dividendYield ?? 0) > 0;
}

function isBlueChip(stock: StockStateModel): boolean {
    return stock.category === StockCategoryModel.NUMBER_1;
}
</script>

<template>
    <div class="border-2 border-slate-900 dark:border-blue-300 rounded-md m-2 p-2 shadow drop-shadow-md shadow-gray-600">
        <div class="flex place-content-between items-center mb-1">
            <strong>{{ showMarket ? 'Stock Market' : 'My Stocks' }}</strong>
            <Button @click="showMarket = !showMarket" class="text-xs px-2 py-1 bg-slate-200 dark:bg-slate-700 hover:bg-slate-300 dark:hover:bg-slate-600">
                {{ showMarket ? 'My Stocks' : 'Market' }}
            </Button>
        </div>
        <hr class="mb-2" />

        <!-- Market View -->
        <div v-if="showMarket">
            <div class="grid grid-cols-[auto_1fr_auto_auto] gap-x-3 gap-y-1 text-sm">
                <template v-for="stock in stocks" :key="stock.ticker">
                    <div class="cursor-pointer font-mono font-bold" @click="toggleRow(stock.ticker!)">
                        {{ stock.ticker }}
                        <span v-if="hasDividend(stock)" class="text-yellow-600 dark:text-yellow-500 text-xs ml-1" title="Pays dividends">{{ ((stock.dividendYield ?? 0) * 100).toFixed(1) }}%</span>
                    </div>
                    <div class="cursor-pointer truncate text-gray-500 dark:text-gray-400" @click="toggleRow(stock.ticker!)">
                        {{ stock.name }}
                        <span v-if="isBlueChip(stock)" class="text-blue-600 dark:text-blue-400 text-xs ml-1" title="Blue chip">&#9679;</span>
                    </div>
                    <div class="cursor-pointer text-right" @click="toggleRow(stock.ticker!)">
                        {{ formatCurrency(stock.currentPrice ?? 0) }}
                    </div>
                    <div class="cursor-pointer text-right"
                         :class="(stock.change ?? 0) >= 0 ? 'text-green-700 dark:text-green-400' : 'text-red-700 dark:text-red-400'"
                         @click="toggleRow(stock.ticker!)">
                        <span v-if="(stock.change ?? 0) !== 0">
                            {{ (stock.change ?? 0) >= 0 ? '+' : '' }}{{ (stock.changePercent ?? 0) }}%
                        </span>
                        <span v-else class="text-gray-500 dark:text-gray-400">--</span>
                    </div>
                    <div v-if="expandedTicker === stock.ticker" class="col-span-4 bg-slate-200 dark:bg-gray-800 rounded p-2 mb-1">
                        <div class="flex items-center gap-2 flex-wrap">
                            <span class="text-xs text-gray-500">Max: {{ maxAffordable(stock) }}</span>
                            <Input v-model.number="tradeQuantity" type="number" min="1"
                                   placeholder="Qty" class="w-20 text-sm" />
                            <Button @click="buyStock(stock.ticker!)"
                                    :disabled="!tradeQuantity || tradeQuantity <= 0 || tradeQuantity > maxAffordable(stock)"
                                    class="text-xs px-3 py-1 bg-green-600 hover:bg-green-700 text-white">
                                Buy {{ formatCurrency((stock.currentPrice ?? 0) * (tradeQuantity ?? 0)) }}
                            </Button>
                            <Button v-if="ownedQuantity(stock.ticker!) > 0"
                                    @click="sellStock(stock.ticker!)"
                                    :disabled="!tradeQuantity || tradeQuantity <= 0 || tradeQuantity > ownedQuantity(stock.ticker!)"
                                    class="text-xs px-3 py-1 bg-red-600 hover:bg-red-700 text-white">
                                Sell
                            </Button>
                            <span v-if="ownedQuantity(stock.ticker!) > 0" class="text-xs text-gray-500">Own: {{ ownedQuantity(stock.ticker!) }}</span>
                        </div>
                    </div>
                </template>
            </div>
        </div>

        <!-- Portfolio View -->
        <div v-else>
            <div v-if="positions.length === 0" class="text-sm text-gray-500 text-center py-4">
                No stocks owned. Switch to Market to buy some!
            </div>
            <template v-else>
                <div class="grid grid-cols-[auto_auto_auto_auto_auto_auto] gap-x-3 gap-y-1 text-sm">
                    <!-- Column headers with portfolio totals -->
                    <div class="text-xs text-gray-500 font-semibold">Stock</div>
                    <div class="text-xs text-gray-500 font-semibold text-right">{{ totalShares }} shares</div>
                    <div class="text-xs text-gray-500 font-semibold text-right">Cost {{ formatCurrency(totalCostBasis) }}</div>
                    <div class="text-xs text-gray-500 font-semibold text-right">Value {{ formatCurrency(totalPortfolioValue) }}</div>
                    <div class="text-xs font-semibold text-right"
                         :class="totalPnL >= 0 ? 'text-green-700 dark:text-green-400' : 'text-red-700 dark:text-red-400'">
                        P&L {{ totalPnL >= 0 ? '+' : '' }}{{ formatCurrency(totalPnL) }}
                    </div>
                    <div class="text-xs font-semibold text-right"
                         :class="totalPnLPercent >= 0 ? 'text-green-700 dark:text-green-400' : 'text-red-700 dark:text-red-400'">
                        {{ totalPnLPercent >= 0 ? '+' : '' }}{{ totalPnLPercent.toFixed(1) }}%
                    </div>
                    <div class="col-span-6"><hr class="my-0.5" /></div>
                    <template v-for="pos in positions" :key="pos.ticker">
                        <div class="cursor-pointer font-mono font-bold" @click="toggleRow(pos.ticker!)">
                            {{ pos.ticker }}
                            <span v-if="hasDividend(stocks.find(s => s.ticker === pos.ticker)!)" class="text-yellow-600 dark:text-yellow-500 text-xs ml-1" title="Pays dividends">{{ ((stocks.find(s => s.ticker === pos.ticker)!.dividendYield ?? 0) * 100).toFixed(1) }}%</span>
                            <span v-if="isBlueChip(stocks.find(s => s.ticker === pos.ticker)!)" class="ml-1 text-blue-600 dark:text-blue-400 text-xs leading-5" title="Blue chip">&#9679;</span>
                        </div>
                        <div class="cursor-pointer text-right" @click="toggleRow(pos.ticker!)">
                            {{ pos.quantity }} shares
                        </div>
                        <div class="cursor-pointer text-right text-gray-500" @click="toggleRow(pos.ticker!)">
                            {{ formatCurrency(positionCostBasis(pos)) }}
                        </div>
                        <div class="cursor-pointer text-right" @click="toggleRow(pos.ticker!)">
                            {{ formatCurrency(positionValue(pos)) }}
                        </div>
                        <div class="cursor-pointer text-right"
                             :class="positionPnL(pos) >= 0 ? 'text-green-700 dark:text-green-400' : 'text-red-700 dark:text-red-400'"
                             @click="toggleRow(pos.ticker!)">
                            {{ positionPnL(pos) >= 0 ? '+' : '' }}{{ formatCurrency(positionPnL(pos)) }}
                        </div>
                        <div class="cursor-pointer text-right"
                             :class="positionPnLPercent(pos) >= 0 ? 'text-green-700 dark:text-green-400' : 'text-red-700 dark:text-red-400'"
                             @click="toggleRow(pos.ticker!)">
                            {{ positionPnLPercent(pos) >= 0 ? '+' : '' }}{{ positionPnLPercent(pos).toFixed(1) }}%
                        </div>
                        <div v-if="expandedTicker === pos.ticker" class="col-span-6 bg-slate-200 dark:bg-gray-800 rounded p-2 mb-1">
                            <div class="flex items-center gap-2 flex-wrap">
                                <span class="text-xs text-gray-500">Owned: {{ pos.quantity }} | Avg: {{ formatCurrency(pos.averageCost ?? 0) }}</span>
                                <Input v-model.number="tradeQuantity" type="number" min="1"
                                       placeholder="Qty" class="w-20 text-sm" />
                                <Button @click="sellStock(pos.ticker!)"
                                        :disabled="!tradeQuantity || tradeQuantity <= 0 || tradeQuantity > (pos.quantity ?? 0)"
                                        class="text-xs px-3 py-1 bg-red-600 hover:bg-red-700 text-white">
                                    Sell
                                </Button>
                                <Button @click="buyStock(pos.ticker!)"
                                        :disabled="!tradeQuantity || tradeQuantity <= 0 || tradeQuantity > maxAffordable(stocks.find(s => s.ticker === pos.ticker)!)"
                                        class="text-xs px-3 py-1 bg-green-600 hover:bg-green-700 text-white">
                                    Buy {{ formatCurrency((stocks.find(s => s.ticker === pos.ticker)?.currentPrice ?? 0) * (tradeQuantity ?? 0)) }}
                                </Button>
                            </div>
                        </div>
                    </template>
                </div>
            </template>
        </div>
    </div>
</template>
