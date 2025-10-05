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
const rolling = ref(false);

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

const safeDiceSpec = () => {
  const n = Number.isFinite(diceToRoll) ? Math.floor(diceToRoll) : 0;
  const clamped = Math.min(Math.max(n, 1), 20); // prevent abuse
  return `${clamped}d6`;
};

const roll = async () => {
  if (!diceLoaded.value || rolling.value) return;
  rolling.value = true;
  try {
    hideRollText.value = true;
    const diceRoll = await diceBox.roll(safeDiceSpec());
    const total = (diceRoll ?? []).map(x => x?.value ?? 0).reduce((a, b) => a + b, 0);
    emit('diceRolled', total);
  } finally {
    rolling.value = false;
  }
};
</script>

<template>
    <div id="diceZone" class="h-full text-center relative" @click="roll" :disabled="!diceLoaded">
        <div :hidden="hideRollText" class="absolute inset-0 flex items-center justify-center z-0">Click to Roll!</div>
    </div>
</template>

<style>
    #diceZone > #dice-canvas {
        width: 100%;
        height: 100%;
    }
</style>