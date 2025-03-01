<script setup lang="ts">
    import DiceBox from '@3d-dice/dice-box';
    import {Button} from "@/components/ui/button";
    import {onMounted, ref} from "vue";

    let diceBox: DiceBox;
    const diceLoaded = ref(false);
    onMounted(async () => {
        diceBox = new DiceBox({
            id: 'dice-canvas',
            assetPath: '/src/assets/dice/',
            container: '#boardCenter',
            scale: 9
        });
        await diceBox.init();
        diceLoaded.value = true;
    });

    const hideRollButton = ref(false);
    const roll = async () => {
        console.log(await diceBox.roll('1d6'));
    }

</script>

<template>
    <div id="boardCenter" class="">
        <Button @click="roll" :hidden="hideRollButton" :disabled="!diceLoaded" >Roll</Button>
    </div>
</template>

<style>
    #boardCenter>#dice-canvas {
        width: 100%;
        height: 100%;
    }
</style>