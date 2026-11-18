export class Counter {
    private input: HTMLInputElement;
    private plusButton: HTMLButtonElement;
    private minusButton: HTMLButtonElement;

    constructor(parent: HTMLElement) {
        this.input = document.createElement("input");
        this.input.type = "number";
        this.input.value = "0";

        this.plusButton = document.createElement("button");
        this.plusButton.textContent = "+";
        this.plusButton.addEventListener("click", () => this.change(1));

        this.minusButton = document.createElement("button");
        this.minusButton.textContent = "-";
        this.minusButton.addEventListener("click", () => this.change(-1));

        parent.appendChild(this.input);
        parent.appendChild(this.plusButton);
        parent.appendChild(this.minusButton);
    }

    private change(difference: number): void {
        this.input.value = String(Number(this.input.value) + difference);
    }
}
