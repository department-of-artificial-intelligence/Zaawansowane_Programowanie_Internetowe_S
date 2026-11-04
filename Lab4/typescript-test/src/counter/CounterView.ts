import { Counter } from "./Counter";

// View - builds the DOM elements and connects them with the model
export class CounterView {
    private input: HTMLInputElement;
    private plusButton: HTMLButtonElement;
    private minusButton: HTMLButtonElement;
    private counter: Counter;

    constructor(counter: Counter, container: HTMLElement) {
        this.counter = counter;

        this.minusButton = document.createElement("button");
        this.minusButton.textContent = "-";

        this.input = document.createElement("input");
        this.input.type = "number";

        this.plusButton = document.createElement("button");
        this.plusButton.textContent = "+";

        container.append(this.minusButton, this.input, this.plusButton);
        this.bindEvents();
        this.render();
    }

    private bindEvents(): void {
        this.plusButton.addEventListener("click", () => {
            this.counter.increment();
            this.render();
        });
        this.minusButton.addEventListener("click", () => {
            this.counter.decrement();
            this.render();
        });
        // the user can also type a value by hand
        this.input.addEventListener("change", () => {
            const value = Number(this.input.value);
            if (this.input.value.trim() !== "" && !Number.isNaN(value)) {
                this.counter.value = value;
            }
            this.render();
        });
    }

    private render(): void {
        this.input.value = String(this.counter.value);
    }
}
