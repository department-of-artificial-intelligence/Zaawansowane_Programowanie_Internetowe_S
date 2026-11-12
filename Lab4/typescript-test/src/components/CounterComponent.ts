export class CounterComponent {
    private _value: number;
    private _container: HTMLElement;
    private _inputElement: HTMLInputElement;
    private _btnPlus: HTMLButtonElement;
    private _btnMinus: HTMLButtonElement;

    constructor(initialValue: number = 0) {
        this._value = initialValue;

        // Kontener główny komponentu
        this._container = document.createElement("div");
        this._container.className = "counter-component";

        // Przycisk "-"
        this._btnMinus = document.createElement("button");
        this._btnMinus.textContent = "-";

        // Pole input
        this._inputElement = document.createElement("input");
        this._inputElement.type = "number";
        this._inputElement.value = this._value.toString();

        // Przycisk "+"
        this._btnPlus = document.createElement("button");
        this._btnPlus.textContent = "+";

        // Budowa struktury DOM
        this._container.appendChild(this._btnMinus);
        this._container.appendChild(this._inputElement);
        this._container.appendChild(this._btnPlus);

        // Podpięcie zdarzeń (Event Listeners)
        this.initEvents();
    }

    get value(): number {
        return this._value;
    }

    set value(newValue: number) {
        this._value = newValue;
        this._inputElement.value = this._value.toString();
    }

    public mount(targetElement: HTMLElement): void {
        targetElement.appendChild(this._container);
    }

    private initEvents(): void {
        this._btnPlus.addEventListener("click", () => this.increment());
        this._btnMinus.addEventListener("click", () => this.decrement());

        // Obsługa ręcznej zmiany wartości w polu input
        this._inputElement.addEventListener("change", () => {
            const parsedValue = parseInt(this._inputElement.value, 10);
            this.value = isNaN(parsedValue) ? 0 : parsedValue;
        });
    }

    public increment(): void {
        this.value += 1;
    }

    public decrement(): void {
        this.value -= 1;
    }
}