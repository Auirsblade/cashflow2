<script setup lang="ts">
import { storeToRefs } from "pinia";
import { useGameStateStore } from "@/stores/gameStateStore.ts";
import { computed, ref, onMounted, onUnmounted, watch, nextTick } from "vue";
import { formatCurrency } from "@/helpers/FormatHelper.ts";

const { game } = storeToRefs(useGameStateStore());
const stocks = computed(() => game.value?.stockMarket?.stocks ?? []);

const contentRef = ref<HTMLElement | null>(null);
const trackRef = ref<HTMLElement | null>(null);
const offset = ref(0);
const speed = 50; // pixels per second
let animationId = 0;
let lastTime = 0;
let contentWidth = 0;

function measure() {
    if (contentRef.value) {
        contentWidth = contentRef.value.offsetWidth;
    }
}

function animate(time: number) {
    if (!lastTime) lastTime = time;
    const delta = (time - lastTime) / 1000;
    lastTime = time;

    offset.value -= speed * delta;

    if (contentWidth > 0 && offset.value <= -contentWidth) {
        offset.value += contentWidth;
    }

    if (trackRef.value) {
        trackRef.value.style.transform = `translateX(${offset.value}px)`;
    }

    animationId = requestAnimationFrame(animate);
}

onMounted(() => {
    nextTick(() => {
        measure();
        animationId = requestAnimationFrame(animate);
    });
});

watch(stocks, () => {
    nextTick(measure);
}, { deep: true });

onUnmounted(() => {
    cancelAnimationFrame(animationId);
});
</script>

<template>
    <div class="bg-black font-mono text-white border-t border-t-white border-b border-b-white overflow-hidden">
        <div ref="trackRef" class="flex whitespace-nowrap">
            <div ref="contentRef" class="flex-shrink-0">
                <strong v-for="(stock, i) in stocks" :key="`a-${stock.ticker ?? i}`" class="inline-block px-4">
                    <span>{{ stock.ticker }}</span>
                    <span class="ml-2">{{ formatCurrency(stock.currentPrice ?? 0) }}</span>
                    <span v-if="(stock.change ?? 0) !== 0"
                          class="ml-1"
                          :class="(stock.change ?? 0) >= 0 ? 'text-green-400' : 'text-red-400'">
                        {{ (stock.change ?? 0) >= 0 ? '+' : '' }}{{ formatCurrency(stock.change ?? 0) }}
                        ({{ (stock.changePercent ?? 0) >= 0 ? '+' : '' }}{{ stock.changePercent }}%)
                    </span>
                    <span class="ml-4 text-gray-500">|</span>
                </strong>
            </div>
            <div class="flex-shrink-0">
                <strong v-for="(stock, i) in stocks" :key="`b-${stock.ticker ?? i}`" class="inline-block px-4">
                    <span>{{ stock.ticker }}</span>
                    <span class="ml-2">{{ formatCurrency(stock.currentPrice ?? 0) }}</span>
                    <span v-if="(stock.change ?? 0) !== 0"
                          class="ml-1"
                          :class="(stock.change ?? 0) >= 0 ? 'text-green-400' : 'text-red-400'">
                        {{ (stock.change ?? 0) >= 0 ? '+' : '' }}{{ formatCurrency(stock.change ?? 0) }}
                        ({{ (stock.changePercent ?? 0) >= 0 ? '+' : '' }}{{ stock.changePercent }}%)
                    </span>
                    <span class="ml-4 text-gray-500">|</span>
                </strong>
            </div>
        </div>
    </div>
</template>
