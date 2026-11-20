export class Counter {
    private _value: number;

    private readonly _incrementButton: HTMLButtonElement;
    private readonly _decrementButton: HTMLButtonElement;
    private readonly _input: HTMLInputElement;
    private readonly _wrapper: HTMLDivElement;
    
    get value(): number {
        return this._value;
    }

    set value(next: number) {
        if (Number.isNaN(next)) return;
        this._value = next;
        this._input.value = String(next);
    }
    
    constructor(initialValue: number = 0) {
        this._value = initialValue;

        this._wrapper = document.createElement("div");

        this._decrementButton = document.createElement("button");
        this._decrementButton.type = "button";
        this._decrementButton.textContent = "-";
        this._decrementButton.addEventListener("click", () => {
            this.value = this._value - 1;
        });

        this._input = document.createElement("input");
        this._input.type = "number";
        this._input.value = String(this._value);
        this._input.addEventListener("input", () => {
            if (this._input.value.trim() === "") return;

            const parsed = Number(this._input.value);
            if (!Number.isNaN(parsed)) {
                this._value = parsed;
            }
        });

        this._incrementButton = document.createElement("button");
        this._incrementButton.type = "button";
        this._incrementButton.textContent = "+";
        this._incrementButton.addEventListener("click", () => {
            this.value = this._value + 1;
        });

        this._wrapper.append(this._input, this._decrementButton, this._incrementButton);
    }

    mount(container: HTMLElement): void {
        container.appendChild(this._wrapper);
    }

    unmount(): void {
        this._wrapper.remove();
    }
}