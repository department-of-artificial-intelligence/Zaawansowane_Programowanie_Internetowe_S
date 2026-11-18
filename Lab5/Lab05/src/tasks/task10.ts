import { fromEvent, Subscription } from "rxjs";
import { map, scan, take } from "rxjs/operators";
import { addButton, addParagraph } from "../dom";

class ClickCounter {
    private startButton: HTMLButtonElement;
    private stopButton: HTMLButtonElement;
    private countButton: HTMLButtonElement;
    private paragraph: HTMLParagraphElement;
    private subscription?: Subscription;

    constructor(parent: HTMLElement) {
        this.startButton = addButton(parent, "Start");
        this.stopButton = addButton(parent, "Stop");
        this.countButton = addButton(parent, "Click me");
        this.paragraph = addParagraph(parent);
    }

    start(): void {
        fromEvent(this.startButton, "click").subscribe(() => this.startCounting());
        fromEvent(this.stopButton, "click").subscribe(() => this.stopCounting());
    }

    private startCounting(): void {
        if (this.subscription && !this.subscription.closed)
            return;
        this.subscription = fromEvent(this.countButton, "click")
            .pipe(
                take(5),
                scan(count => count + 1, 0),
                map(count => `Clicks: ${count}`))
            .subscribe(text => this.paragraph.textContent = text);
    }

    private stopCounting(): void {
        this.subscription?.unsubscribe();
        this.subscription = undefined;
    }
}

export function task10(parent: HTMLElement): void {
    new ClickCounter(parent).start();
}
