<script setup lang="ts">

    import {storeToRefs} from "pinia";
    import {useGameStateStore} from "@/stores/gameStateStore.ts";
    import {formatCurrency} from "../helpers/FormatHelper.ts";
    import {computed} from "vue";

    const {player, game} = storeToRefs(useGameStateStore())

    const maxLineItems = computed(() => Math.max(player.value?.liabilities?.length ?? 0, player.value?.assets?.length ?? 0));

</script>

<template>
    <div v-if="player?.profession" class="border-2 border-slate-900 dark:border-blue-300  rounded-md m-2 p-2 shadow drop-shadow-md shadow-gray-600">
        <div class="w-full flex place-content-between">
            <span><strong>{{ player?.name }}</strong> - {{ player?.profession?.name }}</span>
            <strong>Cash: {{ formatCurrency(player?.cash ?? 0) }}</strong>
        </div>
        <hr/>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-2">
            <div class="grid grid-cols-2">
                <div class="col-span-2 font-bold w-full">Income:</div>
                <div class="ml-2">Salary:</div>
                <div class="col-start-2 text-right">
                    <div>{{ formatCurrency(player.profession.salary ?? 0) }}</div>
                </div>
                <template v-for="asset in player.assets">
                    <div class="ml-2">{{ asset.name }}</div>
                    <div class="col-start-2 text-right">
                        <div>{{ formatCurrency(asset.income ?? 0) }}</div>
                    </div>
                </template>
                <template v-for="i in [...Array(maxLineItems - (player.assets?.length ?? 0))]" :key="i">
                    <div class="h-6 max-sm:hidden"/><div class="h-6 max-sm:hidden"/>
                </template>
                <hr/><hr/>
                <div class="font-bold">Total Income:</div>
                <div class="col-start-2 text-right font-bold">
                    <div>{{ formatCurrency(player.income ?? 0) }}</div>
                </div>
            </div>
            <div class="sm:row-start-2 grid grid-cols-2">
                <div class="font-bold">Passive Income:</div>
                <div class="col-start-2 text-right font-bold">
                    <div>{{ formatCurrency((player.income ?? 0) - (player.profession.salary ?? 0)) }}</div>
                </div>
            </div>
            <div class="grid grid-cols-2">
                <div class="col-span-2 font-bold w-full">Expenses:</div>
                <div class="ml-2">Taxes:</div>
                <div class="col-start-2 text-right">
                    <div>{{ formatCurrency(player.taxes ?? 0) }}</div>
                </div>
                <template v-for="liability in player.liabilities">
                    <div class="ml-2">{{ liability.name }}</div>
                    <div class="col-start-2 text-right">
                        <div>{{ formatCurrency(liability.expense ?? 0) }}</div>
                    </div>
                </template>
                <template v-for="i in [...Array(maxLineItems - (player.liabilities?.length ?? 0) - 2)]" :key="i">
                    <div class="h-6 max-sm:hidden"/><div class="h-6 max-sm:hidden"/>
                </template>
                <div class="ml-2">Other Expenses:</div>
                <div class="col-start-2 text-right">
                    <div>{{ formatCurrency(player.profession.otherExpenses ?? 0) }}</div>
                </div>
                <div class="ml-2">
                    Child Expenses:
                    <span class="text-xs">{{player.numberOfChildren! > 0 ? player.numberOfChildren == 1 ? '(1 child)' : `(${player.numberOfChildren} children)` : ''}}</span>
                </div>
                <div class="col-start-2 text-right">
                    <div>{{ formatCurrency(player.childExpenses ?? 0) }}</div>
                </div>
                <hr/><hr/>
                <div class="font-bold">Total Expenses:</div>
                <div class="col-start-2 text-right font-bold">
                    <div>{{ formatCurrency(player.expenses ?? 0) }}</div>
                </div>
            </div>
            <div class="sm:col-start-2 grid grid-cols-2">
                <div class="font-bold">Monthly Cash Flow:</div>
                <div class="col-start-2 text-right font-bold">
                    <div>{{ formatCurrency(player.netIncome ?? 0) }}</div>
                </div>
            </div>
        </div>
    </div>
</template>