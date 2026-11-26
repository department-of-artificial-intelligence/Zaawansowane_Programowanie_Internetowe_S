export class Counter {
  private _value: number = 0;
  private _input: HTMLInputElement;

  constructor(elementId: string) {
    this._input = document.createElement("input");
    this._input.type = "number";
    this._input.value = this._value.toString();

    let btnMinus: HTMLButtonElement = document.createElement("button");
    btnMinus.innerHTML = "-";
    btnMinus.addEventListener("click", () => {
      this.value = this._value - 1;
    });

    let btnPlus: HTMLButtonElement = document.createElement("button");
    btnPlus.innerHTML = "+";
    btnPlus.addEventListener("click", () => {
      this.value = this._value + 1;
    });

    let container = document.getElementById(elementId);
    if (container !== null) {
      container.appendChild(btnMinus);
      container.appendChild(this._input);
      container.appendChild(btnPlus);
    }
  }

  get value(): number {
    return this._value;
  }

  set value(val: number) {
    this._value = val;
    this._input.value = this._value.toString();
  }
}
