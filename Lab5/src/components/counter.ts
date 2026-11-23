import { fromEvent, Observable, scan, shareReplay, Subscription } from "rxjs";

export class Counter {
    private readonly _count$: Observable<number>;

    private _firstInputSubscription?: Subscription;
    private _secondInputSubscription?: Subscription;

    private _counterButton: HTMLButtonElement;
    private _toggleFirstButton: HTMLButtonElement;
    private _toggleSecondButton: HTMLButtonElement;
    private _firstInput: HTMLInputElement;
    private _secondInput: HTMLInputElement;
    private _wrapper: HTMLDivElement;

    constructor() {
        this._wrapper = document.createElement("div");
        this._counterButton = document.createElement("button");
        this._counterButton.innerText = "+";
        this._toggleFirstButton = document.createElement("button");
        this._toggleFirstButton.innerText = "Show/Hide Input 1";
        this._toggleSecondButton = document.createElement("button");
        this._toggleSecondButton.innerText = "Show/Hide Input 2";
        this._firstInput = document.createElement("input");
        this._firstInput.type = "text";
        this._firstInput.readOnly = true;
        this._secondInput = document.createElement("input");
        this._secondInput.type = "text";
        this._secondInput.readOnly = true;
        this._wrapper.append(this._counterButton, this._toggleFirstButton, this._firstInput, this._toggleSecondButton, this._secondInput);
        this._count$ = fromEvent(this._counterButton, "click").pipe(scan((acc) => acc + 1, 0), shareReplay({ bufferSize: 1, refCount: false }));
        this._count$.subscribe();

        fromEvent(this._toggleFirstButton, "click").subscribe(this._toggleFirstInput);
        fromEvent(this._toggleSecondButton, "click").subscribe(this._toggleSecondInput);
    }

    private _toggleFirstInput(): void {
        if (this._firstInputSubscription) {
            this._firstInputSubscription.unsubscribe();
            this._firstInputSubscription = undefined;
            this._firstInput.value = "";
            return;
        }

        this._firstInputSubscription = this._count$.subscribe((value) => this._firstInput.value = String(value));
    }

    private _toggleSecondInput(): void {
        if (this._secondInputSubscription) {
            this._secondInputSubscription.unsubscribe();
            this._secondInputSubscription = undefined;
            this._secondInput.value = "";
            return;
        }

        this._secondInputSubscription = this._count$.subscribe((value) => {
            this._secondInput.value = String(value);
        });
    }

    public mount(container: HTMLElement): void {
        container.appendChild(this._wrapper);
    }
}
