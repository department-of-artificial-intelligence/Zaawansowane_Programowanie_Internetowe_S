import { fromEvent } from "rxjs";
import { tap } from "rxjs/operators";
import { addButton, addParagraph } from "../dom";

class ClickCounter {
    private count: number = 0;
    private button: HTMLButtonElement;
    private paragraph: HTMLParagraphElement;

    constructor(parent: HTMLElement) {
        this.button = addButton(parent, "Click me");
        this.paragraph = addParagraph(parent);
    }

    start(): void {
        fromEvent(this.button, "click")
            .pipe(tap(() => this.count++))
            .subscribe(() => this.paragraph.textContent = `Clicks: ${this.count}`);
    }
}

export function task07(parent: HTMLElement): void {
    new ClickCounter(parent).start();
}
