<script setup lang="ts">
    import { Button } from "@/components/ui/button"
    import DiceRoller from "@/components/board/DiceRoller.vue";
    import { storeToRefs } from "pinia";
    import { useGameStateStore } from "@/stores/gameStateStore.ts";
    import { computed, onMounted, ref } from "vue";
    import { formatCurrency } from "../../helpers/FormatHelper.ts";

    const gameState = useGameStateStore();
    const { game, player } = storeToRefs(gameState);

    const hasCharity = computed(() => (player.value?.charityTurnsRemaining ?? 0) > 0)
    const diceToRoll = ref(1);
    const readyToRoll = ref(false);
    const promptSelectCharity = ref(false);
    const rolled = ref(0);

    onMounted(() => {
        if (!hasCharity.value) {
            readyToRoll.value = true;
        } else {
            promptSelectCharity.value = true;
        }
    });

    const chooseCharity = (useCharity: boolean) => {
        if (useCharity) diceToRoll.value = 2;
        readyToRoll.value = true;
        promptSelectCharity.value = false;
    }

    const movePlayer = (diceRoll: number) => {
        gameState.movePlayer(diceRoll);
        setTimeout(() => {
        }, 3000);
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

    const getDeal = (isBig: boolean) => {
        gameState.getDeal(isBig);
    }

</script>

<template>
    <div v-if="hasCharity && promptSelectCharity && rolled == 0" class="h-full">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-2 max-w-full m-2">
            <Button @click="chooseCharity(true)" class="block min-h-24 whitespace-pre-wrap bg-pink-200 dark:bg-pink-800 hover:bg-pink-500">
                <div>Use Charity Dice</div>
                <div class="text-xs">(turns remaining: {{ player?.charityTurnsRemaining }})</div>
            </Button>
            <Button @click="chooseCharity(false)" class="block min-h-24 whitespace-pre-wrap bg-gray-200 dark:bg-gray-700 hover:bg-gray-500">
                <div>Skip Charity Roll</div>
                <div class="text-xs">(will still consume 1 charity turn)</div>
            </Button>
        </div>
    </div>
    <DiceRoller v-else-if="readyToRoll && rolled == 0" class="h-full" :diceToRoll="(player?.charityTurnsRemaining ?? 0) > 0 ? 2 : 1" @diceRolled="movePlayer"/>
    <div v-else-if="rolled != 0" class="m-auto text-center">
        <span>You rolled a {{ rolled }}!</span>
        <div v-if="game!.confirmAction" class="m-auto">
            <div v-if="game!.confirmAction.title == 'Payday'" class="m-auto">
                <div class="grid grid-cols-1 m-2">
                    <Button @click="confirmAction" class="min-h-24 bg-green-200 dark:bg-green-800 hover:bg-green-500">Great!</Button>
                </div>
            </div>
            <div v-if="game!.confirmAction.title == 'Doodad'" class="m-auto">
                <div class="grid grid-cols-1 m-2">
                    <Button @click="confirmAction" class="min-h-24 bg-yellow-200 dark:bg-yellow-800 hover:bg-yellow-300 hover:dark:bg-yellow-700">Bummer</Button>
                </div>
            </div>
            <div v-if="game!.confirmAction.title == 'Baby'" class="m-auto">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-2 max-w-full m-2">
                    <Button @click="confirmAction" class="min-h-24 bg-blue-200 dark:bg-blue-800 hover:bg-blue-500">It's a Boy!</Button>
                    <Button @click="confirmAction" class="min-h-24 bg-pink-200 dark:bg-pink-800 hover:bg-pink-500">It's a Girl!</Button>
                </div>
            </div>
            <div v-if="game!.confirmAction.title == 'Downsized'" class="m-auto">
                <div class="grid grid-cols-1 m-2">
                    <Button @click="confirmAction" class="min-h-24 bg-red-200 dark:bg-red-800 hover:bg-red-500">Unfortunate...</Button>
                </div>
            </div>
        </div>
        <div v-else-if="game!.dealAction" class="m-auto">
            <div v-if="!game!.dealAction.asset" class="grid grid-cols-1 md:grid-cols-2 gap-2 m-2">
                <Button @click="getDeal(true)"
                        class="bg-linear-to-t from-green-200 to-green-400 dark:from-green-800 dark:to-green-950 min-h-24 hover:from-green-300 hover:dark:from-green-900">
                    Big Deal
                </Button>
                <Button @click="getDeal(false)"
                        class="bg-linear-to-t from-green-100 to-green-300 dark:from-green-700 dark:to-green-900 min-h-24 hover:from-green-200 hover:dark:from-green-800">
                    Small Deal
                </Button>
            </div>
            <div v-else class="grid grid-cols-2 md:grid-cols-4 m-2 gap-2 p-2 rounded-md bg-gray-200 dark:bg-gray-800 shadow-md">
                <div class="col-span-2 md:col-span-4">{{ game!.dealAction.asset.name }}</div>
                <div class="col-span-1 ml-auto">Cost:</div>
                <div class="col-span-1 ml-auto">{{ formatCurrency(game!.dealAction.asset.value ?? 0) }}</div>
                <div class="col-span-1 ml-auto">Mortgage:</div>
                <div class="col-span-1 ml-auto">{{ formatCurrency(game!.dealAction.asset.loanAmount ?? 0) }}</div>
                <div class="col-span-1 ml-auto">Down Pay:</div>
                <div class="col-span-1 ml-auto">{{ formatCurrency(game!.dealAction.asset.equity ?? 0) }}</div>
                <div class="col-span-1 ml-auto">Cash Flow:</div>
                <div class="col-span-1 ml-auto">{{ formatCurrency(game!.dealAction.asset.income ?? 0) }}</div>
                <Button @click="buyDeal" class="col-span-2 md:col-span-4 min-h-10 bg-blue-200 dark:bg-blue-800 hover:bg-blue-500">Buy Deal</Button>
                <Button @click="buyDeal" class="min-h-10 col-span-2 bg-green-200 dark:bg-green-800 hover:bg-green-500">Sell Deal</Button>
                <Button @click="confirmAction" class="min-h-10 col-span-2 bg-rose-200 dark:bg-rose-800 hover:bg-rose-500">Pass</Button>
            </div>
        </div>
        <div v-else-if="game!.charityAction" class="m-auto">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-2 max-w-full m-2">
                <Button @click="buyCharity" class="min-h-24 whitespace-pre-wrap bg-pink-200 dark:bg-pink-800 hover:bg-pink-500">Give 10% of your Total Income to
                    Charity
                </Button>
                <Button @click="confirmAction" class="min-h-24 whitespace-pre-wrap bg-gray-200 dark:bg-gray-700 hover:bg-gray-500">No Thanks</Button>
            </div>
        </div>
        <div v-else-if="game!.marketAction" class="m-auto">
            <Button @click="confirmAction">Ok</Button>
        </div>
    </div>
</template>