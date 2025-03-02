<script setup lang="ts">
    import DiceBox from '@3d-dice/dice-box';
    import {onMounted, ref} from "vue";

    const { diceToRoll } = defineProps({
        diceToRoll: {
            type: Number,
            required: true
        }
    });

    const emit = defineEmits(['diceRolled']);

    let diceBox: DiceBox;
    const diceLoaded = ref(false);
    onMounted(async () => {
        diceBox = new DiceBox({
            id: 'dice-canvas',
            assetPath: '/assets/dice/',
            container: '#diceZone',
            scale: 9
        });
        await diceBox.init();
        diceLoaded.value = true;
    });

    const roll = async () => {
        let diceRoll = await diceBox.roll(diceToRoll + 'd6');
        let total = diceRoll.map(x => x.value).reduce((a, b) => a + b, 0);
        emit('diceRolled', total);
    };
</script>

<template>
    <div id="diceZone" class="h-full" @click="roll"/>
</template>

<style>
    #diceZone > #dice-canvas {
        width: 100%;
        height: 100%;
    }
</style>