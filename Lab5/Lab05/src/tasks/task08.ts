import { fromEvent } from "rxjs";
import { map, scan } from "rxjs/operators";
import { addButton, addParagraph } from "../dom";

class ClickCounter {
    private button: HTMLButtonElement;
    private paragraph: HTMLParagraphElement;

    constructor(parent: HTMLElement) {
        this.button = addButton(parent, "Click me");
        this.paragraph = addParagraph(parent);
    }

    start(): void {
        fromEvent(this.button, "click")
            .pipe(
                scan(count => count + 1, 0),
                map(count => `Clicks: ${count}`))
            .subscribe(text => this.paragraph.textContent = text);
    }
}

export function task08(parent: HTMLElement): void {
    new ClickCounter(parent).start();
}
