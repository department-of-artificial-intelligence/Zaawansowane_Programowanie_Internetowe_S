// Model - knows nothing about HTML
export class Counter {
    private _value: number;
    private _step: number;

    constructor(value: number = 0, step: number = 1) {
        this._value = value;
        this._step = step;
    }

    get value(): number { return this._value; }
    set value(value: number) { this._value = value; }

    increment(): void {
        this._value += this._step;
    }

    decrement(): void {
        this._value -= this._step;
    }
}
