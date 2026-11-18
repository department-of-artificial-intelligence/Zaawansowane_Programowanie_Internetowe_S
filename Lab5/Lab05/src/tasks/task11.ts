import { fromEvent, Observable, Subscription } from "rxjs";
import { scan, shareReplay, startWith, take } from "rxjs/operators";
import { addButton, addInput } from "../dom";

class ClickCounter {
    private countButton: HTMLButtonElement;
    private firstButton: HTMLButtonElement;
    private secondButton: HTMLButtonElement;
    private firstInput: HTMLInputElement;
    private secondInput: HTMLInputElement;
    private clicks$: Observable<number>;

    constructor(parent: HTMLElement) {
        this.countButton = addButton(parent, "Click me");
        this.firstButton = addButton(parent, "Show in the first field");
        this.secondButton = addButton(parent, "Show in the second field");
        this.firstInput = addInput(parent, "text");
        this.secondInput = addInput(parent, "text");
        this.clicks$ = fromEvent(this.countButton, "click")
            .pipe(
                take(5),
                scan(count => count + 1, 0),
                startWith(0),
                shareReplay(1));
    }

    start(): void {
        this.clicks$.subscribe();
        this.toggle(this.firstButton, this.firstInput);
        this.toggle(this.secondButton, this.secondInput);
    }

    private toggle(button: HTMLButtonElement, input: HTMLInputElement): void {
        let subscription: Subscription | undefined;
        fromEvent(button, "click").subscribe(() => {
            if (subscription) {
                subscription.unsubscribe();
                subscription = undefined;
            } else {
                subscription = this.clicks$.subscribe(count => input.value = String(count));
            }
        });
    }
}

export function task11(parent: HTMLElement): void {
    new ClickCounter(parent).start();
}
