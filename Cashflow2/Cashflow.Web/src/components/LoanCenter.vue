<script setup lang="ts">
import { storeToRefs } from "pinia";
import { useGameStateStore } from "@/stores/gameStateStore.ts";
import { formatCurrency } from "@/helpers/FormatHelper.ts";
import { computed, ref } from "vue";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import type { LiabilityModel } from "@/apiClient";

const gameState = useGameStateStore();
const { player } = storeToRefs(gameState);

const expandedLiabilityId = ref<string | null>(null);
const payAmount = ref<number>(0);
const loanAmount = ref<number>(1000);
const loanTerm = ref<number>(3);
const showLoanForm = ref(false);

const PAYMENTS_PER_ROUND = 12;

const liabilities = computed(() => player.value?.liabilities ?? []);

const previewMonthlyPayment = computed(() => {
    const principal = loanAmount.value;
    const rate = 0.10;
    if (principal <= 0 || loanTerm.value < 1) return 0;
    const monthlyRate = rate / PAYMENTS_PER_ROUND;
    const payments = loanTerm.value * PAYMENTS_PER_ROUND;
    const payment = principal * monthlyRate / (1 - Math.pow(1 + monthlyRate, -payments));
    return Math.round(payment * 100) / 100;
});

function payoffPreviewPayment(liability: LiabilityModel): number {
    const amount = Math.min(payAmount.value, liability.amount ?? 0);
    const remaining = (liability.amount ?? 0) - amount;
    if (remaining <= 0) return 0;
    const rate = liability.interestRate ?? 0;
    const term = liability.term ?? 1;
    const monthlyRate = rate / PAYMENTS_PER_ROUND;
    const payments = term * PAYMENTS_PER_ROUND;
    const payment = remaining * monthlyRate / (1 - Math.pow(1 + monthlyRate, -payments));
    return Math.round(payment * 100) / 100;
}

function toggleRow(id: string) {
    if (expandedLiabilityId.value === id) {
        expandedLiabilityId.value = null;
    } else {
        expandedLiabilityId.value = id;
        payAmount.value = 0;
    }
}

function maxPayable(liability: LiabilityModel): number {
    return Math.min(player.value?.cash ?? 0, liability.amount ?? 0);
}

async function takeOutLoan() {
    if (loanAmount.value <= 0 || loanTerm.value < 1 || loanTerm.value > 5) return;
    await gameState.takeOutLoan(loanAmount.value, loanTerm.value);
    showLoanForm.value = false;
    loanAmount.value = 1000;
    loanTerm.value = 3;
}

async function payLoan(liabilityId: string, amount: number) {
    if (amount <= 0) return;
    await gameState.payOffLoan(liabilityId, amount);
    expandedLiabilityId.value = null;
}

async function payInFull(liability: LiabilityModel) {
    if (!liability.id) return;
    const amount = maxPayable(liability);
    if (amount <= 0) return;
    await gameState.payOffLoan(liability.id, amount);
    expandedLiabilityId.value = null;
}
</script>

<template>
    <div class="border-2 border-slate-900 dark:border-blue-300 rounded-md m-2 p-2 shadow drop-shadow-md shadow-gray-600">
        <strong>Loans</strong>
        <hr class="my-1" />

        <!-- Take Out Loan -->
        <div class="text-sm mb-2">
            <Button v-if="!showLoanForm" @click="showLoanForm = true"
                    class="text-xs px-3 py-1 bg-slate-200 dark:bg-slate-700 hover:bg-slate-300 dark:hover:bg-slate-600 w-full">
                Take Out a Loan
            </Button>
            <div v-else>
                <div class="flex place-content-between items-center mb-1">
                    <span class="font-semibold">Take Out a Loan</span>
                    <Button @click="showLoanForm = false"
                            class="text-xs px-2 py-0.5 bg-slate-200 dark:bg-slate-700 hover:bg-slate-300 dark:hover:bg-slate-600">
                        Cancel
                    </Button>
                </div>
                <div class="flex items-center gap-2 flex-wrap">
                    <div class="flex items-center gap-1">
                        <span class="text-xs text-gray-500">$</span>
                        <Input v-model.number="loanAmount" type="number" min="1" class="w-24 text-sm" />
                    </div>
                    <div class="flex items-center gap-1">
                        <select v-model.number="loanTerm"
                                class="text-sm rounded border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 px-2 py-1">
                            <option :value="1">1 yr</option>
                            <option :value="2">2 yr</option>
                            <option :value="3">3 yr</option>
                            <option :value="4">4 yr</option>
                            <option :value="5">5 yr</option>
                        </select>
                    </div>
                    <span class="text-xs text-gray-500">{{ formatCurrency(previewMonthlyPayment) }}/mo</span>
                    <Button @click="takeOutLoan()"
                            :disabled="loanAmount <= 0 || loanTerm < 1 || loanTerm > 5"
                            class="text-xs px-3 py-1 bg-green-600 hover:bg-green-700 text-white">
                        Borrow
                    </Button>
                </div>
            </div>
        </div>

        <hr class="my-1" />

        <!-- Liability List -->
        <div v-if="liabilities.length === 0" class="text-sm text-gray-500 text-center py-2">
            No liabilities.
        </div>
        <div v-else class="grid grid-cols-[1fr_auto_auto_auto] gap-x-3 gap-y-1 text-sm">
            <template v-for="liability in liabilities" :key="liability.id ?? liability.name">
                <div class="cursor-pointer font-semibold" @click="toggleRow(liability.id ?? '')">
                    {{ liability.name }}
                </div>
                <div class="cursor-pointer text-right" @click="toggleRow(liability.id ?? '')">
                    {{ formatCurrency(liability.amount ?? 0) }}
                </div>
                <div class="cursor-pointer text-right text-gray-500" @click="toggleRow(liability.id ?? '')">
                    {{ ((liability.interestRate ?? 0) * 100).toFixed(0) }}%
                </div>
                <div class="cursor-pointer text-right" @click="toggleRow(liability.id ?? '')">
                    {{ formatCurrency(liability.expense ?? 0) }}/mo
                </div>
                <div v-if="expandedLiabilityId === liability.id" class="col-span-4 bg-slate-200 dark:bg-gray-800 rounded p-2 mb-1">
                    <div class="flex items-center gap-2 flex-wrap">
                        <span class="text-xs text-gray-500">Max: {{ formatCurrency(maxPayable(liability)) }}</span>
                        <Input v-model.number="payAmount" type="number" min="1"
                               :max="maxPayable(liability)"
                               class="w-24 text-sm" />
                        <Button @click="payLoan(liability.id!, payAmount)"
                                :disabled="payAmount <= 0 || payAmount > maxPayable(liability)"
                                class="text-xs px-3 py-1 bg-blue-600 hover:bg-blue-700 text-white">
                            Pay {{ formatCurrency(payAmount) }}
                        </Button>
                        <Button @click="payInFull(liability)"
                                :disabled="(player?.cash ?? 0) < (liability.amount ?? 0)"
                                class="text-xs px-3 py-1 bg-red-600 hover:bg-red-700 text-white">
                            Pay in Full
                        </Button>
                        <span v-if="payAmount > 0 && payAmount <= maxPayable(liability)" class="text-xs text-gray-500">
                            New payment: {{ formatCurrency(payoffPreviewPayment(liability)) }}/mo
                        </span>
                    </div>
                </div>
            </template>
        </div>
    </div>
</template>
