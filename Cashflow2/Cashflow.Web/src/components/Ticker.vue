<script setup lang="ts">

const stonks = [
    {
        ticker: "AAPL",
        currentPrice: 44
    },
    {
        ticker: "MSFT",
        currentPrice: 44
    },
    {
        ticker: "TSLA",
        currentPrice: 44
    },
    {
        ticker: "$SPY",
        currentPrice: 44
    },
    {
        ticker: "PSTR",
        currentPrice: 44
    }
]

const tickerValue = stonks.map((item) => `${item.ticker} - $${item.currentPrice}`).join("  |  ");
</script>

<template>
    <div class="marquee bg-black font-mono text-white border-t border-t-white border-b border-b-white">
        <span>
            <strong v-for="(stonk, i) in stonks" class="inline">
                <span v-if="i != 0" class="p-10" >|</span>
                {{ `${stonk.ticker}   -   $${stonk.currentPrice}` }}
            </strong>
        </span>
    </div>
</template>

<style scoped>
.marquee {
    margin: 0 auto;
    white-space: nowrap;
    overflow: hidden;
}

.marquee>span {
    display: inline-block;
    padding-left: 100%;
    animation: marquee 15s linear infinite;
}

/* Make it move */
@keyframes marquee {
    0% {
        transform: translate(0, 0);
    }
    100% {
        transform: translate(-100%, 0);
    }
}
</style>