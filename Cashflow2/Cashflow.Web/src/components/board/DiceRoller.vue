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
    const hideRollText = ref(false);
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
        hideRollText.value = true;
        let diceRoll = await diceBox.roll(diceToRoll + 'd6');
        let total = diceRoll.map(x => x.value).reduce((a, b) => a + b, 0);
        emit('diceRolled', total);
    };
</script>

<template>
    <div id="diceZone" class="h-full text-center relative" @click="roll">
        <div :hidden="hideRollText" class="absolute inset-0 flex items-center justify-center z-0">Click to Roll!</div>
    </div>
</template>

<style>
    #diceZone > #dice-canvas {
        width: 100%;
        height: 100%;
    }
</style>