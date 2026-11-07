export class Counter {
    private _value: number;
    private _input!: HTMLInputElement;
    private _incrementBtn!: HTMLButtonElement;
    private _decrementBtn!: HTMLButtonElement;
    private _container: HTMLElement;

    constructor(container: HTMLElement, initialValue: number = 0) {
        this._value = initialValue;
        this._container = container;

        this._createElements();
        this._bindEvents();
        this._render();
    }

    get value(): number {
        return this._value;
    }

    set value(newValue: number) {
        this._value = newValue;
        this._updateView();
    }

    private _createElements(): void {
        const wrapper = document.createElement("div");
        wrapper.className = "counter-container";
        wrapper.style.display = "flex";
        wrapper.style.alignItems = "center";
        wrapper.style.gap = "8px";
        wrapper.style.margin = "20px 0";

        // Przycisk dekrementacji "-"
        this._decrementBtn = document.createElement("button");
        this._decrementBtn.textContent = "-";
        this._decrementBtn.type = "button";
        this._decrementBtn.style.padding = "6px 16px";
        this._decrementBtn.style.fontSize = "18px";
        this._decrementBtn.style.cursor = "pointer";

        // Pole tekstowe input
        this._input = document.createElement("input");
        this._input.type = "number";
        this._input.value = this._value.toString();
        this._input.style.textAlign = "center";
        this._input.style.padding = "6px";
        this._input.style.fontSize = "16px";
        this._input.style.width = "80px";

        // Przycisk inkrementacji "+"
        this._incrementBtn = document.createElement("button");
        this._incrementBtn.textContent = "+";
        this._incrementBtn.type = "button";
        this._incrementBtn.style.padding = "6px 16px";
        this._incrementBtn.style.fontSize = "18px";
        this._incrementBtn.style.cursor = "pointer";

        wrapper.appendChild(this._decrementBtn);
        wrapper.appendChild(this._input);
        wrapper.appendChild(this._incrementBtn);

        this._container.appendChild(wrapper);
    }

    private _bindEvents(): void {
        // Reakcja na klikniêcie przycisków
        this._incrementBtn.addEventListener("click", () => this.increment());
        this._decrementBtn.addEventListener("click", () => this.decrement());

        // Obs³uga rêcznej zmiany wartoœci przez u¿ytkownika w polu input
        this._input.addEventListener("input", () => {
            const parsedValue = parseInt(this._input.value, 10);
            this._value = isNaN(parsedValue) ? 0 : parsedValue;
        });
    }

    public increment(): void {
        this._value += 1;
        this._updateView();
    }

    public decrement(): void {
        this._value -= 1;
        this._updateView();
    }

    private _updateView(): void {
        this._input.value = this._value.toString();
    }

    private _render(): void {
        this._updateView();
    }
}