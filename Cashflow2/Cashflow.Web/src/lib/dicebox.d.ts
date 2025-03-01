declare module '@3d-dice/dice-box' {
    interface DiceBoxOptions {
        id?: string;

        /** Path to dice assets (textures, models, etc.) */
        assetPath?: string;

        /** CSS selector or reference for the container where the dice box will render */
        container?: string | HTMLElement;

        /** Scale factor for rendering the dice */
        scale?: number;
    }

    interface RollResult {
        /** String describing the dice rolled (e.g., "1d6") */
        dice: string;

        /** Array of numbers representing the result of the roll */
        values: number[];

        /** Total sum of all dice rolled */
        total: number;
    }

    class DiceBox {
        /**
         * Constructs a new DiceBox instance
         * @param options Configuration for initializing the DiceBox
         */
        constructor(options?: DiceBoxOptions);

        /**
         * Initializes the DiceBox environment
         * @returns A promise that resolves when the DiceBox has loaded
         */
        init(): Promise<void>;

        /**
         * Rolls the specified dice
         * @param diceNotation A dice notation string, e.g., "1d6" or "2d20 + 4"
         * @returns A promise with the results of the roll
         */
        roll(diceNotation: string): Promise<RollResult>;

        /**
         * Clears the current dice from the view
         * @returns A promise that resolves when the dice are cleared
         */
        clear(): Promise<void>;
    }

    export default DiceBox;
}
