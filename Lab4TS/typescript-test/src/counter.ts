export class CounterApp {
  private _value: number;
  
  private readonly _input: HTMLInputElement;
  private readonly _incBtn: HTMLButtonElement;
  private readonly _decBtn: HTMLButtonElement;

  constructor(container: HTMLElement, initialValue: number = 0) {
    this._value = initialValue;

    this._decBtn = document.createElement('button');
    this._decBtn.type = 'button';
    this._decBtn.textContent = '-';

    this._input = document.createElement('input');
    this._input.type = 'number';
    this._input.value = String(this._value);

    this._incBtn = document.createElement('button');
    this._incBtn.type = 'button';
    this._incBtn.textContent = '+';

    this.bindEvents();
    container.append(this._decBtn, this._input, this._incBtn);
  }

  private bindEvents(): void {
    this._incBtn.addEventListener('click', () => this.increment());
    this._decBtn.addEventListener('click', () => this.decrement());
    this._input.addEventListener('input', () => {
      const parsed = Number(this._input.value);
      if (!Number.isNaN(parsed)) {
        this._value = parsed;
      }
    });
  }

  public increment(): void {
    this._value += 1;
    this.render();
  }

  public decrement(): void {
    this._value -= 1;
    this.render();
  }

  private render(): void {
    this._input.value = String(this._value);
  }
}

// Usage:
// const root = document.getElementById('counter-root');
// if (root) new CounterApp(root, 0);